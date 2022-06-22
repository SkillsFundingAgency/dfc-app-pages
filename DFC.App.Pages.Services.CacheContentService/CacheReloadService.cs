﻿using DFC.App.Pages.Data.Common;
using DFC.App.Pages.Data.Contracts;
using DFC.App.Pages.Data.Models;
using DFC.App.Pages.Data.Models.CmsApiModels;
using DFC.Content.Pkg.Netcore.Data.Contracts;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Net;
using System.Threading;
using System.Threading.Tasks;

namespace DFC.App.Pages.Services.CacheContentService
{
    public class CacheReloadService : ICacheReloadService
    {
        private readonly ILogger<CacheReloadService> logger;
        private readonly AutoMapper.IMapper mapper;
        private readonly IEventMessageService<ContentPageModel> eventMessageService;
        private readonly ICmsApiService cmsApiService;
        private readonly IContentCacheService contentCacheService;
        private readonly IAppRegistryApiService appRegistryService;
        private readonly IContentTypeMappingService contentTypeMappingService;
        private readonly IApiCacheService apiCacheService;

        public CacheReloadService(
            ILogger<CacheReloadService> logger,
            AutoMapper.IMapper mapper,
            IEventMessageService<ContentPageModel> eventMessageService,
            ICmsApiService cmsApiService,
            IContentCacheService contentCacheService,
            IAppRegistryApiService appRegistryService,
            IContentTypeMappingService contentTypeMappingService,
            IApiCacheService apiCacheService)
        {
            this.logger = logger;
            this.mapper = mapper;
            this.eventMessageService = eventMessageService;
            this.cmsApiService = cmsApiService;
            this.contentCacheService = contentCacheService;
            this.appRegistryService = appRegistryService;
            this.contentTypeMappingService = contentTypeMappingService;
            this.apiCacheService = apiCacheService;
        }

        public async Task Reload(CancellationToken stoppingToken)
        {
            try
            {
                logger.LogInformation("Reload cache started");

                apiCacheService.StartCache();

                contentTypeMappingService.AddMapping(Constants.ContentTypeHtml, typeof(CmsApiHtmlModel));
                contentTypeMappingService.AddMapping(Constants.ContentTypeHtmlShared, typeof(CmsApiHtmlSharedModel));
                contentTypeMappingService.AddMapping(Constants.ContentTypeSharedContent, typeof(CmsApiSharedContentModel));
                contentTypeMappingService.AddMapping(Constants.ContentTypePageLocation, typeof(CmsApiPageLocationModel));
                contentTypeMappingService.AddMapping(Constants.ContentTypePageLocationParent, typeof(CmsApiPageLocationParentModel));
                contentTypeMappingService.AddMapping(Constants.ContentTypeForm, typeof(CmsApiFormModel));

                await RemoveDuplicateCacheItems().ConfigureAwait(false);

                var summaryList = await GetSummaryListAsync().ConfigureAwait(false);

                if (stoppingToken.IsCancellationRequested)
                {
                    logger.LogWarning("Reload cache cancelled");

                    return;
                }

                if (summaryList != null && summaryList.Any())
                {
                    await ProcessSummaryListAsync(summaryList, stoppingToken).ConfigureAwait(false);

                    if (stoppingToken.IsCancellationRequested)
                    {
                        logger.LogWarning("Reload cache cancelled");

                        return;
                    }

                    await DeleteStaleCacheEntriesAsync(summaryList, stoppingToken).ConfigureAwait(false);
                }

                await PostAppRegistryRefresh().ConfigureAwait(false);

                logger.LogInformation("Reload cache completed");
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error in cache reload");
            }
            finally
            {
                apiCacheService.StopCache();
            }
        }

        public async Task RemoveDuplicateCacheItems()
        {
            logger.LogInformation("Removing duplicate cache items");

            var cachedContentPages = await eventMessageService.GetAllCachedItemsAsync().ConfigureAwait(false);
            var duplicates = cachedContentPages?.GroupBy(s => s.Url).SelectMany(grp => grp.Skip(1)).Select(t => t.Id).ToList();

            if (duplicates != null && duplicates.Any())
            {
                foreach (var id in duplicates)
                {
                    for (int i = 0; i < 10; i++)
                    {
                        if (await eventMessageService.DeleteAsync(id).ConfigureAwait(false) == HttpStatusCode.OK)
                        {
                            break;
                        }
                    }
                }

                logger.LogInformation($"Removed {duplicates.Count} duplicate cache items");
            }
            else
            {
                logger.LogInformation("No duplicate items to be removed");
            }
        }

        public async Task<IList<CmsApiSummaryItemModel>?> GetSummaryListAsync()
        {
            logger.LogInformation("Get summary list");

            var summaryList = await cmsApiService.GetSummaryAsync<CmsApiSummaryItemModel>().ConfigureAwait(false);

            logger.LogInformation("Get summary list completed");

            return summaryList;
        }

        public async Task ProcessSummaryListAsync(IList<CmsApiSummaryItemModel>? summaryList, CancellationToken stoppingToken)
        {
            logger.LogInformation("Process summary list started");

            contentCacheService.Clear();

            foreach (var item in summaryList.OrderByDescending(o => o.Published).ThenBy(o => o.Title))
            {
                if (stoppingToken.IsCancellationRequested)
                {
                    logger.LogWarning("Process summary list cancelled");

                    return;
                }

                await GetAndSaveItemAsync(item, stoppingToken).ConfigureAwait(false);
            }

            logger.LogInformation("Process summary list completed");
        }

        public async Task GetAndSaveItemAsync(CmsApiSummaryItemModel item, CancellationToken stoppingToken)
        {
            _ = item ?? throw new ArgumentNullException(nameof(item));
            var url = Combine(item.Url!.ToString(), "true"); // This enables the multiDirectional support needed for page locations

            try
            {
                logger.LogInformation($"Get details for {item.Title} - {url}");

                var apiDataModel = await cmsApiService.GetItemAsync<CmsApiDataModel>(url).ConfigureAwait(false);

                if (apiDataModel == null)
                {
                    logger.LogWarning($"No details returned from {item.Title} - {url}");

                    return;
                }

                if (stoppingToken.IsCancellationRequested)
                {
                    logger.LogWarning("Process item get and save cancelled");

                    return;
                }

                var contentPageModel = mapper.Map<ContentPageModel>(apiDataModel);

                if (!TryValidateModel(contentPageModel))
                {
                    logger.LogError($"Validation failure for {item.Title} - {url}");

                    return;
                }

                logger.LogInformation($"Updating cache with {item.Title} - {url}");

                var result = await eventMessageService.UpdateAsync(contentPageModel).ConfigureAwait(false);

                if (result == HttpStatusCode.NotFound)
                {
                    logger.LogInformation($"Does not exist, creating cache with {item.Title} - {url}");

                    result = await eventMessageService.CreateAsync(contentPageModel).ConfigureAwait(false);

                    if (result == HttpStatusCode.Created)
                    {
                        logger.LogInformation($"Created cache with {item.Title} - {url}");
                    }
                    else
                    {
                        logger.LogError($"Cache create error status {result} from {item.Title} - {url}");
                    }
                }
                else
                {
                    logger.LogInformation($"Updated cache with {item.Title} - {url}");
                }

                var contentItemIds = contentPageModel.AllContentItemIds;

                contentItemIds.AddRange(contentPageModel.AllPageLocationIds);

                contentCacheService.AddOrReplace(contentPageModel.Id, contentItemIds);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, $"Error in get and save for {item.Title} - {url}");
            }
        }

        private static Uri Combine(string uri1, string uri2)
        {
            uri1 = uri1.TrimEnd('/');
            uri2 = uri2.TrimStart('/');

            return new Uri($"{uri1}/{uri2}", uri1.Contains("http", StringComparison.InvariantCultureIgnoreCase) ? UriKind.Absolute : UriKind.Relative);
        }

        public async Task DeleteStaleCacheEntriesAsync(IList<CmsApiSummaryItemModel> summaryList, CancellationToken stoppingToken)
        {
            logger.LogInformation("Delete stale cache items started");

            var cachedContentPages = await eventMessageService.GetAllCachedItemsAsync().ConfigureAwait(false);

            if (cachedContentPages != null && cachedContentPages.Any())
            {
                var staleContentPages = cachedContentPages.Where(x => !summaryList.Any(z => z.Url == x.Url)).ToList();

                if (staleContentPages.Any())
                {
                    await DeleteStaleItemsAsync(staleContentPages, stoppingToken).ConfigureAwait(false);
                }
            }

            logger.LogInformation("Delete stale cache items completed");
        }

        public async Task DeleteStaleItemsAsync(List<ContentPageModel> staleItems, CancellationToken stoppingToken)
        {
            _ = staleItems ?? throw new ArgumentNullException(nameof(staleItems));

            foreach (var staleContentPage in staleItems)
            {
                if (stoppingToken.IsCancellationRequested)
                {
                    logger.LogWarning("Delete stale cache items cancelled");

                    return;
                }

                logger.LogInformation($"Deleting cache with {staleContentPage.CanonicalName} - {staleContentPage.Id}");

                var deletionResult = await eventMessageService.DeleteAsync(staleContentPage.Id).ConfigureAwait(false);

                if (deletionResult == HttpStatusCode.OK)
                {
                    logger.LogInformation($"Deleted stale cache item {staleContentPage.CanonicalName} - {staleContentPage.Id}");
                }
                else
                {
                    logger.LogError($"Cache delete error status {deletionResult} from {staleContentPage.CanonicalName} - {staleContentPage.Id}");
                }
            }
        }

        public async Task PostAppRegistryRefresh()
        {
            logger.LogInformation("Posting to appRegistry to reload page locations");

            await appRegistryService.PagesDataLoadAsync().ConfigureAwait(false);

            logger.LogInformation("Posted to appRegistry to reload page locations");
        }

        public bool TryValidateModel(ContentPageModel contentPageModel)
        {
            _ = contentPageModel ?? throw new ArgumentNullException(nameof(contentPageModel));
            var (versionWasSet, contentWasSet) = IgnoreContentAndVersionFields(contentPageModel);

            var validationContext = new ValidationContext(contentPageModel, null, null);
            var validationResults = new List<ValidationResult>();
            var isValid = Validator.TryValidateObject(contentPageModel, validationContext, validationResults, true);

            if (!isValid && validationResults.Any())
            {
                foreach (var validationResult in validationResults)
                {
                    logger.LogError($"Error validating {contentPageModel.CanonicalName} - {contentPageModel.Url}: {string.Join(",", validationResult.MemberNames)} - {validationResult.ErrorMessage}");
                }
            }

            ResetContentAndVersionFields(contentPageModel, versionWasSet, contentWasSet);

            if (string.IsNullOrEmpty(contentPageModel.PartitionKey))
            {
                logger.LogError($"Error validating {contentPageModel.CanonicalName} - Partition key (and thus page location) was null");
                return false;
            }

            return isValid;
        }

        /// <summary>
        /// This is used as a workaround as we can't remove the annotations on the inherited type of 'Compui.Cosmos.Models.ContentPageModel', but
        /// do not care to have content or version set.
        /// </summary>
        /// <param name="contentPageModel">A populated content page model - either with content and version set, or not.</param>
        /// <returns>Two bools returning information on whether version and content had to be set or not.</returns>
        private static (bool versionWasSet, bool contentWasSet) IgnoreContentAndVersionFields(ContentPageModel contentPageModel)
        {
            var versionWasSet = false;

            if (contentPageModel.Version == null)
            {
                contentPageModel.Version = Guid.NewGuid();
                versionWasSet = true;
            }

            var contentWasSet = false;

            if (contentPageModel.Content == null)
            {
                contentPageModel.Content = "[ANY CONTENT HERE TO BYPASS CHECK]";
                contentWasSet = true;
            }

            return (versionWasSet, contentWasSet);
        }

        private static void ResetContentAndVersionFields(ContentPageModel contentPageModel, bool versionWasSet, bool contentWasSet)
        {
            if (versionWasSet)
            {
                contentPageModel.Version = null;
            }

            if (contentWasSet)
            {
                contentPageModel.Content = null;
            }
        }
    }
}

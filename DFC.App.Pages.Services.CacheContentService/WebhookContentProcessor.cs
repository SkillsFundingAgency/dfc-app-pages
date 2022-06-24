using DFC.App.Pages.Data.Contracts;
using DFC.App.Pages.Data.Enums;
using DFC.App.Pages.Data.Models;
using DFC.App.Pages.Data.Models.CmsApiModels;
using DFC.Compui.Cosmos.Contracts;
using DFC.Content.Pkg.Netcore.Data.Contracts;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace DFC.App.Pages.Services.CacheContentService
{
    public class WebhookContentProcessor : IWebhookContentProcessor
    {
        private readonly ILogger<WebhookContentProcessor> logger;
        private readonly AutoMapper.IMapper mapper;
        private readonly IEventMessageService<ContentPageModel> eventMessageService;
        private readonly ICmsApiService cmsApiService;
        private readonly IContentPageService<ContentPageModel> contentPageService;
        private readonly IContentCacheService contentCacheService;
        private readonly IEventGridService eventGridService;
        private readonly IPageLocatonUpdater pageLocatonUpdater;
        private readonly IContentItemUpdater contentItemUpdater;

        public WebhookContentProcessor(
            ILogger<WebhookContentProcessor> logger,
            AutoMapper.IMapper mapper,
            IEventMessageService<ContentPageModel> eventMessageService,
            ICmsApiService cmsApiService,
            IContentPageService<ContentPageModel> contentPageService,
            IContentCacheService contentCacheService,
            IEventGridService eventGridService,
            IPageLocatonUpdater pageLocatonUpdater,
            IContentItemUpdater contentItemUpdater)
        {
            this.logger = logger;
            this.mapper = mapper;
            this.eventMessageService = eventMessageService;
            this.cmsApiService = cmsApiService;
            this.contentPageService = contentPageService;
            this.contentCacheService = contentCacheService;
            this.eventGridService = eventGridService;
            this.pageLocatonUpdater = pageLocatonUpdater;
            this.contentItemUpdater = contentItemUpdater;
        }

        public async Task<HttpStatusCode> ProcessContentAsync(Uri url, Guid contentId)
        {
            if (url == null)
            {
                throw new ArgumentNullException(nameof(url));
            }

            url = Combine(url.ToString(), "true"); // This enables the multiDirectional support needed for page locations

            var apiDataModel = await cmsApiService.GetItemAsync<CmsApiDataModel>(url).ConfigureAwait(false);
            var contentPageModel = mapper.Map<ContentPageModel>(apiDataModel);

            if (contentPageModel == null)
            {
                return HttpStatusCode.NoContent;
            }

            if (!TryValidateModel(contentPageModel))
            {
                return HttpStatusCode.BadRequest;
            }

            var existingContentPageModel = await contentPageService.GetByIdAsync(contentId).ConfigureAwait(false);

            var contentResult = await eventMessageService.UpdateAsync(contentPageModel).ConfigureAwait(false);

            if (contentResult == HttpStatusCode.NotFound)
            {
                contentResult = await eventMessageService.CreateAsync(contentPageModel).ConfigureAwait(false);
            }

            if (contentResult == HttpStatusCode.OK || contentResult == HttpStatusCode.Created)
            {
                await eventGridService.CompareAndSendEventAsync(existingContentPageModel, contentPageModel).ConfigureAwait(false);

                var contentItemIds = contentPageModel.AllContentItemIds;

                contentItemIds.AddRange(contentPageModel.AllPageLocationIds);

                contentCacheService.AddOrReplace(contentId, contentItemIds);
            }

            return contentResult;
        }

        public async Task<HttpStatusCode> ProcessContentItemAsync(Uri url, Guid contentItemId)
        {
            HttpStatusCode result = HttpStatusCode.NoContent;
            var contentIds = contentCacheService.GetContentIdsContainingContentItemId(contentItemId);

            if (!contentIds.Any())
            {
                return HttpStatusCode.NoContent;
            }

            foreach (var contentId in contentIds)
            {
                var contentPageModel = await contentPageService.GetByIdAsync(contentId).ConfigureAwait(false);

                if (contentPageModel != null)
                {
                    bool contentWasUpdated;

                    if (contentPageModel.AllPageLocationIds.Contains(contentItemId))
                    {
                        contentWasUpdated = await pageLocatonUpdater.FindAndUpdateAsync(url, contentItemId, contentPageModel.PageLocations).ConfigureAwait(false);
                    }
                    else
                    {
                        contentWasUpdated = await contentItemUpdater.FindAndUpdateAsync(url, contentItemId, contentPageModel.ContentItems).ConfigureAwait(false);
                    }

                    if (contentWasUpdated)
                    {
                        var existingContentPageModel = await contentPageService.GetByIdAsync(contentId).ConfigureAwait(false);

                        await eventMessageService.UpdateAsync(contentPageModel).ConfigureAwait(false);

                        await eventGridService.CompareAndSendEventAsync(existingContentPageModel, contentPageModel).ConfigureAwait(false);
                        result = HttpStatusCode.OK;
                    }
                }
            }

            return result;
        }

        public async Task<HttpStatusCode> DeleteContentAsync(Guid contentId)
        {
            var existingContentPageModel = await contentPageService.GetByIdAsync(contentId).ConfigureAwait(false);
            var result = await eventMessageService.DeleteAsync(contentId).ConfigureAwait(false);

            if (result == HttpStatusCode.OK && existingContentPageModel != null)
            {
                await eventGridService.SendEventAsync(WebhookCacheOperation.Delete, existingContentPageModel).ConfigureAwait(false);

                contentCacheService.Remove(contentId);
            }

            return result;
        }

        public async Task<HttpStatusCode> DeleteContentItemAsync(Guid contentItemId)
        {
            var contentIds = contentCacheService.GetContentIdsContainingContentItemId(contentItemId);

            if (!contentIds.Any())
            {
                return HttpStatusCode.NoContent;
            }

            foreach (var contentId in contentIds)
            {
                var contentPageModel = await contentPageService.GetByIdAsync(contentId).ConfigureAwait(false);

                if (contentPageModel != null)
                {
                    bool removedItem = false;

                    if (contentPageModel.AllContentItemIds.Contains(contentItemId))
                    {
                        removedItem = RemoveContentItem(contentItemId, contentPageModel.ContentItems);
                    }

                    if (contentPageModel.AllPageLocationIds.Contains(contentItemId))
                    {
                        removedItem = RemovePageLocation(contentItemId, contentPageModel.PageLocations);
                    }

                    if (removedItem)
                    {
                        var result = await eventMessageService.UpdateAsync(contentPageModel).ConfigureAwait(false);

                        if (result == HttpStatusCode.OK)
                        {
                            contentCacheService.RemoveContentItem(contentId, contentItemId);
                        }
                    }
                }
            }

            return HttpStatusCode.OK;
        }

        public bool RemoveContentItem(Guid contentItemId, List<ContentItemModel>? items)
        {
            if (items == null || !items.Any())
            {
                return false;
            }

            foreach (var contentItemModel in items)
            {
                if (contentItemModel.ItemId == contentItemId)
                {
                    items.Remove(contentItemModel);
                    return true;
                }

                var removedContentItem = RemoveContentItem(contentItemId, contentItemModel.ContentItems);

                if (removedContentItem)
                {
                    return removedContentItem;
                }
            }

            return false;
        }

        public bool RemovePageLocation(Guid contentItemId, List<PageLocationModel>? items)
        {
            if (items == null || !items.Any())
            {
                return false;
            }

            foreach (var pageLocationModel in items)
            {
                if (pageLocationModel.ItemId == contentItemId)
                {
                    items.Remove(pageLocationModel);
                    return true;
                }

                var removedPageLocation = RemovePageLocation(contentItemId, pageLocationModel.PageLocations);

                if (removedPageLocation)
                {
                    return removedPageLocation;
                }
            }

            return false;
        }

        public bool TryValidateModel(ContentPageModel? contentPageModel)
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

            return isValid;
        }

        private static Uri Combine(string uri1, string uri2)
        {
            uri1 = uri1.TrimEnd('/');
            uri2 = uri2.TrimStart('/');

            return new Uri($"{uri1}/{uri2}", uri1.Contains("http", StringComparison.InvariantCultureIgnoreCase) ? UriKind.Absolute : UriKind.Relative);
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

using DFC.App.Pages.Data.Common;
using DFC.App.Pages.Data.Contracts;
using DFC.App.Pages.Data.Enums;
using DFC.App.Pages.Data.Models;
using DFC.App.Pages.Data.Models.CmsApiModels;
using DFC.Compui.Cosmos.Contracts;
using DFC.Content.Pkg.Netcore.Data.Contracts;
using DFC.Content.Pkg.Netcore.Data.Enums;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace DFC.App.Pages.Services.CacheContentService
{
    public class WebhooksService : IWebhooksService
    {
        private readonly ILogger<WebhooksService> logger;
        private readonly AutoMapper.IMapper mapper;
        private readonly IEventMessageService<ContentPageModel> eventMessageService;
        private readonly ICmsApiService cmsApiService;
        private readonly IContentPageService<ContentPageModel> contentPageService;
        private readonly IContentCacheService contentCacheService;
        private readonly IEventGridService eventGridService;

        public WebhooksService(
            ILogger<WebhooksService> logger,
            AutoMapper.IMapper mapper,
            IEventMessageService<ContentPageModel> eventMessageService,
            ICmsApiService cmsApiService,
            IContentPageService<ContentPageModel> contentPageService,
            IContentCacheService contentCacheService,
            IEventGridService eventGridService)
        {
            this.logger = logger;
            this.mapper = mapper;
            this.eventMessageService = eventMessageService;
            this.cmsApiService = cmsApiService;
            this.contentPageService = contentPageService;
            this.contentCacheService = contentCacheService;
            this.eventGridService = eventGridService;
        }

        public async Task<HttpStatusCode> ProcessMessageAsync(WebhookCacheOperation webhookCacheOperation, Guid eventId, Guid contentId, string apiEndpoint)
        {
            var contentCacheStatus = contentCacheService.CheckIsContentItem(contentId);

            switch (webhookCacheOperation)
            {
                case WebhookCacheOperation.Delete:
                    if (contentCacheStatus == ContentCacheStatus.ContentItem || contentCacheStatus == ContentCacheStatus.Both)
                    {
                        return await DeleteContentItemAsync(contentId).ConfigureAwait(false);
                    }
                    else
                    {
                        return await DeleteContentAsync(contentId).ConfigureAwait(false);
                    }

                case WebhookCacheOperation.CreateOrUpdate:

                    if (!Uri.TryCreate(apiEndpoint, UriKind.Absolute, out Uri? url))
                    {
                        throw new InvalidDataException($"Invalid Api url '{apiEndpoint}' received for Event Id: {eventId}");
                    }

                    if (contentCacheStatus == ContentCacheStatus.ContentItem || contentCacheStatus == ContentCacheStatus.Both)
                    {
                        return await ProcessContentItemAsync(url, contentId).ConfigureAwait(false);
                    }
                    else
                    {
                        return await ProcessContentAsync(url, contentId).ConfigureAwait(false);
                    }

                default:
                    logger.LogError($"Event Id: {eventId} got unknown cache operation - {webhookCacheOperation}");
                    return HttpStatusCode.BadRequest;
            }
        }

        public async Task<HttpStatusCode> ProcessContentAsync(Uri url, Guid contentId)
        {
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

                contentCacheService.AddOrReplace(contentId, contentItemIds);
            }

            return contentResult;
        }

        public async Task<HttpStatusCode> ProcessContentItemAsync(Uri url, Guid contentItemId)
        {
            var contentIds = contentCacheService.GetContentIdsContainingContentItemId(contentItemId);

            if (!contentIds.Any())
            {
                return HttpStatusCode.NoContent;
            }

            var apiDataContentItemModel = await cmsApiService.GetContentItemAsync<CmsApiHtmlModel>(url).ConfigureAwait(false);

            if (apiDataContentItemModel == null)
            {
                return HttpStatusCode.NoContent;
            }

            foreach (var contentId in contentIds)
            {
                var contentPageModel = await contentPageService.GetByIdAsync(contentId).ConfigureAwait(false);

                if (contentPageModel != null)
                {
                    var pageLocationModel = FindPageLocatonItem(contentItemId, contentPageModel.PageLocations);

                    if (pageLocationModel != null)
                    {
                        pageLocationModel.BreadcrumbLinkSegment = apiDataContentItemModel.Title;
                        //TODO: ian: where to get the following from?
                        // pageLocationModel.BreadcrumbText = apiDataContentItemModel.BreadcrumbText;
                        pageLocationModel.LastCached = DateTime.UtcNow;
                    }

                    var contentItemModel = FindContentItem(contentItemId, contentPageModel.ContentItems);

                    if (contentItemModel != null)
                    {
                        switch (contentItemModel.ContentType)
                        {
                            case Constants.ContentTypeHtml:
                            case Constants.ContentTypeHtmlShared:
                            case Constants.ContentTypeSharedContent:
                                contentItemModel.Title = apiDataContentItemModel.Title;
                                contentItemModel.Content = apiDataContentItemModel.Content;
                                contentItemModel.HtmlBody = apiDataContentItemModel.HtmlBody;
                                break;
                            default:
                                mapper.Map(apiDataContentItemModel, contentItemModel);
                                break;
                        }

                        contentItemModel.LastCached = DateTime.UtcNow;
                    }

                    if (contentItemModel != null || pageLocationModel != null)
                    {
                        var existingContentPageModel = await contentPageService.GetByIdAsync(contentId).ConfigureAwait(false);

                        await eventMessageService.UpdateAsync(contentPageModel).ConfigureAwait(false);

                        await eventGridService.CompareAndSendEventAsync(existingContentPageModel, contentPageModel).ConfigureAwait(false);
                    }
                }
            }

            return HttpStatusCode.OK;
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
                    var removedContentitem = RemoveContentItem(contentItemId, contentPageModel.ContentItems);

                    if (removedContentitem)
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

        public ContentItemModel? FindContentItem(Guid itemId, List<ContentItemModel>? items)
        {
            if (items == null || !items.Any())
            {
                return default;
            }

            foreach (var contentItemModel in items)
            {
                if (contentItemModel.ItemId == itemId)
                {
                    return contentItemModel;
                }

                var childContentItemModel = FindContentItem(itemId, contentItemModel.ContentItems);

                if (childContentItemModel != null)
                {
                    return childContentItemModel;
                }
            }

            return default;
        }

        public PageLocationModel? FindPageLocatonItem(Guid itemId, List<PageLocationModel>? items)
        {
            if (items == null || !items.Any())
            {
                return default;
            }

            foreach (var pageLocationModel in items)
            {
                if (pageLocationModel.ItemId == itemId)
                {
                    return pageLocationModel;
                }

                var childContentItemModel = FindPageLocatonItem(itemId, pageLocationModel.PageLocations);

                if (childContentItemModel != null)
                {
                    return childContentItemModel;
                }
            }

            return default;
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

                var removedContentitem = RemoveContentItem(contentItemId, contentItemModel.ContentItems);

                if (removedContentitem)
                {
                    return removedContentitem;
                }
            }

            return false;
        }

        public bool TryValidateModel(ContentPageModel? contentPageModel)
        {
            _ = contentPageModel ?? throw new ArgumentNullException(nameof(contentPageModel));

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

            return isValid;
        }
    }
}

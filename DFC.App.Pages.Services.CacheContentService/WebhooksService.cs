using DFC.App.Pages.Data.Contracts;
using DFC.App.Pages.Data.Enums;
using DFC.App.Pages.Data.Models;
using DFC.Compui.Cosmos.Contracts;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
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

        public async Task<HttpStatusCode> ProcessMessageAsync(WebhookCacheOperation webhookCacheOperation, Guid eventId, Guid contentId, Uri url)
        {
            bool isContentItem = contentCacheService.CheckIsContentItem(contentId);

            switch (webhookCacheOperation)
            {
                case WebhookCacheOperation.Delete:
                    if (isContentItem)
                    {
                        return await DeleteContentItemAsync(contentId).ConfigureAwait(false);
                    }
                    else
                    {
                        return await DeleteContentAsync(contentId).ConfigureAwait(false);
                    }

                case WebhookCacheOperation.CreateOrUpdate:
                    if (isContentItem)
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
            var apiDataModel = await cmsApiService.GetItemAsync(url).ConfigureAwait(false);
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
            contentPageModel.Etag = existingContentPageModel?.Etag;

            var contentResult = await eventMessageService.UpdateAsync(contentPageModel).ConfigureAwait(false);

            if (contentResult == HttpStatusCode.NotFound)
            {
                contentResult = await eventMessageService.CreateAsync(contentPageModel).ConfigureAwait(false);
            }

            if (contentResult == HttpStatusCode.OK || contentResult == HttpStatusCode.Created)
            {
                await eventGridService.CompareAndSendEventAsync(existingContentPageModel, contentPageModel).ConfigureAwait(false);

                var contentItemIds = (from a in contentPageModel.ContentItems select a.ItemId!.Value).ToList();

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

            var apiDataContentItemModel = await cmsApiService.GetContentItemAsync(url).ConfigureAwait(false);

            foreach (var contentId in contentIds)
            {
                var contentPageModel = await contentPageService.GetByIdAsync(contentId).ConfigureAwait(false);

                if (contentPageModel != null)
                {
                    var contentItemModel = contentPageModel.ContentItems.FirstOrDefault(f => f.ItemId == contentItemId);

                    if (contentItemModel != null)
                    {
                        mapper.Map(apiDataContentItemModel, contentItemModel);

                        await eventMessageService.UpdateAsync(contentPageModel).ConfigureAwait(false);
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
                    var contentItemModel = contentPageModel.ContentItems.FirstOrDefault(f => f.ItemId == contentItemId);

                    if (contentItemModel != null)
                    {
                        contentPageModel.ContentItems!.Remove(contentItemModel);

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

using DFC.App.Pages.Data.Contracts;
using DFC.App.Pages.Data.Enums;
using DFC.Content.Pkg.Netcore.Data.Contracts;
using DFC.Content.Pkg.Netcore.Data.Enums;
using Microsoft.Extensions.Logging;
using System;
using System.IO;
using System.Net;
using System.Threading.Tasks;

namespace DFC.App.Pages.Services.CacheContentService
{
    public class WebhooksService : IWebhooksService
    {
        private readonly ILogger<WebhooksService> logger;
        private readonly IContentCacheService contentCacheService;
        private readonly IWebhookContentProcessor webhookContentProcessor;

        public WebhooksService(
            ILogger<WebhooksService> logger,
            IContentCacheService contentCacheService,
            IWebhookContentProcessor webhookContentProcessor)
        {
            this.logger = logger;
            this.contentCacheService = contentCacheService;
            this.webhookContentProcessor = webhookContentProcessor;
        }

        public async Task<HttpStatusCode> ProcessMessageAsync(WebhookCacheOperation webhookCacheOperation, Guid eventId, Guid contentId, string apiEndpoint)
        {
            var contentCacheStatus = contentCacheService.CheckIsContentItem(contentId);

            switch (webhookCacheOperation)
            {
                case WebhookCacheOperation.Delete:
                    if (contentCacheStatus == ContentCacheStatus.ContentItem || contentCacheStatus == ContentCacheStatus.Both)
                    {
                        return await webhookContentProcessor.DeleteContentItemAsync(contentId).ConfigureAwait(false);
                    }
                    else
                    {
                        return await webhookContentProcessor.DeleteContentAsync(contentId).ConfigureAwait(false);
                    }

                case WebhookCacheOperation.CreateOrUpdate:

                    if (!Uri.TryCreate(apiEndpoint, UriKind.Absolute, out Uri? url))
                    {
                        throw new InvalidDataException($"Invalid Api url '{apiEndpoint}' received for Event Id: {eventId}");
                    }

                    if (contentCacheStatus == ContentCacheStatus.ContentItem || contentCacheStatus == ContentCacheStatus.Both)
                    {
                        return await webhookContentProcessor.ProcessContentItemAsync(url, contentId).ConfigureAwait(false);
                    }
                    else
                    {
                        return await webhookContentProcessor.ProcessContentAsync(url, contentId).ConfigureAwait(false);
                    }

                default:
                    logger.LogError($"Event Id: {eventId} got unknown cache operation - {webhookCacheOperation}");
                    return HttpStatusCode.BadRequest;
            }
        }
    }
}

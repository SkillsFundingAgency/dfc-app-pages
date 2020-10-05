using DFC.App.Pages.Data.Contracts;
using DFC.Content.Pkg.Netcore.Data.Contracts;
using FakeItEasy;
using Microsoft.Extensions.Logging;
using System;

namespace DFC.App.Pages.Services.CacheContentService.UnitTests.WebhooksServiceTests
{
    public abstract class BaseWebhooksServiceTests
    {
        protected const string EventTypePublished = "published";
        protected const string EventTypeDraft = "draft";
        protected const string EventTypeDraftDiscarded = "draft-discarded";
        protected const string EventTypeDeleted = "deleted";
        protected const string EventTypeUnpublished = "unpublished";

        protected BaseWebhooksServiceTests()
        {
            Logger = A.Fake<ILogger<WebhooksService>>();
            FakeContentCacheService = A.Fake<IContentCacheService>();
            FakeWebhookContentProcessor = A.Fake<IWebhookContentProcessor>();
        }

        protected Guid ContentIdForCreate { get; } = Guid.NewGuid();

        protected Guid ContentIdForUpdate { get; } = Guid.NewGuid();

        protected Guid ContentIdForDelete { get; } = Guid.NewGuid();

        protected Guid ContentItemIdForDelete { get; } = Guid.NewGuid();

        protected ILogger<WebhooksService> Logger { get; }

        protected IContentCacheService FakeContentCacheService { get; }

        protected IWebhookContentProcessor FakeWebhookContentProcessor { get; }

        protected WebhooksService BuildWebhooksService()
        {
            var service = new WebhooksService(Logger, FakeContentCacheService, FakeWebhookContentProcessor);

            return service;
        }
    }
}
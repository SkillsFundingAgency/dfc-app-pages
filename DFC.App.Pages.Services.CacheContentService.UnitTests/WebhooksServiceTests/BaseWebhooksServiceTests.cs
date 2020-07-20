using DFC.App.Pages.Data.Contracts;
using DFC.App.Pages.Data.Models;
using DFC.Compui.Cosmos.Contracts;
using FakeItEasy;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;

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
            FakeMapper = A.Fake<AutoMapper.IMapper>();
            FakeEventMessageService = A.Fake<IEventMessageService<ContentPageModel>>();
            FakeCmsApiService = A.Fake<ICmsApiService>();
            FakeContentPageService = A.Fake<IContentPageService<ContentPageModel>>();
            FakeContentCacheService = A.Fake<IContentCacheService>();
            FakeEventGridService = A.Fake<IEventGridService>();
        }

        protected Guid ContentIdForCreate { get; } = Guid.NewGuid();

        protected Guid ContentIdForUpdate { get; } = Guid.NewGuid();

        protected Guid ContentIdForDelete { get; } = Guid.NewGuid();

        protected Guid ContentItemIdForCreate { get; } = Guid.NewGuid();

        protected Guid ContentItemIdForUpdate { get; } = Guid.NewGuid();

        protected Guid ContentItemIdForDelete { get; } = Guid.NewGuid();

        protected ILogger<WebhooksService> Logger { get; }

        protected AutoMapper.IMapper FakeMapper { get; }

        protected IEventMessageService<ContentPageModel> FakeEventMessageService { get; }

        protected ICmsApiService FakeCmsApiService { get; }

        protected IContentPageService<ContentPageModel> FakeContentPageService { get; }

        protected IContentCacheService FakeContentCacheService { get; }

        protected IEventGridService FakeEventGridService { get; }

        protected static PagesApiDataModel BuildValidPagesApiContentModel()
        {
            var model = new PagesApiDataModel
            {
                ItemId = Guid.NewGuid(),
                CanonicalName = "an-article",
                BreadcrumbTitle = "An article",
                ExcludeFromSitemap = true,
                Version = Guid.NewGuid(),
                Url = new Uri("https://localhost"),
                ContentLinks = new ContentLinksModel(new JObject())
                {
                    ContentLinks = new List<KeyValuePair<string, List<LinkDetails>>>()
                    {
                        new KeyValuePair<string, List<LinkDetails>>(
                            "test",
                            new List<LinkDetails>
                            {
                                new LinkDetails
                                {
                                    Uri = new Uri("http://www.one.com"),
                                },
                                new LinkDetails
                                {
                                    Uri = new Uri("http://www.two.com"),
                                },
                                new LinkDetails
                                {
                                    Uri = new Uri("http://www.three.com"),
                                },
                            }),
                    },
                },
                ContentItems = new List<PagesApiContentItemModel>
                {
                    BuildValidPagesApiContentItemDataModel(),
                },
                Published = DateTime.UtcNow,
            };

            return model;
        }

        protected static PagesApiContentItemModel BuildValidPagesApiContentItemDataModel()
        {
            var model = new PagesApiContentItemModel
            {
                Alignment = "Left",
                Ordinal = 1,
                Size = 50,
                Content = "<h1>A document</h1>",
            };

            return model;
        }

        protected ContentPageModel BuildValidContentPageModel()
        {
            var model = new ContentPageModel()
            {
                Id = ContentIdForUpdate,
                Etag = Guid.NewGuid().ToString(),
                CanonicalName = "an-article",
                BreadcrumbTitle = "An article",
                IncludeInSitemap = true,
                Version = Guid.NewGuid(),
                Url = new Uri("https://localhost"),
                Content = null,
                ContentItems = new List<ContentItemModel>
                {
                    BuildValidContentItemModel(ContentItemIdForCreate),
                    BuildValidContentItemModel(ContentItemIdForUpdate),
                    BuildValidContentItemModel(ContentItemIdForDelete),
                },
                LastReviewed = DateTime.UtcNow,
            };

            return model;
        }

        protected ContentItemModel BuildValidContentItemModel(Guid contentItemId)
        {
            var model = new ContentItemModel()
            {
                ItemId = contentItemId,
                Version = Guid.NewGuid(),
                LastReviewed = DateTime.Now,
            };

            return model;
        }

        protected WebhooksService BuildWebhooksService()
        {
            var service = new WebhooksService(Logger, FakeMapper, FakeEventMessageService, FakeCmsApiService, FakeContentPageService, FakeContentCacheService, FakeEventGridService);

            return service;
        }
    }
}
using DFC.App.Pages.Data.Contracts;
using DFC.App.Pages.Data.Models;
using DFC.App.Pages.IntegrationTests.Fakes;
using DFC.Compui.Cosmos.Contracts;
using DFC.Compui.Subscriptions.Pkg.NetStandard.Data.Contracts;
using FakeItEasy;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;

namespace DFC.App.Pages.IntegrationTests
{
    public class CustomWebApplicationFactory<TStartup> : WebApplicationFactory<TStartup>
        where TStartup : class
    {
        public CustomWebApplicationFactory()
        {
            MockCosmosRepo = A.Fake<ICosmosRepository<ContentPageModel>>();
            MockContentPageService = A.Fake<IContentPageService<ContentPageModel>>();
        }

        internal ICosmosRepository<ContentPageModel> MockCosmosRepo { get; set; }

        internal IContentPageService<ContentPageModel> MockContentPageService { get; set; }

        internal IEnumerable<ContentPageModel> GetContentPageModels()
        {
            return new List<ContentPageModel>
            {
                new ContentPageModel
                {
                    Id = Guid.NewGuid(),
                    CanonicalName = "test-test",
                    PageLocation = "/top-of-the-tree",
                    RedirectLocations = new List<string>
                    {
                        "/test/test",
                    },
                    Url = new Uri("http://www.test.com"),
                },
                new ContentPageModel
                {
                    Id = Guid.NewGuid(),
                    CanonicalName = "default-page",
                    PageLocation = "/top-of-the-tree",
                    IsDefaultForPageLocation = true,
                    RedirectLocations = new List<string>
                        {
                            "/test/test",
                        },
                    Url = new Uri("http://www.test.com"),
                },
                new ContentPageModel
                {
                    Id = Guid.NewGuid(),
                    CanonicalName = "an-article",
                    PageLocation = "/",
                    IncludeInSitemap = true,
                    Version = Guid.NewGuid(),
                    Url = new Uri("/aaa/bbb", UriKind.Relative),
                    Content = "<h1>A document</h1>",
                    ContentItems = new List<ContentItemModel>
                    {
                        new ContentItemModel
                        {
                            ItemId = Guid.NewGuid(),
                            Alignment = "Left",
                            Ordinal = 1, Size = 50,
                            Content = "<h1>A document</h1>",
                            CreatedDate = DateTime.Now,
                            Title = "title",
                            ContentType = "content type",
                            HtmlBody = "body",
                            ContentItems = new List<ContentItemModel>
                            {
                                new ContentItemModel
                                {
                                    CreatedDate = DateTime.Now,
                                    Url = new Uri("http://www.test.com"),
                                    Content = "content",
                                    ItemId = Guid.NewGuid(),
                                    LastReviewed = DateTime.Now,
                                },
                            },
                        },
                    },
                    LastReviewed = DateTime.UtcNow,
                    CreatedDate = DateTime.UtcNow,
                },
            };
        }

        protected override void ConfigureWebHost(IWebHostBuilder builder)
        {
            builder.ConfigureTestServices(services =>
            {
                var configuration = new ConfigurationBuilder()
                    .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                    .Build();

                services.AddSingleton<IConfiguration>(configuration);
                services.AddTransient<ICacheReloadService, FakeCacheReloadService>();
                services.AddHostedService<FakeCacheReloadBackgroundService>();
                services.AddTransient(sp => MockCosmosRepo);
                services.AddTransient(sp => MockContentPageService);
                services.AddTransient<ISubscriptionRegistrationService, FakeSubscriptionRegistrationService>();
                services.AddTransient<IWebhooksService, FakeWebhooksService>();
            });
        }
    }
}

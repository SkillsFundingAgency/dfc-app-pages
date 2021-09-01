﻿using DFC.App.Pages.Data.Contracts;
using DFC.App.Pages.Data.Models;
using DFC.App.Pages.Data.Models.ClientOptions;
using DFC.App.Pages.Extensions;
using DFC.App.Pages.HttpClientPolicies;
using DFC.App.Pages.IntegrationTests.Extensions;
using DFC.App.Pages.IntegrationTests.Fakes;
using DFC.Compui.Cosmos.Contracts;
using DFC.Content.Pkg.Netcore.Extensions;
using FakeItEasy;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Net.Http;

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

        public HttpClient CreateClientWithWebHostBuilder()
        {
            return WithWebHostBuilder(builder =>
            {
                builder.RegisterServices(MockCosmosRepo, MockContentPageService);
            }).CreateClient();
        }

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
            builder?.ConfigureServices(services =>
            {
                var configuration = new ConfigurationBuilder()
                    .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                    .Build();

                services.AddSingleton<IConfiguration>(configuration);

                const string AppSettingsPolicies = "Policies";
                var policyOptions = configuration.GetSection(AppSettingsPolicies).Get<PolicyOptions>() ?? new PolicyOptions();
                var policyRegistry = services.AddPolicyRegistry();

                services.AddApiServices(configuration, policyRegistry);

                services
                    .AddPolicies(policyRegistry, nameof(AppRegistryClientOptions), policyOptions)
                    .AddHttpClient<IAppRegistryApiService, FakeAppRegistryApiService, AppRegistryClientOptions>(configuration, nameof(AppRegistryClientOptions), nameof(PolicyOptions.HttpRetry), nameof(PolicyOptions.HttpCircuitBreaker));

                services.AddTransient<ICacheReloadService, FakeCacheReloadService>();
                services.AddHostedService<FakeCacheReloadBackgroundService>();

            });
        }
    }
}

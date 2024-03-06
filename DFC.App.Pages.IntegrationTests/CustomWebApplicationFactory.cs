using DFC.App.Pages.Data.Models;
using DFC.Common.SharedContent.Pkg.Netcore.Interfaces;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using System.Collections.Generic;
using System;

namespace DFC.App.Pages.IntegrationTests
{
    public class CustomWebApplicationFactory<TStartup> : WebApplicationFactory<TStartup>
        where TStartup : class
    {
        public CustomWebApplicationFactory()
        {
            this.MockSharedContentRedis = new Mock<ISharedContentRedisInterface>();
        }

        public Mock<ISharedContentRedisInterface> MockSharedContentRedis { get; set; }

        //TODO: To recreate with latest Page model and to update related tests
        internal IEnumerable<ContentPageModel> GetContentPageModels()
        {
            return new List<ContentPageModel>
            {
                new ContentPageModel
                {
                    Id = Guid.NewGuid(), CanonicalName = "test-test",
                    PageLocation = "/top-of-the-tree",
                    RedirectLocations = new List<string>
                    {
                        "/test/test",
                    }, Url = new Uri("http://www.test.com"),
                },
                new ContentPageModel
                {
                    Id = Guid.NewGuid(), CanonicalName = "default-page",
                    PageLocation = "/top-of-the-tree",
                    IsDefaultForPageLocation = true,
                    RedirectLocations = new List<string>
                    {
                        "/test/test",
                    }, Url = new Uri("http://www.test.com"),
                },
                new ContentPageModel
                {
                    LastReviewed = DateTime.UtcNow,
                    CreatedDate = DateTime.UtcNow,
                    Id = Guid.NewGuid(), CanonicalName = "an-article",
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
                            Ordinal = 1,
                            Size = 50,
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
            });

            builder.ConfigureTestServices(services =>
            {
                services.AddScoped(_ => MockSharedContentRedis.Object);
            });
        }
    }
}

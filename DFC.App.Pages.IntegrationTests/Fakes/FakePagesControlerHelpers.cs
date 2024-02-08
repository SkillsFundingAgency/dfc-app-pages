using DFC.App.Pages.Data.Contracts;
using DFC.App.Pages.Data.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DFC.App.Pages.IntegrationTests.Fakes
{
    public class FakePagesControlerHelpers : IPagesControlerHelpers
    {
        public Task<ContentPageModel?> GetContentPageAsync(string? location, string? article)
        {
            ContentPageModel? result = new ContentPageModel
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
            };
            return Task.FromResult(result ?? default);
        }

        public Task<ContentPageModel?> GetContentPageFromSharedAsync(string? location, string? article)
        {
            ContentPageModel? result = new ContentPageModel
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
            };
            return Task.FromResult(result ?? default);
        }

        public Task<ContentPageModel?> GetRedirectedContentPageAsync(string? location, string? article)
        {
            var result = new ContentPageModel();

            return Task.FromResult(result ?? default);
        }
    }
}
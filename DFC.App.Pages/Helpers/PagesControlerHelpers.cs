using AutoMapper;
using DFC.App.Pages.Cms.Data.Interface;
using DFC.App.Pages.Cms.Data.Model;
using DFC.App.Pages.Data.Contracts;
using DFC.App.Pages.Data.Models;
using DFC.App.Pages.Models;
using DFC.Compui.Cosmos.Contracts;

using Microsoft.Extensions.Caching.Memory;

using System;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace DFC.App.Pages.Helpers
{
    public class PagesControlerHelpers : IPagesControlerHelpers
    {
        private const int CacheDurationInSeconds = 30;
        private readonly IContentPageService<ContentPageModel> contentPageService;
        private readonly IMemoryCache memoryCache;
        private readonly IPageService pageService;
        private readonly AutoMapper.IMapper mapper;

        public PagesControlerHelpers(IContentPageService<ContentPageModel> contentPageService, IMemoryCache memoryCache, IPageService pageService, AutoMapper.IMapper mapper)
        {
            this.contentPageService = contentPageService;
            this.memoryCache = memoryCache;
            this.pageService = pageService;
            this.mapper = mapper;
        }

        public static (string location, string? article) ExtractPageLocation(PageRequestModel pageRequestModel)
        {
            _ = pageRequestModel ?? throw new ArgumentNullException(nameof(pageRequestModel));

            var pageLocation = string.Join("/", new[] { pageRequestModel.Location1, pageRequestModel.Location2, pageRequestModel.Location3, pageRequestModel.Location4, pageRequestModel.Location5 });
            var pageLocations = pageLocation.Split("/", StringSplitOptions.RemoveEmptyEntries);
            var location = string.Empty;
            var article = string.Empty;

            if (pageLocations.Length == 1)
            {
                location = pageLocations.First();
            }
            else if (pageLocations.Length > 1)
            {
                location = string.Join("/", pageLocations, 0, pageLocations.Length - 1);
                article = pageLocations.Last();
            }

            return (location, article);
        }

        public async Task<ContentPageModel?> GetRedirectedContentPageAsync(string? location, string? article)
        {
            var redirectLocation = $"/{location}";

            if (!string.IsNullOrWhiteSpace(article))
            {
                redirectLocation += $"/{article}";
            }

            if (!memoryCache.TryGetValue(redirectLocation, out ContentPageModel? content))
            {
                content = await contentPageService.GetByRedirectLocationAsync(redirectLocation);

                memoryCache.Set(redirectLocation, content, TimeSpan.FromSeconds(CacheDurationInSeconds));
            }

            return content;
        }

        public async Task<ContentPageModel?> GetContentPageAsync(string? location, string? article)
        {
            var cacheKey = BuildCacheKey(location, article);

            if (!memoryCache.TryGetValue(cacheKey, out ContentPageModel? content))
            {
                content = await GetContentPageWithoutCacheAsync(location, article);

                memoryCache.Set(cacheKey, content, TimeSpan.FromSeconds(CacheDurationInSeconds));
            }

            return content;
        }

        public async Task<ContentPageModel?> GetContentPageFromPageServiceAsync(string? location, string? article)
        {
            string pageUrl = GetPageUrl(location, article);
            var pageResponse = await this.pageService.GetPage(pageUrl);
            ContentPageModel? content = new ContentPageModel();
            mapper.Map(pageResponse.First(), content);
            return content;
        }

        private string GetPageUrl(string location, string article)
        {
            string pageUrl = string.Empty;
            if (string.IsNullOrWhiteSpace(location) && string.IsNullOrWhiteSpace(article))
            {
                pageUrl = "/home";
            }
            else if (location == "home")
            {
                pageUrl = $"/{location}";
            }
            else
            {
                pageUrl = $"/{location}/{(string.IsNullOrWhiteSpace(article) ? location : article)}";
            }
            return pageUrl;
        }

        private string BuildCacheKey(string? location, string? article) =>
            $"{nameof(GetContentPageAsync)}:Location:{location}:Article:{article}";

        private async Task<ContentPageModel?> GetContentPageWithoutCacheAsync(string? location, string? article)
        {
            Expression<Func<ContentPageModel, bool>> where;

            if (string.IsNullOrWhiteSpace(location) && string.IsNullOrWhiteSpace(article))
            {
                where = p => p.PageLocation == "/" && p.IsDefaultForPageLocation;
            }
            else if (string.IsNullOrWhiteSpace(article))
            {
                where = p => (p.PageLocation == $"/{location}" && p.IsDefaultForPageLocation) || (p.PageLocation == "/" && p.CanonicalName == location);
            }
            else
            {
                where = p => (p.PageLocation == $"/{location}" && p.CanonicalName == article) || (p.PageLocation == $"/{location}/{article}" && p.IsDefaultForPageLocation);
            }

            var contentPageModels = await contentPageService.GetAsync(where);

            if (contentPageModels == null || !contentPageModels.Any())
            {
                var searchLocation = string.IsNullOrWhiteSpace(article) ? $"/{location}" : $"/{location}/{article}";
                where = p => p.PageLocation == searchLocation && !p.IsDefaultForPageLocation;

                contentPageModels = await contentPageService.GetAsync(where);
            }

            if (contentPageModels != null && contentPageModels.Any())
            {
                return contentPageModels.OrderBy(o => o.CreatedDate).First();
            }

            return default;
        }
    }
}

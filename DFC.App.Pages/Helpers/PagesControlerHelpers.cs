using AutoMapper;
using DFC.App.Pages.Cms.Data.Content;
using DFC.App.Pages.Data.Contracts;
using DFC.App.Pages.Data.Models;
using DFC.App.Pages.Models;
using DFC.Common.SharedContent.Pkg.Netcore.Interfaces;
using DFC.Common.SharedContent.Pkg.Netcore.Model.ContentItems;
using DFC.Compui.Cosmos.Contracts;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;
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
        //private readonly IMemoryCache memoryCache;
        private ISharedContentRedisInterface sharedContentRedisInterface;
        private readonly AutoMapper.IMapper mapper;
        private IOptionsMonitor<contentModeOptions> _options;
        private string status;

        public PagesControlerHelpers(IContentPageService<ContentPageModel> contentPageService, ISharedContentRedisInterface sharedContentRedisInterface, AutoMapper.IMapper mapper, IOptionsMonitor<contentModeOptions> options)
        {
            this.contentPageService = contentPageService;
            //this.memoryCache = memoryCache;
            this.sharedContentRedisInterface = sharedContentRedisInterface;
            this.mapper = mapper;
            _options = options;
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

        public async Task<ContentPageModel?> GetContentPageFromSharedAsync(string? location, string? article)
        {
            if (_options.CurrentValue.contentMode != null)
            {
                status = _options.CurrentValue.contentMode;
            }
            else
            {
                status = "PUBLISHED";
            }
            string pageUrl = GetPageUrl(location, article);
            var pageResponse = await this.sharedContentRedisInterface.GetDataAsync<Page>("Page" + pageUrl + "/" + status, status);
            ContentPageModel? content = new ContentPageModel();
            mapper.Map(pageResponse, content);
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
    }
}
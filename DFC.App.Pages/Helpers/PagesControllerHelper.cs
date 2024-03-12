using AutoMapper;
using DFC.App.Pages.Cms.Data.Content;
using DFC.App.Pages.Data.Contracts;
using DFC.App.Pages.Data.Models;
using DFC.Common.SharedContent.Pkg.Netcore.Interfaces;
using DFC.Common.SharedContent.Pkg.Netcore.Model.ContentItems;
using Microsoft.Extensions.Options;
using System.Threading.Tasks;

namespace DFC.App.Pages.Helpers
{
    public class PagesControllerHelper : IPagesControllerHelper
    {
        private readonly IMapper mapper;
        private readonly ISharedContentRedisInterface sharedContentRedisInterface;
        private readonly IOptionsMonitor<contentModeOptions> options;
        private string status = string.Empty;

        public PagesControllerHelper(
            ISharedContentRedisInterface sharedContentRedisInterface,
            IMapper mapper,
            IOptionsMonitor<contentModeOptions> options)
        {
            this.sharedContentRedisInterface = sharedContentRedisInterface;
            this.mapper = mapper;
            this.options = options;
        }

        public string GetPageUrl(string location, string article)
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

        public async Task<ContentPageModel?> GetContentPageFromSharedAsync(string? location, string? article)
        {
            if (options.CurrentValue.contentMode != null)
            {
                status = options.CurrentValue.contentMode;
            }
            else
            {
                status = "PUBLISHED";
            }

            string pageUrl = GetPageUrl(location, article);

            var pageResponse = await this.sharedContentRedisInterface.GetDataAsync<Page>("Page" + pageUrl, status);

            ContentPageModel? content = new ();

            mapper.Map(pageResponse, content);

            return content;
        }
    }
}

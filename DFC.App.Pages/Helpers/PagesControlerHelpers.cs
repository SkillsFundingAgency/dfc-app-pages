using DFC.App.Pages.Data.Contracts;
using DFC.App.Pages.Data.Models;
using DFC.App.Pages.Models;
using DFC.Compui.Cosmos.Contracts;
using System;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace DFC.App.Pages.Helpers
{
    public class PagesControlerHelpers : IPagesControlerHelpers
    {
        private readonly IContentPageService<ContentPageModel> contentPageService;

        public PagesControlerHelpers(IContentPageService<ContentPageModel> contentPageService)
        {
            this.contentPageService = contentPageService;
        }

        public static (string location, string? article) ExtractPageLocation(PageRequestModel pageRequestModel)
        {
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

        public async Task<ContentPageModel?> GetContentPageAsync(string? location, string? article)
        {
            Expression<Func<ContentPageModel, bool>> where;

            if (string.IsNullOrWhiteSpace(location) && string.IsNullOrWhiteSpace(article))
            {
                where = p => p.PartitionKey == "/" && p.IsDefaultForPageLocation;
            }
            else if (string.IsNullOrWhiteSpace(article))
            {
                where = p => (p.PartitionKey == $"/{location}" && p.IsDefaultForPageLocation) || (p.PartitionKey == "/" && p.CanonicalName == location);
            }
            else
            {
                where = p => (p.PartitionKey == $"/{location}" && p.CanonicalName == article) || (p.PartitionKey == $"/{location}/{article}" && p.IsDefaultForPageLocation);
            }

            var contentPageModels = await contentPageService.GetAsync(where).ConfigureAwait(false);

            if (contentPageModels != null && contentPageModels.Any())
            {
                return contentPageModels.First();
            }

            return default;
        }

        public async Task<ContentPageModel?> GetRedirectedContentPageAsync(string? location, string? article)
        {
            var redirectLocation = $"/{location}";

            if (!string.IsNullOrWhiteSpace(article))
            {
                redirectLocation += $"/{article}";
            }

            var contentPageModel = await contentPageService.GetByRedirectLocationAsync(redirectLocation).ConfigureAwait(false);

            return contentPageModel;
        }
    }
}

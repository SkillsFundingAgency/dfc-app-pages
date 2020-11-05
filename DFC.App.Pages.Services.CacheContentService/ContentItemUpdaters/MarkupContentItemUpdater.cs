using DFC.App.Pages.Data.Models;
using DFC.Content.Pkg.Netcore.Data.Contracts;
using System;
using System.Threading.Tasks;

namespace DFC.App.Pages.Data.Contracts
{
    public class MarkupContentItemUpdater<T> : IMarkupContentItemUpdater<T>
        where T : class, IBaseContentItemModel, ICmsApiMarkupContentItem
    {
        private readonly ICmsApiService cmsApiService;

        public MarkupContentItemUpdater(ICmsApiService cmsApiService)
        {
            this.cmsApiService = cmsApiService;
        }

        public async Task<bool> FindAndUpdateAsync(ContentItemModel contentItemModel, Uri url)
        {
            _ = contentItemModel ?? throw new ArgumentNullException(nameof(contentItemModel));

            var cmsApiHtmlModel = await cmsApiService.GetContentItemAsync<T>(url).ConfigureAwait(false);

            if (cmsApiHtmlModel != null)
            {
                contentItemModel.Title = cmsApiHtmlModel.Title;
                contentItemModel.Content = cmsApiHtmlModel.Content;
                contentItemModel.HtmlBody = cmsApiHtmlModel.HtmlBody;
                contentItemModel.LastCached = DateTime.UtcNow;

                return true;
            }

            return false;
        }
    }
}

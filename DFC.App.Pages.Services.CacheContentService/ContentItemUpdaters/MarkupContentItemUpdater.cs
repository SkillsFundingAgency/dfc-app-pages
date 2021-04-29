using DFC.App.Pages.Data.Common;
using DFC.App.Pages.Data.Contracts;
using DFC.App.Pages.Data.Models;
using DFC.App.Pages.Data.Models.CmsApiModels;
using DFC.Content.Pkg.Netcore.Data.Contracts;
using System;
using System.Threading.Tasks;

namespace DFC.App.Pages.Services.CacheContentService.ContentItemUpdaters
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

            var cmsApiModel = await cmsApiService.GetContentItemAsync<T>(url).ConfigureAwait(false);

            if (cmsApiModel != null)
            {
                switch (contentItemModel.ContentType)
                {
                    case Constants.ContentTypeHtml:
                        if (cmsApiModel is CmsApiHtmlModel cmsApiHtmlModel)
                        {
                            contentItemModel.Title = cmsApiHtmlModel.Title;
                            contentItemModel.Content = cmsApiHtmlModel.Content;
                            contentItemModel.HtmlBody = cmsApiHtmlModel.HtmlBody;
                        }

                        break;
                    case Constants.ContentTypeHtmlShared:
                        if (cmsApiModel is CmsApiHtmlSharedModel cmsApiHtmlSharedModel)
                        {
                            contentItemModel.Title = cmsApiHtmlSharedModel.Title;
                            contentItemModel.Content = cmsApiHtmlSharedModel.Content;
                            contentItemModel.HtmlBody = cmsApiHtmlSharedModel.HtmlBody;
                        }

                        break;
                    case Constants.ContentTypeSharedContent:
                        if (cmsApiModel is CmsApiSharedContentModel cmsApiSharedContentModel)
                        {
                            contentItemModel.Title = cmsApiSharedContentModel.Title;
                            contentItemModel.Content = cmsApiSharedContentModel.Content;
                            contentItemModel.HtmlBody = cmsApiSharedContentModel.HtmlBody;
                        }

                        break;
                    case Constants.ContentTypeForm:
                        if (cmsApiModel is CmsApiFormModel cmsApiFormModel)
                        {
                            contentItemModel.Alignment = cmsApiFormModel.Alignment;
                            contentItemModel.Ordinal = cmsApiFormModel.Ordinal;
                            contentItemModel.Size = cmsApiFormModel.Size;
                        }

                        break;
                }

                contentItemModel.Alignment = cmsApiModel.Alignment;
                contentItemModel.Ordinal = cmsApiModel.Ordinal;
                contentItemModel.Size = cmsApiModel.Size;
                contentItemModel.LastCached = DateTime.UtcNow;

                return true;
            }

            return false;
        }
    }
}

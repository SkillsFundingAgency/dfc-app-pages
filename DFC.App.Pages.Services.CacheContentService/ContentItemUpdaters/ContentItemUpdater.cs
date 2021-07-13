using DFC.App.Pages.Data.Common;
using DFC.App.Pages.Data.Contracts;
using DFC.App.Pages.Data.Models;
using DFC.App.Pages.Data.Models.CmsApiModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DFC.App.Pages.Services.CacheContentService.ContentItemUpdaters
{
    public class ContentItemUpdater : IContentItemUpdater
    {
        private readonly IMarkupContentItemUpdater<CmsApiHtmlModel> htmlMarkupContentItemUpdater;
        private readonly IMarkupContentItemUpdater<CmsApiHtmlSharedModel> htmlSharedMarkupContentItemUpdater;
        private readonly IMarkupContentItemUpdater<CmsApiSharedContentModel> sharedContentMarkupContentItemUpdater;
        private readonly IMarkupContentItemUpdater<CmsApiFormModel> formMarkupContentItemUpdater;

        public ContentItemUpdater(
            IMarkupContentItemUpdater<CmsApiHtmlModel> htmlMarkupContentItemUpdater,
            IMarkupContentItemUpdater<CmsApiHtmlSharedModel> htmlSharedMarkupContentItemUpdater,
            IMarkupContentItemUpdater<CmsApiSharedContentModel> sharedContentMarkupContentItemUpdater,
            IMarkupContentItemUpdater<CmsApiFormModel> formMarkupContentItemUpdater)
        {
            this.htmlMarkupContentItemUpdater = htmlMarkupContentItemUpdater;
            this.htmlSharedMarkupContentItemUpdater = htmlSharedMarkupContentItemUpdater;
            this.sharedContentMarkupContentItemUpdater = sharedContentMarkupContentItemUpdater;
            this.formMarkupContentItemUpdater = formMarkupContentItemUpdater;
        }

        public async Task<bool> FindAndUpdateAsync(Uri url, Guid contentItemId, List<ContentItemModel>? contentItems)
        {
            var contentItemModel = FindItem(contentItemId, contentItems);

            if (contentItemModel != null)
            {
                switch (contentItemModel.ContentType)
                {
                    case Constants.ContentTypeHtml:
                        return await htmlMarkupContentItemUpdater.FindAndUpdateAsync(contentItemModel, url).ConfigureAwait(false);
                    case Constants.ContentTypeHtmlShared:
                        return await htmlSharedMarkupContentItemUpdater.FindAndUpdateAsync(contentItemModel, url).ConfigureAwait(false);
                    case Constants.ContentTypeSharedContent:
                        return await sharedContentMarkupContentItemUpdater.FindAndUpdateAsync(contentItemModel, url).ConfigureAwait(false);
                    case Constants.ContentTypeForm:
                        return await formMarkupContentItemUpdater.FindAndUpdateAsync(contentItemModel, url).ConfigureAwait(false);
                }
            }

            return false;
        }

        public ContentItemModel? FindItem(Guid itemId, List<ContentItemModel>? items)
        {
            if (items == null || !items.Any())
            {
                return default;
            }

            foreach (var contentItemModel in items)
            {
                if (contentItemModel.ItemId == itemId)
                {
                    return contentItemModel;
                }

                var childContentItemModel = FindItem(itemId, contentItemModel.ContentItems);

                if (childContentItemModel != null)
                {
                    return childContentItemModel;
                }
            }

            return default;
        }
    }
}

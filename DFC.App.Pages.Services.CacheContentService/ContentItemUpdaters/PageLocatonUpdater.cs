using DFC.App.Pages.Data.Contracts;
using DFC.App.Pages.Data.Models;
using DFC.App.Pages.Data.Models.CmsApiModels;
using DFC.Content.Pkg.Netcore.Data.Contracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DFC.App.Pages.Services.CacheContentService.ContentItemUpdaters
{
    public class PageLocatonUpdater : IPageLocatonUpdater
    {
        private readonly ICmsApiService cmsApiService;

        public PageLocatonUpdater(ICmsApiService cmsApiService)
        {
            this.cmsApiService = cmsApiService;
        }

        public async Task<bool> FindAndUpdateAsync(Uri url, Guid contentItemId, List<PageLocationModel>? pageLocations)
        {
            var pageLocationModel = FindItem(contentItemId, pageLocations);

            if (pageLocationModel != null)
            {
                var cmsApiPageLocationModel = await cmsApiService.GetContentItemAsync<CmsApiPageLocationModel>(url).ConfigureAwait(false);

                if (cmsApiPageLocationModel != null)
                {
                    pageLocationModel.BreadcrumbLinkSegment = cmsApiPageLocationModel.Title;
                    pageLocationModel.BreadcrumbText = cmsApiPageLocationModel.BreadcrumbText;
                    pageLocationModel.LastCached = DateTime.UtcNow;

                    return true;
                }
            }

            return false;
        }

        public PageLocationModel? FindItem(Guid itemId, List<PageLocationModel>? items)
        {
            if (items == null || !items.Any())
            {
                return default;
            }

            foreach (var pageLocationModel in items)
            {
                if (pageLocationModel.ItemId == itemId)
                {
                    return pageLocationModel;
                }

                var childContentItemModel = FindItem(itemId, pageLocationModel.PageLocations);

                if (childContentItemModel != null)
                {
                    return childContentItemModel;
                }
            }

            return default;
        }
    }
}

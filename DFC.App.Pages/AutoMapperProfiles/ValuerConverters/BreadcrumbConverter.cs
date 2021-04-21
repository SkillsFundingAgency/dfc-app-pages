using AutoMapper;
using DFC.App.Pages.Data.Models;
using DFC.App.Pages.ViewModels;
using System.Collections.Generic;
using System.Linq;

namespace DFC.App.Pages.AutoMapperProfiles.ValuerConverters
{
    public class BreadcrumbConverter : IValueConverter<ContentPageModel?, List<BreadcrumbItemViewModel>?>
    {
        public List<BreadcrumbItemViewModel>? Convert(ContentPageModel? sourceMember, ResolutionContext context)
        {
            if (sourceMember?.PageLocations == null || !sourceMember.PageLocations.Any())
            {
                return null;
            }

            var result = new List<BreadcrumbItemViewModel>();

            foreach (var item in sourceMember.PageLocations)
            {
                result.AddRange(IterateChilderen(item));
            }

            result.Reverse();

            if (!sourceMember.IsDefaultForPageLocation && !string.IsNullOrWhiteSpace(sourceMember.MetaTags?.Title))
            {
                var articlePathViewModel = new BreadcrumbItemViewModel
                {
                    Route = $"{sourceMember.CanonicalName}",
                    Title = sourceMember.MetaTags?.Title,
                    AddHyperlink = false,
                };

                result.Add(articlePathViewModel);
            }

            string segmentRoute = string.Empty;

            foreach (var segment in result)
            {
                if (segment.Route != "/")
                {
                    segmentRoute += "/" + segment.Route;

                    segment.Route = segmentRoute;
                }
            }

            return result;
        }

        private static List<BreadcrumbItemViewModel> IterateChilderen(PageLocationModel pageLocationModel)
        {
            var result = new List<BreadcrumbItemViewModel>();

            if (!string.IsNullOrWhiteSpace(pageLocationModel.BreadcrumbText))
            {
                var breadcrumbItemModel = new BreadcrumbItemViewModel
                {
                    Title = pageLocationModel.BreadcrumbText,
                    Route = pageLocationModel.BreadcrumbLinkSegment,
                };

                result.Add(breadcrumbItemModel);
            }

            foreach (var sharedContentItemModel in pageLocationModel.PageLocations.Where(w => !string.IsNullOrWhiteSpace(w?.BreadcrumbText)))
            {
                if (sharedContentItemModel != null)
                {
                    result.AddRange(IterateChilderen(sharedContentItemModel));
                }
            }

            return result;
        }
    }
}

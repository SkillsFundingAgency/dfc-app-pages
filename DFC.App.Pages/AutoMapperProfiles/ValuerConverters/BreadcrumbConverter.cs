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
            if (sourceMember == null)
            {
                return null;
            }

            var pageLocations = sourceMember.ContentItems.Where(w => w.ContentType!.Equals("PageLocation", System.StringComparison.OrdinalIgnoreCase)).ToList();

            if (pageLocations == null || !pageLocations.Any())
            {
                return null;
            }

            var result = new List<BreadcrumbItemViewModel>();

            foreach (var item in pageLocations)
            {
                result.AddRange(IterateChilderen(item));
            }

            result.Reverse();

            var articlePathViewModel = new BreadcrumbItemViewModel
            {
                Route = $"{sourceMember.CanonicalName}",
                Title = sourceMember.MetaTags?.Title,
                AddHyperlink = false,
            };

            result.Add(articlePathViewModel);

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

        private static List<BreadcrumbItemViewModel> IterateChilderen(ContentItemModel item)
        {
            var result = new List<BreadcrumbItemViewModel>();

            if (!string.IsNullOrWhiteSpace(item.BreadcrumbText))
            {
                var breadcrumbItemModel = new BreadcrumbItemViewModel
                {
                    Title = item.BreadcrumbText,
                    Route = item.BreadcrumbLinkSegment,
                };

                result.Add(breadcrumbItemModel);
            }

            foreach (var sharedContentItemModel in item.ContentItems.Where(w => !string.IsNullOrWhiteSpace(w.BreadcrumbText)))
            {
                result.AddRange(IterateChilderen(sharedContentItemModel));
            }

            return result;
        }

        private static List<BreadcrumbItemViewModel> IterateChilderen(SharedContentItemModel item)
        {
            var result = new List<BreadcrumbItemViewModel>();

            if (!string.IsNullOrWhiteSpace(item.BreadcrumbText))
            {
                var breadcrumbItemModel = new BreadcrumbItemViewModel
                {
                    Title = item.BreadcrumbText,
                    Route = item.BreadcrumbLinkSegment,
                };

                result.Add(breadcrumbItemModel);
            }

            foreach (var sharedContentItemModel in item.ContentItems.Where(w => !string.IsNullOrWhiteSpace(w.BreadcrumbText)))
            {
                result.AddRange(IterateChilderen(sharedContentItemModel));
            }

            return result;
        }
    }
}

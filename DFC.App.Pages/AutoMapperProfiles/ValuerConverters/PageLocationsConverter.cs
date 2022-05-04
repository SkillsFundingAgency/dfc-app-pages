using AutoMapper;
using DFC.App.Pages.Data.Common;
using DFC.App.Pages.Data.Models;
using DFC.App.Pages.Data.Models.CmsApiModels;
using DFC.Content.Pkg.Netcore.Data.Contracts;
using System;
using System.Collections.Generic;
using System.Linq;

namespace DFC.App.Pages.AutoMapperProfiles.ValuerConverters
{
    public class PageLocationsConverter : IValueConverter<IList<IBaseContentItemModel>?, List<PageLocationModel>?>
    {
        public List<PageLocationModel>? Convert(IList<IBaseContentItemModel>? sourceMember, ResolutionContext context)
        {
            _ = context ?? throw new ArgumentNullException(nameof(context));

            if (sourceMember == null || !sourceMember.Any())
            {
                return default;
            }

            var result = new List<PageLocationModel>();

            foreach (var item in sourceMember)
            {
                switch (item.ContentType.ToLower())
                {
                    case Constants.ContentTypePageLocation:
                        if (item is CmsApiPageLocationModel cmsApiPageLocationModel)
                        {
                            result.Add(context.Mapper.Map<PageLocationModel>(cmsApiPageLocationModel));
                        }

                        break;
                }
            }

            return result;
        }
    }
}

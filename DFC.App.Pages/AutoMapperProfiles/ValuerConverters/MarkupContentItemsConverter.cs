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
    public class MarkupContentItemsConverter : IValueConverter<IList<IBaseContentItemModel>?, List<ContentItemModel>?>
    {
        public List<ContentItemModel>? Convert(IList<IBaseContentItemModel>? sourceMember, ResolutionContext context)
        {
            _ = context ?? throw new ArgumentNullException(nameof(context));

            if (sourceMember == null || !sourceMember.Any())
            {
                return default;
            }

            var result = new List<ContentItemModel>();

            foreach (var item in sourceMember)
            {
                switch (item.ContentType)
                {
                    case Constants.ContentTypeHtml:
                        if (item is CmsApiHtmlModel cmsApiHtmlModel)
                        {
                            result.Add(context.Mapper.Map<ContentItemModel>(cmsApiHtmlModel));
                        }

                        break;
                    case Constants.ContentTypeHtmlShared:
                        if (item is CmsApiHtmlSharedModel cmsApiHtmlSharedModel)
                        {
                            result.Add(context.Mapper.Map<ContentItemModel>(cmsApiHtmlSharedModel));
                        }

                        break;
                    case Constants.ContentTypeSharedContent:
                        if (item is CmsApiSharedContentModel cmsApiSharedContentModel)
                        {
                            result.Add(context.Mapper.Map<ContentItemModel>(cmsApiSharedContentModel));
                        }

                        break;
                }
            }

            return result;
        }
    }
}

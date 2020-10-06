using AutoMapper;
using DFC.App.Pages.Data.Models.CmsApiModels;
using System;

namespace DFC.App.Pages.AutoMapperProfiles.ValuerConverters
{
    public class CanonicalNameConverter : IValueConverter<CmsApiDataModel?, string?>
    {
        public string? Convert(CmsApiDataModel? sourceMember, ResolutionContext context)
        {
            const string delimiter = "/";

            if (sourceMember == null)
            {
                return null;
            }

            if (!string.IsNullOrWhiteSpace(sourceMember.CanonicalName))
            {
                return sourceMember.CanonicalName;
            }

            string? result = null;

            if (!string.IsNullOrWhiteSpace(sourceMember.PageLocation))
            {
                int fromPosition = sourceMember.PageLocation.LastIndexOf(delimiter, StringComparison.OrdinalIgnoreCase);

                result = fromPosition != -1 ? sourceMember.PageLocation.Substring(fromPosition + 1) : null;
            }

            return result;
        }
    }
}

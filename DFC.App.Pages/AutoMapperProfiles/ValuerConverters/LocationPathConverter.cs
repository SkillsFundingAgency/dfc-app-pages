using AutoMapper;
using DFC.App.Pages.Data.Models;
using System;

namespace DFC.App.Pages.AutoMapperProfiles.ValuerConverters
{
    public class LocationPathConverter : IValueConverter<ContentPageModel?, string?>
    {
        public string? Convert(ContentPageModel? sourceMember, ResolutionContext context)
        {
            const string delimiter = "/";

            if (sourceMember == null)
            {
                return null;
            }

            string pageLocation;
            string? fullPath;

            if (string.IsNullOrWhiteSpace(sourceMember.PageLocation) || sourceMember.PageLocation == delimiter)
            {
                pageLocation = string.Empty;
            }
            else if (sourceMember.PageLocation.StartsWith(delimiter, StringComparison.Ordinal))
            {
                pageLocation = sourceMember.PageLocation.Substring(1);
            }
            else
            {
                pageLocation = sourceMember.PageLocation;
            }

            if (sourceMember.IsDefaultForPageLocation)
            {
                fullPath = pageLocation;
            }
            else if (string.IsNullOrWhiteSpace(pageLocation))
            {
                fullPath = sourceMember.CanonicalName;
            }
            else if (string.IsNullOrWhiteSpace(sourceMember.CanonicalName))
            {
                fullPath = $"{pageLocation}";
            }
            else
            {
                fullPath = $"{pageLocation}/{sourceMember.CanonicalName}";
            }

            return delimiter + fullPath;
        }
    }
}

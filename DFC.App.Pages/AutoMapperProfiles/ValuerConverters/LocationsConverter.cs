using AutoMapper;
using DFC.App.Pages.Data.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace DFC.App.Pages.AutoMapperProfiles.ValuerConverters
{
    public class LocationsConverter : IValueConverter<ContentPageModel?, IList<string>?>
    {
        public IList<string>? Convert(ContentPageModel? sourceMember, ResolutionContext context)
        {
            const string delimiter = "/";

            if (sourceMember == null)
            {
                return default;
            }

            var result = new List<string>();
            string pageLocation;

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
                result.Add($"{delimiter}{pageLocation}");
                result.Add($"{delimiter}{pageLocation}{delimiter}{sourceMember.CanonicalName}");
            }
            else if (string.IsNullOrWhiteSpace(pageLocation))
            {
                result.Add($"{delimiter}{sourceMember.CanonicalName}");
            }
            else if (string.IsNullOrWhiteSpace(sourceMember.CanonicalName))
            {
                result.Add($"{delimiter}{pageLocation}");
            }
            else
            {
                result.Add($"{delimiter}{pageLocation}{delimiter}{sourceMember.CanonicalName}");
            }

            if (sourceMember.RedirectLocations != null && sourceMember.RedirectLocations.Any())
            {
                result.AddRange(sourceMember.RedirectLocations);
            }

            return result;
        }
    }
}

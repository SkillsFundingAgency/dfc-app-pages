using AutoMapper;
using System;

namespace DFC.App.Pages.AutoMapperProfiles.ValuerConverters
{
    public class PageLocationConverter : IValueConverter<string?, string?>
    {
        public string? Convert(string? sourceMember, ResolutionContext context)
        {
            const string delimiter = "/";

            if (string.IsNullOrWhiteSpace(sourceMember))
            {
                return delimiter;
            }

            int uptoPosition = sourceMember.LastIndexOf(delimiter, StringComparison.OrdinalIgnoreCase);

            var result = uptoPosition > 0 ? sourceMember.Substring(0, uptoPosition) : delimiter;

            return result;
        }
    }
}

using AutoMapper;
using System.Collections.Generic;
using System.Linq;

namespace DFC.App.Pages.AutoMapperProfiles.ValuerConverters
{
    public class CleanStringListConverter : IValueConverter<List<string>?, List<string>?>
    {
        public List<string>? Convert(List<string>? sourceMember, ResolutionContext context)
        {
            if (sourceMember == null || !sourceMember.Any())
            {
                return default;
            }

            return sourceMember.Where(w => !string.IsNullOrWhiteSpace(w)).ToList();
        }
    }
}

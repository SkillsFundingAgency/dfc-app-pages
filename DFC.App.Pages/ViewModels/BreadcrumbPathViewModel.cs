using Newtonsoft.Json;
using System.Diagnostics.CodeAnalysis;

namespace DFC.App.Pages.ViewModels
{
    [ExcludeFromCodeCoverage]
    public class BreadcrumbPathViewModel
    {
        public string? Route { get; set; }

        public string? Title { get; set; }

        [JsonIgnore]
        public bool AddHyperlink { get; set; } = true;
    }
}

using Newtonsoft.Json;

namespace DFC.App.Pages.ViewModels
{
    public class BreadcrumbPathViewModel
    {
        public string? Route { get; set; }

        public string? Title { get; set; }

        [JsonIgnore]
        public bool AddHyperlink { get; set; } = true;
    }
}

using DFC.App.Pages.Data.Contracts;
using dfc_content_pkg_netcore.contracts;
using Newtonsoft.Json;
using System;

namespace DFC.App.Pages.Data.Models
{
    public class PagesSummaryItemModel : IBaseApiDataModel
    {
        [JsonProperty(PropertyName = "uri")]
        public Uri? Url { get; set; }

        [JsonProperty(PropertyName = "skos__prefLabel")]
        public string? Title { get; set; }

        public DateTime? CreatedDate { get; set; }

        [JsonProperty(PropertyName = "ModifiedDate")]
        public DateTime Published { get; set; }
    }
}

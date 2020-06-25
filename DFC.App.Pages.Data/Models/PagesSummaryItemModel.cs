using DFC.App.Pages.Data.Contracts;
using Newtonsoft.Json;
using System;

namespace DFC.App.Pages.Data.Models
{
    public class PagesSummaryItemModel : IApiDataModel
    {
        [JsonProperty(PropertyName = "uri")]
        public Uri? Url { get; set; }

        [JsonProperty(PropertyName = "skos__prefLabel")]
        public string? Title { get; set; }

        [JsonProperty(PropertyName = "alias_alias")]
        public string? CanonicalName { get; set; }

        public DateTime CreatedDate { get; set; }

        [JsonProperty(PropertyName = "ModifiedDate")]
        public DateTime Published { get; set; }
    }
}

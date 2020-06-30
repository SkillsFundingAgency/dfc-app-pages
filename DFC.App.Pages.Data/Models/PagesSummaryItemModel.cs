using DFC.App.Pages.Data.Contracts;
using System;
using Newtonsoft.Json;

namespace DFC.App.Pages.Data.Models
{
    public class PagesSummaryItemModel : IApiDataModel
    {
        public Uri? Url { get; set; }

        [JsonProperty("skos__prefLabel")]
        public string? Title { get; set; }

        public DateTime? CreatedDate { get; set; }

        public DateTime ModifiedDate { get; set; }
    }
}

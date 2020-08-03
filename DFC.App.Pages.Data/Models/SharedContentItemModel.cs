using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace DFC.App.Pages.Data.Models
{
    public class SharedContentItemModel
    {
        public Uri? Url { get; set; }

        public Guid? ItemId { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string? Title { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string? Content { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string? BreadcrumbText { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string? BreadcrumbLinkSegment { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public DateTime? CreatedDate { get; set; }

        public DateTime LastReviewed { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string? ContentType { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public IList<SharedContentItemModel>? ContentItems { get; set; }
    }
}

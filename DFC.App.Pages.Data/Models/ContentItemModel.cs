using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace DFC.App.Pages.Data.Models
{
    public class ContentItemModel
    {
        public Uri? Url { get; set; }

        public Guid? ItemId { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string? Title { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string? BreadcrumbText { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string? BreadcrumbLinkSegment { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string? Content { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string? Alignment { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public int? Ordinal { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public int? Size { get; set; }

        public DateTime LastReviewed { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public DateTime? CreatedDate { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string? ContentType { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string? HtmlBody { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public List<ContentItemModel> ContentItems { get; set; } = new List<ContentItemModel>();
    }
}

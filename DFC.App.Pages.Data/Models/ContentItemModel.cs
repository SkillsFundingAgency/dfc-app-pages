using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace DFC.App.Pages.Data.Models
{
    [ExcludeFromCodeCoverage]
    public class ContentItemModel
    {
        public Uri? Url { get; set; }

        public Guid? ItemId { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string? Title { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string? Content { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string? HtmlBody { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string? Alignment { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public int? Ordinal { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public int? Size { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string? Action { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public bool EnableAntiForgeryToken { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string? Method { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string? EncType { get; set; }

        public DateTime LastCached { get; set; } = DateTime.UtcNow;

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public DateTime? CreatedDate { get; set; }

        public DateTime LastReviewed { get; set; }

        public string? ContentType { get; set; }

        public List<ContentItemModel>? ContentItems { get; set; } = new List<ContentItemModel>();
    }
}

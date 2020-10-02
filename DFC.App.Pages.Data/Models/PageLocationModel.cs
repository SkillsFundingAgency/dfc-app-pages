using DFC.App.Pages.Data.Contracts;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace DFC.App.Pages.Data.Models
{
    [ExcludeFromCodeCoverage]
    public class PageLocationModel
    {
        public Uri? Url { get; set; }

        public Guid? ItemId { get; set; }

        public string? BreadcrumbText { get; set; }

        public string? BreadcrumbLinkSegment { get; set; }

        public DateTime LastCached { get; set; } = DateTime.UtcNow;

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public DateTime? CreatedDate { get; set; }

        public DateTime LastReviewed { get; set; }

        public string? ContentType { get; set; }

        public List<PageLocationModel>? PageLocations { get; set; } = new List<PageLocationModel>();
    }
}

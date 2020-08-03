using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace DFC.App.Pages.Data.Models
{
    public class SharedContentItemModel
    {
        public Uri? Url { get; set; }

        public Guid? ItemId { get; set; }

        public string? Title { get; set; }

        public string? Content { get; set; }

        public string? BreadcrumbText { get; set; }

        public string? BreadcrumbLinkSegment { get; set; }

        public DateTime? CreatedDate { get; set; }

        public DateTime LastReviewed { get; set; }

        public string? ContentType { get; set; }

        public IList<SharedContentItemModel>? ContentItems { get; set; }
    }
}

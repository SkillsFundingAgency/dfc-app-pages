using System;
using System.Collections.Generic;

namespace DFC.App.Pages.Data.Models
{
    public class SharedContentItemModel
    {
        public Guid? ItemId { get; set; }

        public Uri? Url { get; set; }

        public Guid? Version { get; set; }

        public string? Content { get; set; }

        public string? BreadcrumbText { get; set; }

        public string? BreadcrumbLinkSegment { get; set; }

        public DateTime? CreatedDate { get; set; }

        public DateTime LastReviewed { get; set; }

        public IList<SharedContentItemModel>? ContentItems { get; set; }
    }
}

using DFC.App.Pages.Data.Contracts;
using System;
using System.Collections.Generic;

namespace DFC.App.Pages.Data.Models
{
    public class PagesApiDataModel : IApiDataModel
    {
        public Guid? ItemId { get; set; }

        public string? CanonicalName { get; set; }

        public Guid? Version { get; set; }

        public string? BreadcrumbTitle { get; set; }

        public bool IncludeInSitemap { get; set; }

        public Uri? Url { get; set; }

        public IList<string>? AlternativeNames { get; set; }

        public string? Title { get; set; }

        public string? Description { get; set; }

        public string? Keywords { get; set; }

        public IList<Uri>? ContentItemUrls { get; set; }

        public IList<PagesApiContentItemModel> ContentItems { get; set; } = new List<PagesApiContentItemModel>();

        public DateTime? Published { get; set; }
    }
}

using DFC.Compui.Cosmos.Enums;
using Microsoft.AspNetCore.Html;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;

namespace DFC.App.Pages.ViewModels
{
    [ExcludeFromCodeCoverage]
    public class DocumentViewModel
    {
        public HtmlHeadViewModel? HtmlHead { get; set; }

        public BreadcrumbViewModel? Breadcrumb { get; set; }

        [Display(Name = "Document Id")]
        public Guid? DocumentId { get; set; }

        [Display(Name = "Canonical Name")]
        public string? CanonicalName { get; set; }

        [Display(Name = "PartitionKey")]
        public string? PartitionKey { get; set; }

        [Required]
        public Guid? Version { get; set; }

        [Display(Name = "Is Default For Page Location")]
        public bool IsDefaultForPageLocation { get; set; }

        [Display(Name = "Page Location")]
        public string? PageLocation { get; set; }

        [Display(Name = "Use Browser Width")]
        public bool UseBrowserWidth { get; set; }

        [Display(Name = "Show Breadcrumb")]
        public bool ShowBreadcrumb { get; set; }

        [Display(Name = "Include In SiteMap")]
        public bool IncludeInSitemap { get; set; }

        [Display(Name = "SiteMap Priority")]
        public decimal SiteMapPriority { get; set; }

        [Display(Name = "SiteMap Change Frequency")]
        public SiteMapChangeFrequency SiteMapChangeFrequency { get; set; }

        public Uri? Url { get; set; }

        public HtmlString? Content { get; set; }

        [Display(Name = "Last Reviewed")]
        [DisplayFormat(DataFormatString = "{0:dd MMM yyyy HH:mm:ss}")]
        public DateTime LastReviewed { get; set; }

        [Display(Name = "Created Date")]
        [DisplayFormat(DataFormatString = "{0:dd MMM yyyy HH:mm:ss}")]
        public DateTime CreatedDate { get; set; }

        [Display(Name = "Last Cached")]
        [DisplayFormat(DataFormatString = "{0:dd MMM yyyy HH:mm:ss}")]
        public DateTime LastCached { get; set; }

        public IList<string>? Redirects { get; set; }

        public HeroBannerViewModel? HeroBannerViewModel { get; set; }

        public BodyViewModel? BodyViewModel { get; set; }
    }
}

using DFC.App.Pages.Data.Enums;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace DFC.App.Pages.Data.Models
{
    public class ContentPageModel : Compui.Cosmos.Models.ContentPageModel
    {
        [Required]
        [JsonProperty(Order = -10)]
        public override string? PartitionKey => PageLocation;

        public override string? PageLocation { get; set; } = "/missing-location";

        public new string? Content { get; set; }

        public IList<ContentItemModel>? ContentItems { get; set; }

        [Display(Name = "Breadcrumb Title")]
        public new string? BreadcrumbTitle { get; set; }

        [JsonProperty(Order = -10)]
        public new Guid? Version { get; set; }

        [Display(Name = "SiteMap Priority")]
        public decimal SiteMapPriority { get; set; }

        [Display(Name = "SiteMap Change Frequency")]
        public ChangeFrequency SiteMapChangeFrequency { get; set; }
    }
}

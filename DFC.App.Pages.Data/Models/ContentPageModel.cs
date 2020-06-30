using System;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace DFC.App.Pages.Data.Models
{
    public class ContentPageModel : DFC.Compui.Cosmos.Models.ContentPageModel
    {
        [Required]
        [JsonProperty(Order = -10)]
        public override string PartitionKey => "static-page";

        public new string? Content { get; set; }

        public IList<ContentItemModel>? ContentItems { get; set; }

        [Display(Name = "Breadcrumb Title")]
        public string BreadcrumbTitle { get; set; }

        [JsonProperty(Order = -10)]
        public Guid? Version { get; set; }

        public List<string> RedirectLocations { get; set; }
    }
}

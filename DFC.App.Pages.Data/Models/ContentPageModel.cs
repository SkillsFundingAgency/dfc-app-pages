using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace DFC.App.Pages.Data.Models
{
    public class ContentPageModel : Compui.Cosmos.Models.ContentPageModel
    {
        [Required]
        public override string? PartitionKey => PageLocation;

        public override string? PageLocation { get; set; } = "/missing-location";

        public new Guid? Version { get; set; }

        public new string? Content { get; set; }

        public IList<ContentItemModel>? ContentItems { get; set; }
    }
}

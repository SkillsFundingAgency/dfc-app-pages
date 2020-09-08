using DFC.App.Pages.Data.Extensions;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace DFC.App.Pages.Data.Models
{
    public class ContentPageModel : Compui.Cosmos.Models.ContentPageModel
    {
        [Required]
        public override string? PartitionKey => PageLocation;

        public override string? PageLocation { get; set; } = "/missing-location";

        public DateTime LastCached { get; set; } = DateTime.UtcNow;

        public new Guid? Version { get; set; }

        public new string? Content { get; set; }

        public string? HeroBanner { get; set; }

        public List<ContentItemModel> ContentItems { get; set; } = new List<ContentItemModel>();

        [JsonIgnore]
        public List<Guid> AllContentItemIds => ContentItems.Flatten(s => s.ContentItems).Where(w => w.ItemId != null).Select(s => s.ItemId!.Value).ToList();
    }
}

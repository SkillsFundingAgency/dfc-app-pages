using DFC.App.Pages.Data.Extensions;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;
using System.Linq;

namespace DFC.App.Pages.Data.Models
{
    [ExcludeFromCodeCoverage]
    public class ContentPageModel : Compui.Cosmos.Models.ContentPageModel
    {
        [Required]
        public override string? PartitionKey => PageLocation;

        public override string? PageLocation { get; set; } = "/missing-location";

        public DateTime LastCached { get; set; } = DateTime.UtcNow;

        public DateTime? CreatedDate { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public new Guid? Version { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public new string? Content { get; set; }

        public string? HeroBanner { get; set; }

        public List<ContentItemModel>? ContentItems { get; set; } = new List<ContentItemModel>();

        public List<PageLocationModel>? PageLocations { get; set; } = new List<PageLocationModel>();

        [JsonIgnore]
        public List<Guid> AllContentItemIds
        {
            get
            {
                var result = new List<Guid>();

                if (ContentItems != null)
                {
                    result.AddRange(ContentItems.Flatten(s => s.ContentItems).Where(w => w.ItemId != null).Select(s => s.ItemId!.Value));
                }

                if (PageLocations != null)
                {
                    result.AddRange(PageLocations.Flatten(s => s.PageLocations).Where(w => w.ItemId != null).Select(s => s.ItemId!.Value));
                }

                return result;
            }
        }
    }
}

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
    }
}

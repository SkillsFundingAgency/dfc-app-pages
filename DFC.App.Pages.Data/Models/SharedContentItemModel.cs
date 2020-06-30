using System;

namespace DFC.App.Pages.Data.Models
{
    public class SharedContentItemModel
    {
        public Guid? ItemId { get; set; }

        public Uri? Url { get; set; }

        public Guid? Version { get; set; }

        public string? Content { get; set; }

        public DateTime? CreatedDate { get; set; }

        public DateTime LastReviewed { get; set; }
    }
}

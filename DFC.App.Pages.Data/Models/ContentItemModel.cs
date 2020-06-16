using System;

namespace DFC.App.Pages.Data.Models
{
    public class ContentItemModel
    {
        public Uri? Url { get; set; }

        public Guid? ItemId { get; set; }

        public string? DisplayText { get; set; }

        public Guid Version { get; set; }

        public string? Content { get; set; }

        public int Justify { get; set; }

        public int Ordinal { get; set; }

        public int Width { get; set; }

        public DateTime? LastReviewed { get; set; }
    }
}

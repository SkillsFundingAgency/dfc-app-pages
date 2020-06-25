using System;

namespace DFC.App.Pages.Data.Models
{
    public class PagesApiContentItemModel
    {
        public Uri? Url { get; set; }

        public Guid? ItemId { get; set; }

        public string? DisplayText { get; set; }

        public Guid Version { get; set; }

        public string? Content { get; set; }

        public string? Alignment { get; set; }

        public int Ordinal { get; set; }

        public int Size { get; set; }

        public DateTime? Published { get; set; }
    }
}

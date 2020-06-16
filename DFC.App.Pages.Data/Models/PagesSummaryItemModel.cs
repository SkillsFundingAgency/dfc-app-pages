using DFC.App.Pages.Data.Contracts;
using System;

namespace DFC.App.Pages.Data.Models
{
    public class PagesSummaryItemModel : IApiDataModel
    {
        public Uri? Url { get; set; }

        public string? CanonicalName { get; set; }

        public DateTime Published { get; set; }
    }
}

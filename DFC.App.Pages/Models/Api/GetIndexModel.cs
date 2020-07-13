using System;
using System.Collections.Generic;

namespace DFC.App.Pages.Models.Api
{
    public class GetIndexModel
    {
        public string? CanonicalName { get; set; }

        public Uri? Url { get; set; }

        public IList<string>? RedirectLocations { get; set; }
    }
}

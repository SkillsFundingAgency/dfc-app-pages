using System;
using Newtonsoft.Json;

namespace DFC.App.Pages.Data.Models
{
    public class LinkDetails
    {
        [JsonIgnore]
        public Uri Uri { get; set; }

        public string Href { get; set; }

        public string Title { get; set; }

        public string ContentType { get; set; }

        public string Alignment { get; set; }

        public int Ordinal { get; set; }

        public string Size { get; set; }
    }
}

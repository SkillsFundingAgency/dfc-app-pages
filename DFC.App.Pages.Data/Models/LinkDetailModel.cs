using Newtonsoft.Json;
using System;

namespace DFC.App.Pages.Data.Models
{
    public class LinkDetailModel
    {
        [JsonIgnore]
        public Uri? Uri { get; set; }

        public string? Href { get; set; }

        public string? ContentType { get; set; }

        public string? Alignment { get; set; }

        public int Ordinal { get; set; }

        public int Size { get; set; }

        public string? Title { get; set; }

 //       public Type? LinkedType { get; set; } = typeof(PagesApiContentItemModel);
    }
}

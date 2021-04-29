using DFC.App.Pages.Data.Contracts;
using DFC.Content.Pkg.Netcore.Data.Models;
using Newtonsoft.Json;
using System.Diagnostics.CodeAnalysis;

namespace DFC.App.Pages.Data.Models.CmsApiModels
{
    [ExcludeFromCodeCoverage]
    public class CmsApiHtmlModel : BaseContentItemModel, ICmsApiMarkupContentItem
    {
        public string? Alignment { get; set; }

        public int? Ordinal { get; set; }

        public int? Size { get; set; }

        public string? Href { get; set; }

        public string? Content { get; set; }

        [JsonProperty("htmlbody_Html")]
        public string? HtmlBody { get; set; }
    }
}

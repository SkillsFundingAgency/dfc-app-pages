using DFC.Content.Pkg.Netcore.Data.Models;
using Newtonsoft.Json;
using System.Diagnostics.CodeAnalysis;

namespace DFC.App.Pages.Data.Models.CmsApiModels
{
    [ExcludeFromCodeCoverage]
    public class CmsApiFormModel : BaseContentItemModel
    {
        [JsonProperty("form_Action")]
        public string? Action { get; set; }

        [JsonProperty("form_EnableAntiForgeryToken")]
        public bool EnableAntiForgeryToken { get; set; }

        [JsonProperty("form_Method")]
        public string? Method { get; set; }

        [JsonProperty("form_EncType")]
        public string? EncType { get; set; }

        public string? Alignment { get; set; }

        public int? Ordinal { get; set; }

        public int? Size { get; set; }
    }
}

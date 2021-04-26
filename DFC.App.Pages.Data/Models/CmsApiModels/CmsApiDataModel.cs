using DFC.App.Pages.Data.JsonConveerters;
using DFC.Compui.Cosmos.Enums;
using DFC.Content.Pkg.Netcore.Data.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace DFC.App.Pages.Data.Models.CmsApiModels
{
    [ExcludeFromCodeCoverage]
    public class CmsApiDataModel : BaseContentItemModel
    {
        [JsonProperty("pagelocation_UrlName")]
        public string? CanonicalName { get; set; }

        [JsonProperty("pagelocation_DefaultPageForLocation")]
        public bool IsDefaultForPageLocation { get; set; }

        [JsonProperty("pagelocation_FullUrl")]
        public string? PageLocation { get; set; }

        [JsonProperty("taxonomy_terms")]
        public List<string> TaxonomyTerms { get; set; } = new List<string>();

        public Guid? Version { get; set; }

        [JsonProperty("sitemap_Exclude")]
        public bool ExcludeFromSitemap { get; set; }

        [JsonIgnore]
        public bool IncludeInSitemap => !ExcludeFromSitemap;

        public string? Description { get; set; }

        public string? Keywords { get; set; }

        public bool ShowBreadcrumb { get; set; } = true;

        public bool ShowHeroBanner { get; set; }

        public string? Herobanner { get; set; }

        [JsonProperty("sitemap_Priority")]
        public decimal SiteMapPriority { get; set; }

        [JsonProperty("sitemap_ChangeFrequency")]
        public SiteMapChangeFrequency SiteMapChangeFrequency { get; set; }

        [JsonProperty("pagelocation_RedirectLocations")]
        [JsonConverter(typeof(SingleOrArrayConverter<string>))]
        public List<string> RedirectLocations { get; set; } = new List<string>();
    }
}

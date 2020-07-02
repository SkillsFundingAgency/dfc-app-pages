using DFC.App.Pages.Data.Contracts;
using DFC.App.Pages.Data.Enums;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;

namespace DFC.App.Pages.Data.Models
{
    public class PagesApiDataModel : IApiDataModel
    {
        [JsonProperty("id")]
        public Guid? ItemId { get; set; }

        [JsonProperty("alias_alias")]
        public string? CanonicalName { get; set; }

        public string? Pagelocation { get; set; }

        public Guid? Version { get; set; }

        public string? BreadcrumbTitle { get; set; }

        [JsonProperty("sitemap_Exclude")]
        public bool ExcludeFromSitemap { get; set; }

        [JsonIgnore]
        public bool IncludeInSitemap => !ExcludeFromSitemap;

        [JsonProperty(PropertyName = "uri")]
        public Uri? Url { get; set; }

        [JsonProperty("skos__prefLabel")]
        public string? Title { get; set; }

        public string? Description { get; set; }

        public string? Keywords { get; set; }

        [JsonProperty("_links")]
        public JObject? Links { get; set; }

        [JsonIgnore]
        public ContentLinksModel? ContentLinks
        {
            get => PrivateLinksModel ??= new ContentLinksModel(Links);

            set => PrivateLinksModel = value;
        }

        public IList<PagesApiContentItemModel> ContentItems { get; set; } = new List<PagesApiContentItemModel>();

        [JsonProperty(PropertyName = "ModifiedDate")]
        public DateTime Published { get; set; }

        public DateTime? CreatedDate { get; set; }

        [JsonProperty("sitemap_Priority")]
        public decimal SiteMapPriority { get; set; }

        [JsonProperty("sitemap_ChangeFrequency")]
        public ChangeFrequency SiteMapChangeFrequency { get; set; }

        public string RedirectLocations { get; set; } = string.Empty;

        private ContentLinksModel? PrivateLinksModel { get; set; }

        public List<string> Redirects()
        {
            return string.IsNullOrEmpty(RedirectLocations) ? new List<string>() : RedirectLocations.Split("\r\n").ToList();
        }
    }
}

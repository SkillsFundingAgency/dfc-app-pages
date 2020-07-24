using DFC.App.Pages.Data.Contracts;
using DFC.App.Pages.Data.JsonConveerters;
using DFC.Compui.Cosmos.Enums;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;

namespace DFC.App.Pages.Data.Models
{
    //TODO: ian add these new properties
    /*
     *
         *@Ian Crisp, your updated property names for page locations should be as follows:
        15:06
        pagelocation_UrlName is the canonical name, auto generated from the page title of left blank
        15:07
   ^     pagelocation_FullUrl is the generated "alias", which is the page location hierarchy concatenated together with the UrlName and slugified
        15:08
   ^     pagelocation_DefaultPageForLocation is the default page flag
        15:08
   ^     pagelocation_RedirectLocations is the redirect URL's array
     */

    public class PagesApiDataModel : IApiDataModel
    {
        [JsonProperty("id")]
        public Guid? ItemId { get; set; }

        [JsonIgnore]
        [JsonProperty("pagelocation_UrlName")]
        public string? CanonicalName { get; set; }

        [JsonProperty("pagelocation_DefaultPageForLocation")]
        public bool IsDefaultForPageLocation { get; set; }

        [JsonProperty("pagelocation_FullUrl")]
        public string? PageLocation { get; set; }

        //[JsonProperty("taxonomy_terms")]
        //public List<string> TaxonomyTerms { get; set; } = new List<string>();

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
        public SiteMapChangeFrequency SiteMapChangeFrequency { get; set; }

        [JsonProperty("pagelocation_RedirectLocations")]
        [JsonConverter(typeof(SingleOrArrayConverter<string>))]
        public List<string> RedirectLocations { get; set; } = new List<string>();

        private ContentLinksModel? PrivateLinksModel { get; set; }
    }
}

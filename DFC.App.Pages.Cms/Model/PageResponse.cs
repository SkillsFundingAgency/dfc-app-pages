using Newtonsoft.Json;
using System.Text.Json.Serialization;

namespace DFC.App.Pages.Cms.Model
{
    public class Content
    {
        [JsonPropertyName("html")]
        public string Html { get; set; }
    }

    public class ContentItem
    {
        [JsonPropertyName("displayText")]
        public string DisplayText { get; set; }

        [JsonPropertyName("content")]
        public Content Content { get; set; }
    }

    public class PageResponse
    {
        [JsonPropertyName("page")]
        public List<Page> Page { get; set; }
    }

    public class Flow
    {
        [JsonPropertyName("widgets")]
        public List<Widget> Widgets { get; set; }
    }

    public class HtmlBody
    {
        [JsonPropertyName("html")]
        public string Html { get; set; }
    }

    public class Metadata
    {
        [JsonPropertyName("alignment")]
        public string Alignment { get; set; }

        [JsonPropertyName("size")]
        public int Size { get; set; }
    }

    public class Page
    {
        [JsonPropertyName("displayText")]
        public string DisplayText { get; set; }

        [JsonPropertyName("description")]
        public object Description { get; set; }

        [JsonPropertyName("pageLocation")]
        public PageLocation PageLocation { get; set; }

        [JsonPropertyName("breadcrumb")]
        public Breadcrumb Breadcrumb { get; set; }

        [JsonPropertyName("useBrowserWidth")]
        public bool? UseBrowserWidth { get; set; }

        [JsonPropertyName("showBreadcrumb")]
        public bool ShowBreadcrumb { get; set; }

        [JsonPropertyName("showHeroBanner")]
        public bool ShowHeroBanner { get; set; }

        [JsonPropertyName("herobanner")]
        public Herobanner Herobanner { get; set; }

        [JsonPropertyName("useInTriageTool")]
        public bool UseInTriageTool { get; set; }

        [JsonPropertyName("triageToolSummary")]
        public TriageToolSummary TriageToolSummary { get; set; }

        [JsonPropertyName("triageToolFilters")]
        public TriageToolFilters TriageToolFilters { get; set; }

        [JsonPropertyName("flow")]
        public Flow Flow { get; set; }
    }

    public class Herobanner
    {
        [JsonPropertyName("html")]
        public string Html { get; set; }
    }

    public class Breadcrumb
    {
        [JsonPropertyName("termContentItems")]
        public List<TermContentItem> TermContentItems { get; set; }
    }

    public class TermContentItem
    {
        [JsonPropertyName("displayText")]
        public string DisplayText { get; set; }

        [JsonPropertyName("modifiedUtc")]
        public DateTime ModifiedUtc { get; set; }

        [JsonPropertyName("breadcrumbText")]
        public string BreadcrumbText { get; set; }
    }
    public class PageLocation
    {
        [JsonPropertyName("urlName")]
        public string UrlName { get; set; }

        [JsonPropertyName("fullUrl")]
        public string FullUrl { get; set; }

        [JsonPropertyName("redirectLocations")]
        public string RedirectLocations { get; set; }

        [JsonPropertyName("defaultPageForLocation")]
        public bool DefaultPageForLocation { get; set; }
    }

    public class SharedContent
    {
        [JsonPropertyName("contentItems")]
        public List<ContentItem> ContentItems { get; set; }
    }

    public class TriageToolFilters
    {
        [JsonPropertyName("contentItems")]
        public List<object> ContentItems { get; set; }
    }

    public class TriageToolSummary
    {
        [JsonPropertyName("html")]
        public string Html { get; set; }
    }

    public class Widget
    {
        [JsonPropertyName("metadata")]
        public Metadata Metadata { get; set; }

        [JsonPropertyName("htmlBody")]
        public HtmlBody HtmlBody { get; set; }

        [JsonPropertyName("sharedContent")]
        public SharedContent SharedContent { get; set; }

        [JsonPropertyName("formContent")]
        public string FormContent { get; set; }

        [JsonProperty("formElement")]
        public FormElement FormElement { get; set; }

        [JsonProperty("form")]
        public Form Form { get; set; }

        [JsonProperty("flow")]
        public Flow Flow { get; set; }
    }

    public class Form
    {
        [JsonProperty("action")]
        public string Action { get; set; }

        [JsonProperty("method")]
        public string Method { get; set; }
    }

    public class FormElement
    {
        [JsonProperty("id")]
        public string Id { get; set; }
    }
}

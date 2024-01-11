using System.Text.Json.Serialization;

namespace DFC.App.Pages.Cms.Model
{
    //Content headers of the Breadcrumb response sent back from SQL query
    public class Item
    {
        [JsonPropertyName("content")]
        public string Content { get; set; }
    }

    public class BreadcrumbResponse
    {
        [JsonPropertyName("items")]
        public List<Item> Items { get; set; }
    }
}

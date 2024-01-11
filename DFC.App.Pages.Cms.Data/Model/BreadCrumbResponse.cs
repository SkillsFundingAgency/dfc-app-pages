using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace DFC.App.Pages.Cms.Data.Model
{
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

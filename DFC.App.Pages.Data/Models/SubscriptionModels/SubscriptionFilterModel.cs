using Microsoft.Azure.Management.EventGrid.Models;
using System.Collections.Generic;

namespace DFC.App.Pages.Data.Models.SubscriptionModels
{
    public class SubscriptionFilterModel
    {
        public string BeginsWith { get; set; } = "/content/page/";

        public string? EndsWith { get; set; }

        public List<string> IncludeEventTypes { get; set; } = new List<string> { "published", "unpublished", "deleted" };

        public StringInAdvancedFilter? PropertyContainsFilter { get; set; }
    }
}

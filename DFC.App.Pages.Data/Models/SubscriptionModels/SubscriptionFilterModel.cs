using Microsoft.Azure.Management.EventGrid.Models;
using System.Collections.Generic;

namespace DFC.App.Pages.Data.Models.SubscriptionModels
{
    public class SubscriptionFilterModel
    {
        public string? BeginsWith { get; set; }

        public string? EndsWith { get; set; }

        public List<string> IncludeEventTypes { get; set; } = new List<string>();

        public List<StringInAdvancedFilter> PropertyContainsFilters { get; set; } = new List<StringInAdvancedFilter>();
    }
}

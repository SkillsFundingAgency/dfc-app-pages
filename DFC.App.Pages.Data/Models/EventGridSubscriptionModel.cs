using System;

namespace DFC.App.Pages.Data.Models
{
    public class EventGridSubscriptionModel
    {
        public Guid? Id { get; set; }

        public string? ApplicationName { get; set; }

        public string? EventGridTopic { get; set; }

        public string? EventGridResourceGroup { get; set; }
    }
}

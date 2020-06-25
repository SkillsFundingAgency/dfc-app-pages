using DFC.App.Pages.Models;
using Microsoft.Azure.EventGrid.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net.Http;
using System.Net.Http.Formatting;

namespace DFC.App.Pages.IntegrationTests.ControllerTests
{
    public static class DataSeeding
    {
        public const string ExampleArticleName = "send-us-a-letter";
        public const string AlternativeArticleName = "alternative-name";

        private const string EventTypePublished = "published";
        private const string WebhookApiUrl = "api/webhook/ReceiveEvents";

        public static void SeedDefaultArticles(CustomWebApplicationFactory<DFC.App.Pages.Startup> factory)
        {
            var eventGridEventDataItems = new List<EventGridEventData>()
            {
                new EventGridEventData()
                {
                    ItemId = "3627EDA0-A5EF-405F-BD91-349FCAD91105",
                    DisplayText = "Send us a letter",
                },
                new EventGridEventData()
                {
                    ItemId = "46CB08FD-613E-4E72-8C08-39A8B256844E",
                    DisplayText = "Thank you for contacting us",
                },
                new EventGridEventData()
                {
                    ItemId = "EDFC8852-9820-4F29-B006-9FBD46CAB646",
                    DisplayText = "test-grid-4-x-3",
                },
            };

            var client = factory?.CreateClient();

            client!.DefaultRequestHeaders.Accept.Clear();

            foreach (var eventGridEventData in eventGridEventDataItems)
            {
                eventGridEventData.Api = "https://localhost:44354/home/item/contact-us/" + eventGridEventData.ItemId;
                var eventGridEvents = BuildValidEventGridEvent(EventTypePublished, eventGridEventData);
                var uri = new Uri("/" + WebhookApiUrl, UriKind.Relative);
                var result = client.PostAsync(uri, eventGridEvents, new JsonMediaTypeFormatter()).GetAwaiter().GetResult();
            }
        }

        private static EventGridEvent[] BuildValidEventGridEvent<TModel>(string eventType, TModel data)
        {
            var models = new EventGridEvent[]
            {
                new EventGridEvent
                {
                    Id = Guid.NewGuid().ToString(),
                    Subject = "pages/an-integration-test-name",
                    Data = data,
                    EventType = eventType,
                    EventTime = DateTime.Now,
                    DataVersion = "1.0",
                },
            };

            return models;
        }
    }
}

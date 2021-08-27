using DFC.App.Pages.Data.Models;
using Microsoft.Azure.EventGrid.Models;
using System;
using System.Net.Http;
using System.Net.Mime;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace DFC.App.Pages.IntegrationTests.ControllerTests.WebhooksControllerTests
{
    [Trait("Category", "Integration")]
    public class WebhooksControllerRouteTests : IClassFixture<CustomWebApplicationFactory<DFC.App.Pages.Startup>>
    {
        private const string EventTypePublished = "published";
        private const string WebhookApiUrl = "/api/webhook/ReceiveEvents";

        private readonly CustomWebApplicationFactory<Startup> factory;

        public WebhooksControllerRouteTests(CustomWebApplicationFactory<Startup> factory)
        {
            this.factory = factory;
        }

        [Fact]
        public async Task WebhooksControllerRouteTestsSubscriptionValidationReturnsSuccess()
        {
            // Arrange
            string expectedValidationCode = Guid.NewGuid().ToString();
            var eventGridEvents = BuildValidEventGridEvent(Microsoft.Azure.EventGrid.EventTypes.EventGridSubscriptionValidationEvent, new SubscriptionValidationEventData(expectedValidationCode, "https://somewhere.com"));
            var uri = new Uri(WebhookApiUrl, UriKind.Relative);
            var client = factory.CreateClientWithWebHostBuilder();
            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue(MediaTypeNames.Application.Json));

            // Act
            var response = await client.PostAsJsonAsync(uri, eventGridEvents).ConfigureAwait(false);

            // Assert
            response.EnsureSuccessStatusCode();
            Assert.Equal($"{MediaTypeNames.Application.Json}; charset={Encoding.UTF8.WebName}", response.Content.Headers.ContentType.ToString());
        }

        [Fact]
        public async Task WebhooksControllerRouteTestsPublishCreatePostReturnsSuccess()
        {
            // Arrange
            var eventGridEvents = BuildValidEventGridEvent(EventTypePublished, new EventGridEventData { ItemId = "edfc8852-9820-4f29-b006-9fbd46cab646", Api = "https://localhost:44354/home/item/contact-us/edfc8852-9820-4f29-b006-9fbd46cab646", });
            var uri = new Uri(WebhookApiUrl, UriKind.Relative);
            var client = factory.CreateClientWithWebHostBuilder();
            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue(MediaTypeNames.Application.Json));

            // Act
            var response = await client.PostAsJsonAsync(uri, eventGridEvents).ConfigureAwait(false);

            // Assert
            response.EnsureSuccessStatusCode();
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

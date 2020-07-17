using DFC.App.Pages.Data.Contracts;
using Microsoft.Azure.EventGrid;
using Microsoft.Azure.EventGrid.Models;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Threading.Tasks;

namespace DFC.App.Pages.Services.EventProcessorService
{
    [ExcludeFromCodeCoverage]
    public class EventGridClientService : IEventGridClientService
    {
        private readonly ILogger<EventGridClientService> logger;

        public EventGridClientService(ILogger<EventGridClientService> logger)
        {
            this.logger = logger;
        }

        public async Task SendEventAsync(List<EventGridEvent>? eventGridEvents, string? topicEndpoint, string? topicKey, string? logMessage)
        {
            _ = eventGridEvents ?? throw new ArgumentNullException(nameof(eventGridEvents));
            _ = topicEndpoint ?? throw new ArgumentNullException(nameof(topicEndpoint));
            _ = topicKey ?? throw new ArgumentNullException(nameof(topicKey));

            logger.LogInformation($"Sending Event Grid message for: {logMessage}");

            try
            {
                string topicHostname = new Uri(topicEndpoint).Host;
                var topicCredentials = new TopicCredentials(topicKey);
                using var client = new EventGridClient(topicCredentials);

                await client.PublishEventsAsync(topicHostname, eventGridEvents).ConfigureAwait(false);

                logger.LogInformation($"Sent Event Grid message for: {logMessage}");
            }
            catch (Exception ex)
            {
                logger.LogError(ex, $"Exception sending Event Grid message for: {logMessage}");
            }
        }
    }
}

using DFC.App.Pages.Data.Contracts;
using DFC.App.Pages.Data.Enums;
using DFC.App.Pages.Data.Models;
using DFC.App.Pages.Data.Models.ClientOptions;
using Microsoft.Azure.EventGrid;
using Microsoft.Azure.EventGrid.Models;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DFC.App.Pages.Services.EventProcessorService
{
    public class EventGridService : IEventGridService
    {
        private readonly ILogger<EventGridService> logger;
        private readonly EventGridPublishClientOptions eventGridPublishClientOptions;

        public EventGridService(ILogger<EventGridService> logger, EventGridPublishClientOptions eventGridPublishClientOptions)
        {
            this.logger = logger;
            this.eventGridPublishClientOptions = eventGridPublishClientOptions;
        }

        public async Task CompareAndSendEventAsync(ContentPageModel? existingContentPageModel, ContentPageModel updatedContentPageModel)
        {
            _ = updatedContentPageModel ?? throw new ArgumentNullException(nameof(updatedContentPageModel));

            logger.LogInformation($"Comparing differences to new: {updatedContentPageModel.Id} - {updatedContentPageModel.CanonicalName}");

            bool containsDifferences = existingContentPageModel == null;

            if (!containsDifferences)
            {
                containsDifferences |= !Equals(existingContentPageModel!.PageLocation, updatedContentPageModel.PageLocation);

                //TODO: ian: string array coparison required on following line
                containsDifferences |= !Equals(existingContentPageModel!.RedirectLocations, updatedContentPageModel.RedirectLocations);
            }

            if (containsDifferences)
            {
                await SendEventAsync(WebhookCacheOperation.CreateOrUpdate, updatedContentPageModel).ConfigureAwait(false);
            }
            else
            {
                logger.LogInformation($"No differences to create Event Grid message for: {updatedContentPageModel.Id} - {updatedContentPageModel.CanonicalName}");
            }
        }

        public async Task SendEventAsync(WebhookCacheOperation webhookCacheOperation, ContentPageModel updatedContentPageModel)
        {
            _ = updatedContentPageModel ?? throw new ArgumentNullException(nameof(updatedContentPageModel));

            logger.LogInformation($"Sending Event Grid message for: {webhookCacheOperation} - {updatedContentPageModel.Id} - {updatedContentPageModel.CanonicalName}");

            var eventGridEvents = new List<EventGridEvent>
            {
                new EventGridEvent
                {
                    Id = Guid.NewGuid().ToString(),
                    Subject = $"{eventGridPublishClientOptions.SubjectPrefix}/{updatedContentPageModel.Id}",
                    Data = new EventGridEventData
                    {
                        ItemId = updatedContentPageModel.Id.ToString(),
                        Api = updatedContentPageModel.Url!.ToString(),
                        DisplayText = updatedContentPageModel.CanonicalName,
                        VersionId = updatedContentPageModel.Version.ToString(),
                        Author = eventGridPublishClientOptions.SubjectPrefix,
                    },
                    EventType = webhookCacheOperation == WebhookCacheOperation.Delete ? "deleted" : "published",
                    EventTime = DateTime.Now,
                    DataVersion = "1.0",
                },
            };

            eventGridEvents.ForEach(f => f.Validate());

            try
            {
                string topicHostname = new Uri(eventGridPublishClientOptions.TopicEndpoint).Host;
                var topicCredentials = new TopicCredentials(eventGridPublishClientOptions.TopicKey);
                using var client = new EventGridClient(topicCredentials);

                await client.PublishEventsAsync(topicHostname, eventGridEvents).ConfigureAwait(false);

                logger.LogInformation($"Sent Event Grid message for: {webhookCacheOperation} - {updatedContentPageModel.Id} - {updatedContentPageModel.CanonicalName}");
            }
            catch (Exception ex)
            {
                logger.LogError(ex, $"Exception sending Event Grid message for: {webhookCacheOperation} - {updatedContentPageModel.Id} - {updatedContentPageModel.CanonicalName}");
            }
        }
    }
}

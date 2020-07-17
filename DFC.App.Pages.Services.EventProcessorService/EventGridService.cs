using DFC.App.Pages.Data.Contracts;
using DFC.App.Pages.Data.Enums;
using DFC.App.Pages.Data.Models;
using DFC.App.Pages.Data.Models.ClientOptions;
using Microsoft.Azure.EventGrid.Models;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;

namespace DFC.App.Pages.Services.EventProcessorService
{
    public class EventGridService : IEventGridService
    {
        private readonly ILogger<EventGridService> logger;
        private readonly IEventGridClientService eventGridClientService;
        private readonly EventGridPublishClientOptions eventGridPublishClientOptions;

        public EventGridService(ILogger<EventGridService> logger, IEventGridClientService eventGridClientService, EventGridPublishClientOptions eventGridPublishClientOptions)
        {
            this.logger = logger;
            this.eventGridClientService = eventGridClientService;
            this.eventGridPublishClientOptions = eventGridPublishClientOptions;
        }

        public async Task CompareAndSendEventAsync(ContentPageModel? existingContentPageModel, ContentPageModel? updatedContentPageModel)
        {
            _ = updatedContentPageModel ?? throw new ArgumentNullException(nameof(updatedContentPageModel));

            logger.LogInformation($"Comparing differences to new: {updatedContentPageModel.Id} - {updatedContentPageModel.CanonicalName}");

            bool containsDifferences = existingContentPageModel == null;

            if (!containsDifferences)
            {
                containsDifferences |= !Equals(existingContentPageModel.PageLocation, updatedContentPageModel.PageLocation);

                containsDifferences |= existingContentPageModel.RedirectLocations == null && updatedContentPageModel.RedirectLocations != null;
                containsDifferences |= existingContentPageModel.RedirectLocations != null && updatedContentPageModel.RedirectLocations == null;

                if (!containsDifferences && existingContentPageModel!.RedirectLocations != null && updatedContentPageModel.RedirectLocations != null)
                {
                    containsDifferences |= !Enumerable.SequenceEqual(existingContentPageModel.RedirectLocations, updatedContentPageModel.RedirectLocations);
                }
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

        public async Task SendEventAsync(WebhookCacheOperation webhookCacheOperation, ContentPageModel? updatedContentPageModel)
        {
            if (string.IsNullOrWhiteSpace(eventGridPublishClientOptions.TopicEndpoint))
            {
                throw new DataException($"EventGridPublishClientOptions is missing a value for: {nameof(eventGridPublishClientOptions.TopicEndpoint)}");
            }

            if (string.IsNullOrWhiteSpace(eventGridPublishClientOptions.TopicKey))
            {
                throw new DataException($"EventGridPublishClientOptions is missing a value for: {nameof(eventGridPublishClientOptions.TopicKey)}");
            }

            if (string.IsNullOrWhiteSpace(eventGridPublishClientOptions.SubjectPrefix))
            {
                throw new DataException($"EventGridPublishClientOptions is missing a value for: {nameof(eventGridPublishClientOptions.SubjectPrefix)}");
            }

            _ = updatedContentPageModel ?? throw new ArgumentNullException(nameof(updatedContentPageModel));

            var logMessage = $"{webhookCacheOperation} - {updatedContentPageModel.Id} - {updatedContentPageModel.CanonicalName}";
            logger.LogInformation($"Sending Event Grid message for: {logMessage}");

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

            await eventGridClientService.SendEventAsync(eventGridEvents, eventGridPublishClientOptions.TopicEndpoint, eventGridPublishClientOptions.TopicKey, logMessage).ConfigureAwait(false);
        }
    }
}

using DFC.App.Pages.Data.Contracts;
using DFC.App.Pages.Data.Enums;
using DFC.App.Pages.Data.Models;
using DFC.App.Pages.Data.Models.ClientOptions;
using Microsoft.Azure.EventGrid.Models;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
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

        public static bool ContainsDifferences(ContentPageModel? existingContentPageModel, ContentPageModel? updatedContentPageModel)
        {
            _ = updatedContentPageModel ?? throw new ArgumentNullException(nameof(updatedContentPageModel));

            if (existingContentPageModel == null)
            {
                return true;
            }

            if (!Equals(existingContentPageModel.PageLocation, updatedContentPageModel.PageLocation))
            {
                return true;
            }

            if ((existingContentPageModel.RedirectLocations == null && updatedContentPageModel.RedirectLocations != null) ||
                (existingContentPageModel.RedirectLocations != null && updatedContentPageModel.RedirectLocations == null))
            {
                return true;
            }

            if (!Enumerable.SequenceEqual(existingContentPageModel.RedirectLocations, updatedContentPageModel.RedirectLocations))
            {
                return true;
            }

            return false;
        }

        public static bool IsValidEventGridPublishClientOptions(ILogger<EventGridService> logger, EventGridPublishClientOptions? eventGridPublishClientOptions)
        {
            _ = eventGridPublishClientOptions ?? throw new ArgumentNullException(nameof(eventGridPublishClientOptions));

            if (string.IsNullOrWhiteSpace(eventGridPublishClientOptions.TopicEndpoint))
            {
                logger.LogWarning($"EventGridPublishClientOptions is missing a value for: {nameof(eventGridPublishClientOptions.TopicEndpoint)}");
                return false;
            }

            if (string.IsNullOrWhiteSpace(eventGridPublishClientOptions.TopicKey))
            {
                logger.LogWarning($"EventGridPublishClientOptions is missing a value for: {nameof(eventGridPublishClientOptions.TopicKey)}");
                return false;
            }

            if (string.IsNullOrWhiteSpace(eventGridPublishClientOptions.SubjectPrefix))
            {
                logger.LogWarning($"EventGridPublishClientOptions is missing a value for: {nameof(eventGridPublishClientOptions.SubjectPrefix)}");
                return false;
            }

            if (eventGridPublishClientOptions.ApiEndpoint == null)
            {
                logger.LogWarning($"EventGridPublishClientOptions is missing a value for: {nameof(eventGridPublishClientOptions.ApiEndpoint)}");
                return false;
            }

            return true;
        }

        public async Task CompareAndSendEventAsync(ContentPageModel? existingContentPageModel, ContentPageModel? updatedContentPageModel)
        {
            _ = updatedContentPageModel ?? throw new ArgumentNullException(nameof(updatedContentPageModel));

            logger.LogInformation($"Comparing differences to new: {updatedContentPageModel.Id} - {updatedContentPageModel.CanonicalName}");

            if (ContainsDifferences(existingContentPageModel, updatedContentPageModel))
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
            _ = updatedContentPageModel ?? throw new ArgumentNullException(nameof(updatedContentPageModel));

            if (!IsValidEventGridPublishClientOptions(logger, eventGridPublishClientOptions))
            {
                logger.LogWarning("Unable to send to event grid due to invalid EventGridPublishClientOptions options");
                return;
            }

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
                        Api = $"{eventGridPublishClientOptions.ApiEndpoint}/{updatedContentPageModel.Id}",
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

using AutoMapper;
using DFC.App.Pages.Data.Contracts;
using DFC.App.Pages.Data.Models;
using Microsoft.Extensions.Logging;
using System;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;

namespace DFC.App.Pages.Services.EventProcessorService
{
    public class EventGridSubscriptionService : IEventGridSubscriptionService
    {
        private readonly ILogger<EventGridSubscriptionService> logger;
        private readonly IMapper mapper;
        private readonly IApiDataProcessorService apiDataProcessorService;
        private readonly EventGridSubscriptionClientOptions eventGridSubscriptionClientOptions;
        private readonly HttpClient httpClient;

        public EventGridSubscriptionService(
            ILogger<EventGridSubscriptionService> logger,
            IMapper mapper,
            EventGridSubscriptionClientOptions eventGridSubscriptionClientOptions,
            IApiDataProcessorService apiDataProcessorService,
            HttpClient httpClient)
        {
            this.logger = logger;
            this.mapper = mapper;
            this.eventGridSubscriptionClientOptions = eventGridSubscriptionClientOptions;
            this.apiDataProcessorService = apiDataProcessorService;
            this.httpClient = httpClient;
        }

        public async Task CreateAsync(Guid id)
        {
            var url = new Uri($"{eventGridSubscriptionClientOptions.BaseAddress}{eventGridSubscriptionClientOptions.Endpoint}", UriKind.Absolute);
            var eventGridSubscriptionModel = mapper.Map<EventGridSubscriptionModel>(eventGridSubscriptionClientOptions);
            eventGridSubscriptionModel.Id = id;

            var statusCode = await apiDataProcessorService.PostAsync(httpClient, url, eventGridSubscriptionModel).ConfigureAwait(false);

            if (statusCode == HttpStatusCode.Created)
            {
                logger.LogInformation($"{nameof(CreateAsync)} has created an Event Grid subscription for: {id}");
            }
            else
            {
                logger.LogWarning($"{nameof(CreateAsync)} has not created an Event Grid subscription for: {id}: status code :{statusCode}");
            }
        }

        public async Task DeleteAsync(Guid id)
        {
            var url = new Uri($"{eventGridSubscriptionClientOptions.BaseAddress}{eventGridSubscriptionClientOptions.Endpoint}", UriKind.Absolute);
            var eventGridSubscriptionModel = mapper.Map<EventGridSubscriptionModel>(eventGridSubscriptionClientOptions);
            eventGridSubscriptionModel.Id = id;

            var statusCode = await apiDataProcessorService.DeleteAsync(httpClient, url, eventGridSubscriptionModel).ConfigureAwait(false);

            if (statusCode == HttpStatusCode.OK)
            {
                logger.LogInformation($"{nameof(DeleteAsync)} has deleted an Event Grid subscription for: {id}");
            }
            else
            {
                logger.LogWarning($"{nameof(DeleteAsync)} has not deleted an Event Grid subscription for: {id}: status code :{statusCode}");
            }
        }
    }
}

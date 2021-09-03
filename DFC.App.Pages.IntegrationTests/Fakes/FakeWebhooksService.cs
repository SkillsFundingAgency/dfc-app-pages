using DFC.App.Pages.Data.Contracts;
using DFC.App.Pages.Data.Enums;
using System;
using System.Net;
using System.Threading.Tasks;

namespace DFC.App.Pages.IntegrationTests.Fakes
{
    public class FakeWebhooksService : IWebhooksService
    {
        public Task<HttpStatusCode> ProcessMessageAsync(WebhookCacheOperation webhookCacheOperation, Guid eventId, Guid contentId, string apiEndpoint)
        {
            return Task.FromResult(HttpStatusCode.OK);
        }
    }
}

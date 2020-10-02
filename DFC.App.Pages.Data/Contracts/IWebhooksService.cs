using DFC.App.Pages.Data.Enums;
using DFC.App.Pages.Data.Models;
using System;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;

namespace DFC.App.Pages.Data.Contracts
{
    public interface IWebhooksService
    {
        Task<HttpStatusCode> DeleteContentAsync(Guid contentId);

        Task<HttpStatusCode> DeleteContentItemAsync(Guid contentItemId);

        Task<HttpStatusCode> ProcessContentAsync(Uri url, Guid contentId);

        Task<HttpStatusCode> ProcessContentItemAsync(Uri url, Guid contentItemId);

        Task<HttpStatusCode> ProcessMessageAsync(WebhookCacheOperation webhookCacheOperation, Guid eventId, Guid contentId, string apiEndpoint);

        ContentItemModel? FindContentItem(Guid itemId, List<ContentItemModel>? items);

        PageLocationModel? FindPageLocatonItem(Guid itemId, List<PageLocationModel>? items);

        bool RemoveContentItem(Guid contentItemId, List<ContentItemModel>? items);
    }
}

using DFC.App.Pages.Data.Models;
using System;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;

namespace DFC.App.Pages.Data.Contracts
{
    public interface IWebhookContentProcessor
    {
        Task<HttpStatusCode> ProcessContentAsync(Uri url, Guid contentId);

        Task<HttpStatusCode> ProcessContentItemAsync(Uri url, Guid contentItemId);

        Task<HttpStatusCode> DeleteContentAsync(Guid contentId);

        Task<HttpStatusCode> DeleteContentItemAsync(Guid contentItemId);

        bool RemoveContentItem(Guid contentItemId, List<ContentItemModel>? items);

        bool RemovePageLocation(Guid contentItemId, List<PageLocationModel>? items);

        bool TryValidateModel(ContentPageModel? contentPageModel);
    }
}
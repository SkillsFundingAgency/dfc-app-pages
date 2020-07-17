using DFC.App.Pages.Data.Enums;
using DFC.App.Pages.Data.Models;
using System.Threading.Tasks;

namespace DFC.App.Pages.Data.Contracts
{
    public interface IEventGridService
    {
        Task CompareAndSendEventAsync(ContentPageModel? existingContentPageModel, ContentPageModel updatedContentPageModel);

        Task SendEventAsync(WebhookCacheOperation webhookCacheOperation, ContentPageModel updatedContentPageModel);
    }
}

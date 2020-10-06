using DFC.App.Pages.Data.Models;
using DFC.App.Pages.Data.Models.CmsApiModels;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace DFC.App.Pages.Data.Contracts
{
    public interface ICacheReloadService
    {
        Task Reload(CancellationToken stoppingToken);

        Task<IList<CmsApiSummaryItemModel>?> GetSummaryListAsync();

        Task ProcessSummaryListAsync(IList<CmsApiSummaryItemModel> summaryList, CancellationToken stoppingToken);

        Task GetAndSaveItemAsync(CmsApiSummaryItemModel item, CancellationToken stoppingToken);

        Task DeleteStaleItemsAsync(List<ContentPageModel> staleItems, CancellationToken stoppingToken);

        Task DeleteStaleCacheEntriesAsync(IList<CmsApiSummaryItemModel> summaryList, CancellationToken stoppingToken);
    }
}
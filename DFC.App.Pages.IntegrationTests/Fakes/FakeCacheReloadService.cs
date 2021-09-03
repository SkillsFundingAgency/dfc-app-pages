using DFC.App.Pages.Data.Contracts;
using DFC.App.Pages.Data.Models;
using DFC.App.Pages.Data.Models.CmsApiModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace DFC.App.Pages.IntegrationTests.Fakes
{
    public class FakeCacheReloadService : ICacheReloadService
    {
        public Task DeleteStaleCacheEntriesAsync(IList<CmsApiSummaryItemModel> summaryList, CancellationToken stoppingToken)
        {
            throw new NotImplementedException();
        }

        public Task DeleteStaleItemsAsync(List<ContentPageModel> staleItems, CancellationToken stoppingToken)
        {
            throw new NotImplementedException();
        }

        public Task GetAndSaveItemAsync(CmsApiSummaryItemModel item, CancellationToken stoppingToken)
        {
            throw new NotImplementedException();
        }

        public Task<IList<CmsApiSummaryItemModel>?> GetSummaryListAsync()
        {
            IList<CmsApiSummaryItemModel>? result = new List<CmsApiSummaryItemModel>();
            return Task.FromResult(result.Any() ? result : default);
        }

        public Task ProcessSummaryListAsync(IList<CmsApiSummaryItemModel> summaryList, CancellationToken stoppingToken)
        {
            throw new NotImplementedException();
        }

        public Task Reload(CancellationToken stoppingToken)
        {
            return Task.CompletedTask;
        }
    }
}

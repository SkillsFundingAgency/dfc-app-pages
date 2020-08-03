using DFC.App.Pages.Data.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DFC.App.Pages.Data.Contracts
{
    public interface ICmsApiService
    {
        Task<IList<PagesSummaryItemModel>?> GetSummaryAsync();

        Task<PagesApiDataModel?> GetItemAsync(Uri url);

        Task<TApiModel?> GetContentItemAsync<TApiModel>(Uri uri)
            where TApiModel : class, IApiDataModel;
    }
}

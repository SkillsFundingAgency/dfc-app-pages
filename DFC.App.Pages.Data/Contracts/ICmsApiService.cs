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

        Task<PagesApiContentItemModel?> GetContentItemAsync(LinkDetails details);

        Task<PagesApiContentItemModel?> GetContentItemAsync(Uri uri);
    }
}

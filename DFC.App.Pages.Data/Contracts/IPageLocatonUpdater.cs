using DFC.App.Pages.Data.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DFC.App.Pages.Data.Contracts
{
    public interface IPageLocatonUpdater
    {
        Task<bool> FindAndUpdateAsync(Uri url, Guid contentItemId, List<PageLocationModel>? pageLocations);

        PageLocationModel? FindItem(Guid itemId, List<PageLocationModel>? items);
    }
}
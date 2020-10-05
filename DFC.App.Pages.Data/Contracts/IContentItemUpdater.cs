using DFC.App.Pages.Data.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DFC.App.Pages.Data.Contracts
{
    public interface IContentItemUpdater
    {
        Task<bool> FindAndUpdateAsync(Uri url, Guid contentItemId, List<ContentItemModel>? contentItems);

        ContentItemModel? FindItem(Guid itemId, List<ContentItemModel>? items);
    }
}
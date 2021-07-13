using DFC.App.Pages.Data.Models;
using DFC.Content.Pkg.Netcore.Data.Contracts;
using System;
using System.Threading.Tasks;

namespace DFC.App.Pages.Data.Contracts
{
    public interface IMarkupContentItemUpdater<T>
        where T : class, IBaseContentItemModel
    {
        Task<bool> FindAndUpdateAsync(ContentItemModel contentItemModel, Uri url);
    }
}
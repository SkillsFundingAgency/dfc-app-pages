using System;
using System.Collections.Generic;

namespace DFC.App.Pages.Data.Contracts
{
    public interface IContentCacheService
    {
        bool CheckIsContentItem(Guid contentItemId);

        void Clear();

        IList<Guid> GetContentIdsContainingContentItemId(Guid contentItemId);

        void Remove(Guid contentId);

        void RemoveContentItem(Guid contentId, Guid contentItemId);

        void AddOrReplace(Guid contentId, List<Guid> contentItemIds);
    }
}
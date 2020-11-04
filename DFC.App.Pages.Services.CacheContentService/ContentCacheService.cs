using DFC.App.Pages.Data.Contracts;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace DFC.App.Pages.Services.CacheContentService
{
    public class ContentCacheService : IContentCacheService
    {
        private readonly ILogger<ContentCacheService> logger;

        public ContentCacheService(ILogger<ContentCacheService> logger)
        {
            this.logger = logger;
        }

        private IDictionary<Guid, List<Guid>> ContentItems { get; set; } = new Dictionary<Guid, List<Guid>>();

        public bool CheckIsContentItem(Guid contentItemId)
        {
            logger.LogInformation($"Checking if {contentItemId} is a Content Item");

            foreach (var contentId in ContentItems.Keys)
            {
                if (ContentItems[contentId].Contains(contentItemId))
                {
                    logger.LogInformation($"{contentItemId} is a Content Item");
                    return true;
                }
            }

            logger.LogInformation($"{contentItemId} is NOT a Content Item");
            LogCacheContents();

            return false;
        }

        public void Clear()
        {
            logger.LogInformation($"Clear content cache called.");
            ContentItems.Clear();
        }

        public IList<Guid> GetContentIdsContainingContentItemId(Guid contentItemId)
        {
            logger.LogInformation($"Looking for {contentItemId} in content cache");
            LogCacheContents();

            var contentIds = new List<Guid>();

            foreach (var contentId in ContentItems.Keys)
            {
                if (ContentItems[contentId].Contains(contentItemId))
                {
                    contentIds.Add(contentId);
                }
            }

            return contentIds;
        }

        public void Remove(Guid contentId)
        {
            logger.LogInformation($"Removing Content {contentId} from cache");

            if (ContentItems.ContainsKey(contentId))
            {
                ContentItems.Remove(contentId);
            }

            LogCacheContents();
        }

        public void RemoveContentItem(Guid contentId, Guid contentItemId)
        {
            logger.LogInformation($"Removing Content Item {contentId} - {contentItemId} from cache");

            if (ContentItems.ContainsKey(contentId))
            {
                ContentItems[contentId].Remove(contentItemId);
            }

            LogCacheContents();
        }

        public void AddOrReplace(Guid contentId, List<Guid> contentItemIds)
        {
            logger.LogInformation($"Adding or Updating {contentId} with Items {JsonConvert.SerializeObject(contentItemIds)}");

            if (ContentItems.ContainsKey(contentId))
            {
                ContentItems[contentId] = contentItemIds;
            }
            else
            {
                ContentItems.Add(contentId, contentItemIds);
            }

            LogCacheContents();
        }

        private void LogCacheContents()
        {
            logger.LogInformation($"Cache contents is: {JsonConvert.SerializeObject(ContentItems)}");
        }
    }
}

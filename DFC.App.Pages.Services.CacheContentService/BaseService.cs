using DFC.App.Pages.Data.Models;
using System;

namespace DFC.App.Pages.Services.CacheContentService
{
    public static class BaseService
    {
        public static void ResetContentAndVersionFields(ContentPageModel contentPageModel, bool versionWasSet, bool contentWasSet)
        {
            if (contentPageModel == null)
            {
                throw new ArgumentNullException(nameof(contentPageModel));
            }

            if (versionWasSet)
            {
                contentPageModel.Version = null;
            }

            if (contentWasSet)
            {
                contentPageModel.Content = null;
            }
        }

        /// <summary>
        /// This is used as a workaround as we can't remove the annotations on the inherited type of 'Compui.Cosmos.Models.ContentPageModel', but
        /// do not care to have content or version set.
        /// </summary>
        /// <param name="contentPageModel">A populated content page model - either with content and version set, or not.</param>
        /// <returns>Two bools returning information on whether version and content had to be set or not.</returns>
        public static (bool versionWasSet, bool contentWasSet) IgnoreContentAndVersionFields(ContentPageModel contentPageModel)
        {
            if (contentPageModel == null)
            {
                throw new ArgumentNullException(nameof(contentPageModel));
            }

            var versionWasSet = false;

            if (contentPageModel.Version == null)
            {
                contentPageModel.Version = Guid.NewGuid();
                versionWasSet = true;
            }

            var contentWasSet = false;

            if (contentPageModel.Content == null)
            {
                contentPageModel.Content = "[ANY CONTENT HERE TO BYPASS CHECK]";
                contentWasSet = true;
            }

            return (versionWasSet, contentWasSet);
        }
    }
}

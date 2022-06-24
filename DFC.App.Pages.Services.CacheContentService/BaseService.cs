using DFC.App.Pages.Data.Models;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;

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

        public static bool TryValidateModel(ContentPageModel? contentPageModel, ILogger logger)
        {
            _ = contentPageModel ?? throw new ArgumentNullException(nameof(contentPageModel));
            var (versionWasSet, contentWasSet) = IgnoreContentAndVersionFields(contentPageModel);

            var validationContext = new ValidationContext(contentPageModel, null, null);
            var validationResults = new List<ValidationResult>();
            var isValid = Validator.TryValidateObject(contentPageModel, validationContext, validationResults, true);

            if (!isValid && validationResults.Any())
            {
                foreach (var validationResult in validationResults)
                {
                    logger.LogError($"Error validating {contentPageModel.CanonicalName} - {contentPageModel.Url}: {string.Join(",", validationResult.MemberNames)} - {validationResult.ErrorMessage}");
                }
            }

            ResetContentAndVersionFields(contentPageModel, versionWasSet, contentWasSet);

            if (string.IsNullOrEmpty(contentPageModel.PartitionKey))
            {
                logger.LogError($"Error validating {contentPageModel.CanonicalName} - Partition key (and thus page location) was null");
                return false;
            }

            return isValid;
        }
    }
}

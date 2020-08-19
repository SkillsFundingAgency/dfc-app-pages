using DFC.App.Pages.Data.Common;
using DFC.App.Pages.Data.Contracts;
using DFC.App.Pages.Data.Models;
using DFC.Compui.Cosmos.Contracts;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace DFC.App.Pages.Services.EventProcessorService
{
    public class EventMessageService<TModel> : IEventMessageService<TModel>
           where TModel : class, IContentPageModel
    {
        private readonly ILogger<EventMessageService<TModel>> logger;
        private readonly IContentPageService<TModel> contentPageService;

        public EventMessageService(ILogger<EventMessageService<TModel>> logger, IContentPageService<TModel> contentPageService)
        {
            this.logger = logger;
            this.contentPageService = contentPageService;
        }

        public async Task<IList<TModel>?> GetAllCachedItemsAsync()
        {
            var serviceDataModels = await contentPageService.GetAllAsync().ConfigureAwait(false);

            return serviceDataModels?.ToList();
        }

        public async Task<HttpStatusCode> CreateAsync(TModel? upsertDocumentModel)
        {
            if (upsertDocumentModel == null)
            {
                return HttpStatusCode.BadRequest;
            }

            var existingDocument = await contentPageService.GetByIdAsync(upsertDocumentModel.Id).ConfigureAwait(false);
            if (existingDocument != null)
            {
                return HttpStatusCode.AlreadyReported;
            }

            upsertDocumentModel.PageLocation = ExtractPageLocation(upsertDocumentModel);

            var response = await contentPageService.UpsertAsync(upsertDocumentModel).ConfigureAwait(false);

            logger.LogInformation($"{nameof(CreateAsync)} has upserted content for: {upsertDocumentModel.CanonicalName} with response code {response}");

            return response;
        }

        public async Task<HttpStatusCode> UpdateAsync(TModel? upsertDocumentModel)
        {
            if (upsertDocumentModel == null)
            {
                return HttpStatusCode.BadRequest;
            }

            var existingDocument = await contentPageService.GetByIdAsync(upsertDocumentModel.Id).ConfigureAwait(false);
            if (existingDocument == null)
            {
                return HttpStatusCode.NotFound;
            }

            upsertDocumentModel.PageLocation = ExtractPageLocation(upsertDocumentModel);

            if (existingDocument.PartitionKey != null && existingDocument.PartitionKey.Equals(upsertDocumentModel.PartitionKey, StringComparison.Ordinal))
            {
                upsertDocumentModel.Etag = existingDocument.Etag;
            }
            else
            {
                var deleted = await contentPageService.DeleteAsync(existingDocument.Id).ConfigureAwait(false);

                if (deleted)
                {
                    logger.LogInformation($"{nameof(UpdateAsync)} has deleted content for: {existingDocument.CanonicalName} due to partition key change: {existingDocument.PartitionKey} -> {upsertDocumentModel.PartitionKey}");
                }
                else
                {
                    logger.LogWarning($"{nameof(UpdateAsync)} failed to delete content for: {existingDocument.CanonicalName} due to partition key change: {existingDocument.PartitionKey} -> {upsertDocumentModel.PartitionKey}");
                    return HttpStatusCode.BadRequest;
                }
            }

            var response = await contentPageService.UpsertAsync(upsertDocumentModel).ConfigureAwait(false);

            logger.LogInformation($"{nameof(UpdateAsync)} has upserted content for: {upsertDocumentModel.CanonicalName} with response code {response}");

            return response;
        }

        public async Task<HttpStatusCode> DeleteAsync(Guid id)
        {
            var isDeleted = await contentPageService.DeleteAsync(id).ConfigureAwait(false);

            if (isDeleted)
            {
                logger.LogInformation($"{nameof(DeleteAsync)} has deleted content for document Id: {id}");
                return HttpStatusCode.OK;
            }
            else
            {
                logger.LogWarning($"{nameof(DeleteAsync)} has returned no content for: {id}");
                return HttpStatusCode.NotFound;
            }
        }

        public string? ExtractPageLocation(TModel? model)
        {
            string? result = null;
            var contentPageModel = model as ContentPageModel;
            var contentItems = contentPageModel?.ContentItems.Where(w => w.ContentType == Constants.ContentTypePageLocation).ToList();

            if (contentItems != null && contentItems.Any())
            {
                var pageLocations = ExtractPageLocationItem(contentItems);

                pageLocations.RemoveAll(r => string.IsNullOrWhiteSpace(r));
                pageLocations.RemoveAll(r => r.Equals("/", StringComparison.Ordinal));

                pageLocations.Reverse();

                result = "/" + string.Join("/", pageLocations);
            }

            return result;
        }

        private List<string> ExtractPageLocationItem(List<ContentItemModel> contentItems)
        {
            var pageLocations = new List<string>();

            foreach (var contentItem in contentItems)
            {
                if (!string.IsNullOrWhiteSpace(contentItem.BreadcrumbLinkSegment))
                {
                    pageLocations.Add(contentItem.BreadcrumbLinkSegment);
                }

                var children = contentItem.ContentItems.Where(w => w.ContentType == Constants.ContentTypePageLocation).ToList();

                if (children != null && children.Any())
                {
                    pageLocations.AddRange(ExtractPageLocationItem(children));
                }
            }

            return pageLocations;
        }
    }
}

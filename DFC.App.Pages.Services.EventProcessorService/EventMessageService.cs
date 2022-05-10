using DFC.App.Pages.Data.Contracts;
using DFC.App.Pages.Data.Models;
using DFC.Compui.Cosmos.Contracts;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Newtonsoft.Json;

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

            var beforePageLocationUpdate = JsonConvert.SerializeObject(upsertDocumentModel);
            upsertDocumentModel.PageLocation = ExtractPageLocation(upsertDocumentModel);

            if (string.IsNullOrEmpty(upsertDocumentModel.PageLocation))
            {
                logger.LogError(
                    "PageLocation ({PageLocation}) and/or PartitionKey ({PartitionKey}) is empty or null. Document before = {SerialisedDocumentBefore}. Document = {SerialisedDocument}",
                    upsertDocumentModel.PageLocation,
                    upsertDocumentModel.PartitionKey,
                    beforePageLocationUpdate,
                    JsonConvert.SerializeObject(upsertDocumentModel));

                return HttpStatusCode.BadRequest;
            }

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

            var beforePageLocationUpdate = JsonConvert.SerializeObject(upsertDocumentModel);
            upsertDocumentModel.PageLocation = ExtractPageLocation(upsertDocumentModel);

            if (string.IsNullOrEmpty(upsertDocumentModel.PageLocation) || string.IsNullOrEmpty(upsertDocumentModel.PartitionKey))
            {
                logger.LogError(
                    "PageLocation ({PageLocation}) and/or PartitionKey ({PartitionKey}) is empty or null. Document before = {SerialisedDocumentBefore}. Document = {SerialisedDocument}",
                    upsertDocumentModel.PageLocation,
                    upsertDocumentModel.PartitionKey,
                    beforePageLocationUpdate,
                    JsonConvert.SerializeObject(upsertDocumentModel));

                return HttpStatusCode.BadRequest;
            }

            if (existingDocument.PartitionKey?.Equals(upsertDocumentModel.PartitionKey, StringComparison.OrdinalIgnoreCase) == true)
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
            var contentPageModel = model as ContentPageModel;

            if (contentPageModel?.PageLocations?.Any() != true)
            {
                logger.LogInformation(
                    "{MethodName} returns null. Is ContentPageModel = {ContentPageModelCastable}. {PageLocationCount} page locations",
                    nameof(ExtractPageLocation),
                    contentPageModel != null,
                    contentPageModel?.PageLocations?.Count);

                return null;
            }

            var pageLocations = ExtractPageLocationItem(contentPageModel.PageLocations);
            pageLocations.RemoveAll(r => string.IsNullOrWhiteSpace(r));
            pageLocations.RemoveAll(r => r.Equals("/", StringComparison.Ordinal));
            pageLocations.Reverse();

            return $"/{string.Join("/", pageLocations)}";
        }

        private List<string> ExtractPageLocationItem(List<PageLocationModel> parentPageLocations)
        {
            var pagelocations = new List<string>();

            foreach (var item in parentPageLocations)
            {
                if (!string.IsNullOrWhiteSpace(item.BreadcrumbLinkSegment))
                {
                    pagelocations.Add(item.BreadcrumbLinkSegment);
                }

                if (item.PageLocations != null && item.PageLocations.Any())
                {
                    pagelocations.AddRange(ExtractPageLocationItem(item.PageLocations));
                }
            }

            return pagelocations;
        }
    }
}

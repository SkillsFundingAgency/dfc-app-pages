using AutoMapper;
using DFC.App.Pages.Data.Contracts;
using DFC.App.Pages.Data.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace DFC.App.Pages.Services.CmsApiProcessorService
{
    public class CmsApiService : ICmsApiService
    {
        private readonly CmsApiClientOptions cmsApiClientOptions;
        private readonly IApiDataProcessorService apiDataProcessorService;
        private readonly HttpClient httpClient;
        private readonly AutoMapper.IMapper mapper;

        public CmsApiService(
            CmsApiClientOptions cmsApiClientOptions,
            IApiDataProcessorService apiDataProcessorService,
            HttpClient httpClient,
            IMapper mapper)
        {
            this.cmsApiClientOptions = cmsApiClientOptions;
            this.apiDataProcessorService = apiDataProcessorService;
            this.httpClient = httpClient;
            this.mapper = mapper;
        }

        public async Task<IList<PagesSummaryItemModel>?> GetSummaryAsync()
        {
            var url = new Uri(
                $"{cmsApiClientOptions.BaseAddress}{cmsApiClientOptions.SummaryEndpoint}",
                UriKind.Absolute);

            return await apiDataProcessorService.GetAsync<IList<PagesSummaryItemModel>>(httpClient, url)
                .ConfigureAwait(false);
        }

        public async Task<PagesApiDataModel?> GetItemAsync(Uri url)
        {
            var pagesApiDataModel = await apiDataProcessorService.GetAsync<PagesApiDataModel>(httpClient, url)
                .ConfigureAwait(false);

            await GetSharedChildContentItems(pagesApiDataModel.ContentLinks, pagesApiDataModel.ContentItems).ConfigureAwait(false);

            return pagesApiDataModel;
        }

        public async Task<PagesApiContentItemModel?> GetContentItemAsync(LinkDetails details)
        {
            return await apiDataProcessorService.GetAsync<PagesApiContentItemModel>(httpClient, details.Uri)
                .ConfigureAwait(false);
        }

        public async Task<PagesApiContentItemModel?> GetContentItemAsync(Uri uri)
        {
            return await apiDataProcessorService.GetAsync<PagesApiContentItemModel>(httpClient, uri)
                .ConfigureAwait(false);
        }

        private async Task GetSharedChildContentItems(ContentLinksModel? model, IList<PagesApiContentItemModel> contentItem)
        {
            if (model != null && model.ContentLinks.Any())
            {
                foreach (var linkDetail in model.ContentLinks.SelectMany(contentLink => contentLink.Value))
                {
                    var pagesApiContentItemModel =
                        await GetContentItemAsync(linkDetail).ConfigureAwait(false);

                    if (pagesApiContentItemModel != null)
                    {
                        mapper.Map(linkDetail, pagesApiContentItemModel);
                        await GetSharedChildContentItems(pagesApiContentItemModel.ContentLinks, pagesApiContentItemModel.ContentItems).ConfigureAwait(false);
                        contentItem.Add(pagesApiContentItemModel);
                    }
                }
            }
        }
    }
}

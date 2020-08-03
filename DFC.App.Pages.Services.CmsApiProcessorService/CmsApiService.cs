using AutoMapper;
using DFC.App.Pages.Data.Contracts;
using DFC.App.Pages.Data.Models;
using DFC.App.Pages.Data.Models.ClientOptions;
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

            return await apiDataProcessorService.GetAsync<IList<PagesSummaryItemModel>>(httpClient, url).ConfigureAwait(false);
        }

        public async Task<PagesApiDataModel?> GetItemAsync(Uri url)
        {
            var pagesApiDataModel = await apiDataProcessorService.GetAsync<PagesApiDataModel>(httpClient, url).ConfigureAwait(false);

            if (pagesApiDataModel != null)
            {
                await GetSharedChildContentItems(pagesApiDataModel.ContentLinks, pagesApiDataModel.ContentItems).ConfigureAwait(false);
            }

            return pagesApiDataModel;
        }

        public async Task<TApiModel?> GetContentItemAsync<TApiModel>(Uri uri)
            where TApiModel : class, IApiDataModel
        {
            return await apiDataProcessorService.GetAsync<TApiModel>(httpClient, uri).ConfigureAwait(false);
        }

        private async Task GetSharedChildContentItems(ContentLinksModel? model, IList<PagesApiContentItemModel> contentItem)
        {
            var linkDetails = model?.ContentLinks.SelectMany(contentLink => contentLink.Value);

            if (linkDetails != null && linkDetails.Any())
            {
                foreach (var linkDetail in linkDetails)
                {
                    var pagesApiContentItemModel = await GetContentItemAsync<PagesApiContentItemModel>(linkDetail.Uri!).ConfigureAwait(false);

                    if (pagesApiContentItemModel != null)
                    {
                        mapper.Map(linkDetail, pagesApiContentItemModel);

                        if (pagesApiContentItemModel.ContentLinks != null)
                        {
                            pagesApiContentItemModel.ContentLinks.ExcludePageLocation = true;

                            await GetSharedChildContentItems(pagesApiContentItemModel.ContentLinks, pagesApiContentItemModel.ContentItems).ConfigureAwait(false);
                        }

                        contentItem.Add(pagesApiContentItemModel);
                    }
                }
            }
        }
    }
}

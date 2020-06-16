using DFC.App.Pages.Data.Contracts;
using DFC.App.Pages.Data.Models;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;

namespace DFC.App.Pages.Services.CmsApiProcessorService
{
    public class CmsApiService : ICmsApiService
    {
        private readonly CmsApiClientOptions cmsApiClientOptions;
        private readonly IApiDataProcessorService apiDataProcessorService;
        private readonly HttpClient httpClient;

        public CmsApiService(CmsApiClientOptions cmsApiClientOptions, IApiDataProcessorService apiDataProcessorService, HttpClient httpClient)
        {
            this.cmsApiClientOptions = cmsApiClientOptions;
            this.apiDataProcessorService = apiDataProcessorService;
            this.httpClient = httpClient;
        }

        public async Task<IList<PagesSummaryItemModel>?> GetSummaryAsync()
        {
            var url = new Uri($"{cmsApiClientOptions.BaseAddress}{cmsApiClientOptions.SummaryEndpoint}", UriKind.Absolute);

            return await apiDataProcessorService.GetAsync<IList<PagesSummaryItemModel>>(httpClient, url).ConfigureAwait(false);
        }

        public async Task<PagesApiDataModel?> GetItemAsync(Uri url)
        {
            var pagesApiDataModel = await apiDataProcessorService.GetAsync<PagesApiDataModel>(httpClient, url).ConfigureAwait(false);

            if (pagesApiDataModel?.ContentItemUrls != null)
            {
                foreach (var contentItemUrl in pagesApiDataModel.ContentItemUrls)
                {
                    var pagesApiContentItemModel = await GetContentItemAsync(contentItemUrl).ConfigureAwait(false);

                    if (pagesApiContentItemModel != null)
                    {
                        pagesApiDataModel.ContentItems.Add(pagesApiContentItemModel);
                    }
                }
            }

            return pagesApiDataModel;
        }

        public async Task<PagesApiContentItemModel?> GetContentItemAsync(Uri url)
        {
            return await apiDataProcessorService.GetAsync<PagesApiContentItemModel>(httpClient, url).ConfigureAwait(false);
        }
    }
}

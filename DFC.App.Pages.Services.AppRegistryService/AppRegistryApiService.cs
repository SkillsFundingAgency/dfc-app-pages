using DFC.App.Pages.Data.Contracts;
using DFC.App.Pages.Data.Models.ClientOptions;
using DFC.Content.Pkg.Netcore.Data.Contracts;
using System.Net.Http;
using System.Threading.Tasks;

namespace DFC.App.Pages.Services.AppRegistryService
{
    public class AppRegistryApiService : IAppRegistryApiService
    {
        private readonly IApiDataProcessorService apiDataProcessorService;
        private readonly HttpClient httpClient;
        private readonly AppRegistryClientOptions appRegistryClientOptions;

        public AppRegistryApiService(IApiDataProcessorService apiDataProcessorService, HttpClient httpClient, AppRegistryClientOptions appRegistryClientOptions)
        {
            this.apiDataProcessorService = apiDataProcessorService;
            this.httpClient = httpClient;
            this.appRegistryClientOptions = appRegistryClientOptions;
        }

        public async Task PagesDataLoadAsync()
        {
            await apiDataProcessorService.PostAsync(httpClient, appRegistryClientOptions.BaseAddress).ConfigureAwait(false);
        }
    }
}

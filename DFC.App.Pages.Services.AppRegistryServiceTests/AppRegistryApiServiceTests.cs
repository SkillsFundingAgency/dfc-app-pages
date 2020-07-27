using DFC.App.Pages.Data.Contracts;
using DFC.App.Pages.Data.Models.ClientOptions;
using DFC.App.Pages.Services.AppRegistryService;
using FakeItEasy;
using System;
using System.Net.Http;
using System.Threading.Tasks;
using Xunit;

namespace DFC.App.Pages.Services.AppRegistryServiceTests
{
    public class AppRegistryApiServiceTests
    {
        private readonly IApiDataProcessorService fakeApiDataProcessorService = A.Fake<IApiDataProcessorService>();
        private readonly AppRegistryClientOptions appRegistryClientOptions = new AppRegistryClientOptions
        {
            BaseAddress = new Uri("https://somewhere.com", UriKind.Absolute),
            ApiKey = null,
        };

        [Fact]
        public async Task AppRegistryApiServicePagesDataLoadAsyncSuccess()
        {
            // arrange
            var fakeHttpClient = A.Fake<HttpClient>();

            var appRegistryApiService = new AppRegistryApiService(fakeApiDataProcessorService, fakeHttpClient, appRegistryClientOptions);

            // act
            await appRegistryApiService.PagesDataLoadAsync().ConfigureAwait(false);

            // assert
            A.CallTo(() => fakeApiDataProcessorService.PostAsync(A<HttpClient>.Ignored, A<Uri>.Ignored)).MustHaveHappenedOnceExactly();
        }
    }
}

using DFC.App.Pages.Data.Contracts;
using DFC.App.Pages.Data.Models;
using FakeItEasy;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using Xunit;

namespace DFC.App.Pages.Services.CmsApiProcessorService.UnitTests
{
    public class CmsApiServiceTests
    {
        private const string ValidPostcode = "CV1 1CV";

        private readonly IApiDataProcessorService fakeApiDataProcessorService = A.Fake<IApiDataProcessorService>();
        private readonly HttpClient fakeHttpClient = A.Fake<HttpClient>();

        private CmsApiClientOptions CmsApiClientOptions
        {
            get
            {
                return new CmsApiClientOptions
                {
                    BaseAddress = new Uri("https://localhost/", UriKind.Absolute),
                    SummaryEndpoint = "api/something",
                };
            }
        }

        [Fact]
        public async Task CmsApiServiceGetSummaryReturnsSuccess()
        {
            // arrange
            var expectedResults = A.CollectionOfFake<PagesSummaryItemModel>(2);

            A.CallTo(() => fakeApiDataProcessorService.GetAsync<IList<PagesSummaryItemModel>>(A<HttpClient>.Ignored, A<Uri>.Ignored)).Returns(expectedResults);

            var cmsApiService = new CmsApiService(CmsApiClientOptions, fakeApiDataProcessorService, fakeHttpClient);

            // act
            var result = await cmsApiService.GetSummaryAsync().ConfigureAwait(false);

            // assert
            A.CallTo(() => fakeApiDataProcessorService.GetAsync<IList<PagesSummaryItemModel>>(A<HttpClient>.Ignored, A<Uri>.Ignored)).MustHaveHappenedOnceExactly();
            A.Equals(result, expectedResults);
        }

        [Fact]
        public async Task CmsApiServiceGetItemReturnsSuccess()
        {
            // arrange
            var expectedResult = A.Fake<PagesApiDataModel>();
            var expectedItemResult = A.Fake<PagesApiContentItemModel>();
            var url = new Uri($"{CmsApiClientOptions.BaseAddress}api/someitem", UriKind.Absolute);
            expectedResult.ContentItemUrls = new List<Uri> { new Uri(url.ToString() + "/one"), new Uri(url.ToString() + "/two"), new Uri(url.ToString() + "/three"), };

            A.CallTo(() => fakeApiDataProcessorService.GetAsync<PagesApiDataModel>(A<HttpClient>.Ignored, A<Uri>.Ignored)).Returns(expectedResult);
            A.CallTo(() => fakeApiDataProcessorService.GetAsync<PagesApiContentItemModel>(A<HttpClient>.Ignored, A<Uri>.Ignored)).Returns(expectedItemResult);

            var cmsApiService = new CmsApiService(CmsApiClientOptions, fakeApiDataProcessorService, fakeHttpClient);

            // act
            var result = await cmsApiService.GetItemAsync(url).ConfigureAwait(false);

            // assert
            A.CallTo(() => fakeApiDataProcessorService.GetAsync<PagesApiDataModel>(A<HttpClient>.Ignored, A<Uri>.Ignored)).MustHaveHappenedOnceExactly();
            A.CallTo(() => fakeApiDataProcessorService.GetAsync<PagesApiContentItemModel>(A<HttpClient>.Ignored, A<Uri>.Ignored)).MustHaveHappened(expectedResult.ContentItemUrls.Count, Times.Exactly);
            A.Equals(result, expectedResult);
        }

        [Fact]
        public async Task CmsApiServiceGetContentItemReturnsSuccess()
        {
            // arrange
            var expectedResult = A.Fake<PagesApiContentItemModel>();
            var url = new Uri($"{CmsApiClientOptions.BaseAddress}api/someitemcontent", UriKind.Absolute);

            A.CallTo(() => fakeApiDataProcessorService.GetAsync<PagesApiContentItemModel>(A<HttpClient>.Ignored, A<Uri>.Ignored)).Returns(expectedResult);

            var cmsApiService = new CmsApiService(CmsApiClientOptions, fakeApiDataProcessorService, fakeHttpClient);

            // act
            var result = await cmsApiService.GetContentItemAsync(url).ConfigureAwait(false);

            // assert
            A.CallTo(() => fakeApiDataProcessorService.GetAsync<PagesApiContentItemModel>(A<HttpClient>.Ignored, A<Uri>.Ignored)).MustHaveHappenedOnceExactly();
            A.Equals(result, expectedResult);
        }
    }
}

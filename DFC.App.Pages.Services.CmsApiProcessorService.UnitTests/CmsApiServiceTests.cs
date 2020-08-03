using AutoMapper;
using DFC.App.Pages.Data.Contracts;
using DFC.App.Pages.Data.Models;
using DFC.App.Pages.Data.Models.ClientOptions;
using FakeItEasy;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Xunit;

namespace DFC.App.Pages.Services.CmsApiProcessorService.UnitTests
{
    public class CmsApiServiceTests
    {
        private readonly IApiDataProcessorService fakeApiDataProcessorService = A.Fake<IApiDataProcessorService>();
        private readonly HttpClient fakeHttpClient = A.Fake<HttpClient>();
        private readonly AutoMapper.Mapper mapper = A.Fake<Mapper>();

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
        public async Task CmsApiServiceGetSummaryReturnsNullFornNData()
        {
            // arrange
            IList<PagesSummaryItemModel>? nullExpectedResults = null;

            A.CallTo(() => fakeApiDataProcessorService.GetAsync<IList<PagesSummaryItemModel>>(A<HttpClient>.Ignored, A<Uri>.Ignored)).Returns(nullExpectedResults);

            var cmsApiService = new CmsApiService(CmsApiClientOptions, fakeApiDataProcessorService, fakeHttpClient, mapper);

            // act
            var result = await cmsApiService.GetSummaryAsync().ConfigureAwait(false);

            // assert
            A.CallTo(() => fakeApiDataProcessorService.GetAsync<IList<PagesSummaryItemModel>>(A<HttpClient>.Ignored, A<Uri>.Ignored)).MustHaveHappenedOnceExactly();
            Assert.Equal(result, nullExpectedResults);
        }

        [Fact]
        public async Task CmsApiServiceGetSummaryReturnsSuccess()
        {
            // arrange
            var expectedResults = A.CollectionOfFake<PagesSummaryItemModel>(2);

            A.CallTo(() => fakeApiDataProcessorService.GetAsync<IList<PagesSummaryItemModel>>(A<HttpClient>.Ignored, A<Uri>.Ignored)).Returns(expectedResults);

            var cmsApiService = new CmsApiService(CmsApiClientOptions, fakeApiDataProcessorService, fakeHttpClient, mapper);

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
            var contentUrl = new Uri("http://www.test.com");
            var childContentUrl = new Uri("http://www.testChild.com");
            A.CallTo(() => fakeApiDataProcessorService.GetAsync<PagesApiDataModel>(A<HttpClient>.Ignored, A<Uri>.Ignored)).Returns(expectedResult);
            A.CallTo(() => fakeApiDataProcessorService.GetAsync<PagesApiContentItemModel>(A<HttpClient>.Ignored, contentUrl)).Returns(expectedItemResult);
            A.CallTo(() => fakeApiDataProcessorService.GetAsync<PagesApiContentItemModel>(A<HttpClient>.Ignored, childContentUrl)).Returns(new PagesApiContentItemModel());
            expectedResult.ContentLinks = new ContentLinksModel(new JObject())
            {
                ContentLinks = new List<KeyValuePair<string, List<LinkDetailModel>>>()
                {
                    new KeyValuePair<string, List<LinkDetailModel>>(
                        "test",
                        new List<LinkDetailModel>
                        {
                            new LinkDetailModel
                            {
                                Uri = contentUrl,
                            },
                        }),
                },
            };
            expectedItemResult.ContentLinks = new ContentLinksModel(new JObject())
            {
                ContentLinks = new List<KeyValuePair<string, List<LinkDetailModel>>>()
                {
                    new KeyValuePair<string, List<LinkDetailModel>>(
                        "Child",
                        new List<LinkDetailModel>
                        {
                            new LinkDetailModel
                            {
                                Uri = new Uri("http://www.testChild.com"),
                            },
                        }),
                },
            };

            var cmsApiService = new CmsApiService(CmsApiClientOptions, fakeApiDataProcessorService, fakeHttpClient, mapper);

            // act
            var result = await cmsApiService.GetItemAsync(url).ConfigureAwait(false);

            // assert
            var expectedCount =
                expectedResult.ContentLinks.ContentLinks.SelectMany(contentLink => contentLink.Value).Count() +
                expectedItemResult.ContentLinks.ContentLinks.SelectMany(contentLink => contentLink.Value).Count();

            A.CallTo(() => fakeApiDataProcessorService.GetAsync<PagesApiDataModel>(A<HttpClient>.Ignored, A<Uri>.Ignored)).MustHaveHappenedOnceExactly();
            A.CallTo(() => fakeApiDataProcessorService.GetAsync<PagesApiContentItemModel>(A<HttpClient>.Ignored, A<Uri>.Ignored)).MustHaveHappened(expectedCount, Times.Exactly);
            A.Equals(result, expectedResult);
        }

        [Fact]
        public async Task CmsApiServiceGetContentItemReturnsSuccess()
        {
            // arrange
            var expectedResult = A.Fake<PagesApiContentItemModel>();
            var url = new Uri($"{CmsApiClientOptions.BaseAddress}api/someitemcontent", UriKind.Absolute);

            A.CallTo(() => fakeApiDataProcessorService.GetAsync<PagesApiContentItemModel>(A<HttpClient>.Ignored, A<Uri>.Ignored)).Returns(expectedResult);

            var cmsApiService = new CmsApiService(CmsApiClientOptions, fakeApiDataProcessorService, fakeHttpClient, mapper);

            // act
            var result = await cmsApiService.GetContentItemAsync<PagesApiContentItemModel>(url).ConfigureAwait(false);

            // assert
            A.CallTo(() => fakeApiDataProcessorService.GetAsync<PagesApiContentItemModel>(A<HttpClient>.Ignored, A<Uri>.Ignored)).MustHaveHappenedOnceExactly();
            A.Equals(result, expectedResult);
        }
    }
}

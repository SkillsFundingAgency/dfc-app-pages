using DFC.App.Pages.Data.Contracts;
using DFC.App.Pages.Data.Models;
using FakeItEasy;
using Newtonsoft.Json;
using System;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Xunit;

namespace DFC.App.Pages.Services.ApiProcessorService.UnitTests
{
    [Trait("Category", "API Data Processor Service Unit Tests")]
    public class ApiDataProcessorServiceTests
    {
        private readonly IApiService fakeApiService = A.Fake<IApiService>();

        [Fact]
        public async Task ApiDataProcessorServiceGetReturnsSuccess()
        {
            // arrange
            var expectedResult = new PagesSummaryItemModel
            {
                Url = new Uri("https://somewhere.com"),
                Title = "a-name",
                Published = DateTime.Now,
                CreatedDate = DateTime.Now,
            };
            var jsonResponse = JsonConvert.SerializeObject(expectedResult);

            A.CallTo(() => fakeApiService.GetAsync(A<HttpClient>.Ignored, A<Uri>.Ignored, A<string>.Ignored)).Returns(jsonResponse);

            var apiDataProcessorService = new ApiDataProcessorService(fakeApiService);

            // act
            var result = await apiDataProcessorService.GetAsync<PagesSummaryItemModel>(A.Fake<HttpClient>(), new Uri("https://somewhere.com")).ConfigureAwait(false);

            // assert
            A.CallTo(() => fakeApiService.GetAsync(A<HttpClient>.Ignored, A<Uri>.Ignored, A<string>.Ignored)).MustHaveHappenedOnceExactly();
            A.Equals(result, expectedResult);
        }

        [Fact]
        public async Task ApiDataProcessorServiceGetReturnsNullForNoData()
        {
            // arrange
            PagesSummaryItemModel? expectedResult = null;

            A.CallTo(() => fakeApiService.GetAsync(A<HttpClient>.Ignored, A<Uri>.Ignored, A<string>.Ignored)).Returns(string.Empty);

            var apiDataProcessorService = new ApiDataProcessorService(fakeApiService);

            // act
            var result = await apiDataProcessorService.GetAsync<PagesSummaryItemModel>(A.Fake<HttpClient>(), new Uri("https://somewhere.com")).ConfigureAwait(false);

            // assert
            A.CallTo(() => fakeApiService.GetAsync(A<HttpClient>.Ignored, A<Uri>.Ignored, A<string>.Ignored)).MustHaveHappenedOnceExactly();
            A.Equals(result, expectedResult);
        }

        [Fact]
        public async Task ApiDataProcessorServiceGetReturnsExceptionForNoHttpClient()
        {
            // arrange
            var apiDataProcessorService = new ApiDataProcessorService(fakeApiService);

            // act
            var exceptionResult = await Assert.ThrowsAsync<ArgumentNullException>(async () => await apiDataProcessorService.GetAsync<PagesSummaryItemModel>(null, new Uri("https://somewhere.com")).ConfigureAwait(false)).ConfigureAwait(false);

            // assert
            A.CallTo(() => fakeApiService.GetAsync(A<HttpClient>.Ignored, A<Uri>.Ignored, A<string>.Ignored)).MustNotHaveHappened();
            Assert.Equal("Value cannot be null. (Parameter 'httpClient')", exceptionResult.Message);
        }

        [Fact]
        public async Task ApiDataProcessorServicePostReturnsSuccess()
        {
            // arrange
            var expectedResult = HttpStatusCode.Created;
            var fakeEventGridSubscriptionModel = A.Fake<EventGridSubscriptionModel>();

            A.CallTo(() => fakeApiService.PostAsync(A<HttpClient>.Ignored, A<Uri>.Ignored, A<EventGridSubscriptionModel>.Ignored)).Returns(expectedResult);

            var apiDataProcessorService = new ApiDataProcessorService(fakeApiService);

            // act
            var result = await apiDataProcessorService.PostAsync(A.Fake<HttpClient>(), new Uri("https://somewhere.com"), fakeEventGridSubscriptionModel).ConfigureAwait(false);

            // assert
            A.CallTo(() => fakeApiService.PostAsync(A<HttpClient>.Ignored, A<Uri>.Ignored, A<EventGridSubscriptionModel>.Ignored)).MustHaveHappenedOnceExactly();
            A.Equals(result, expectedResult);
        }

        [Fact]
        public async Task ApiDataProcessorServicePostReturnsExceptionForNoHttpClient()
        {
            // arrange
            var fakeEventGridSubscriptionModel = A.Fake<EventGridSubscriptionModel>();

            var apiDataProcessorService = new ApiDataProcessorService(fakeApiService);

            // act
            var exceptionResult = await Assert.ThrowsAsync<ArgumentNullException>(async () => await apiDataProcessorService.PostAsync(null, new Uri("https://somewhere.com"), fakeEventGridSubscriptionModel).ConfigureAwait(false)).ConfigureAwait(false);

            // assert
            A.CallTo(() => fakeApiService.PostAsync(A<HttpClient>.Ignored, A<Uri>.Ignored, A<string>.Ignored)).MustNotHaveHappened();
            Assert.Equal("Value cannot be null. (Parameter 'httpClient')", exceptionResult.Message);
        }

        [Fact]
        public async Task ApiDataProcessorServiceDeleteReturnsSuccess()
        {
            // arrange
            var expectedResult = HttpStatusCode.Created;
            var fakeEventGridSubscriptionModel = A.Fake<EventGridSubscriptionModel>();

            A.CallTo(() => fakeApiService.DeleteAsync(A<HttpClient>.Ignored, A<Uri>.Ignored, A<EventGridSubscriptionModel>.Ignored)).Returns(expectedResult);

            var apiDataProcessorService = new ApiDataProcessorService(fakeApiService);

            // act
            var result = await apiDataProcessorService.DeleteAsync(A.Fake<HttpClient>(), new Uri("https://somewhere.com"), fakeEventGridSubscriptionModel).ConfigureAwait(false);

            // assert
            A.CallTo(() => fakeApiService.DeleteAsync(A<HttpClient>.Ignored, A<Uri>.Ignored, A<EventGridSubscriptionModel>.Ignored)).MustHaveHappenedOnceExactly();
            A.Equals(result, expectedResult);
        }

        [Fact]
        public async Task ApiDataProcessorServiceDeleteReturnsExceptionForNoHttpClient()
        {
            // arrange
            var fakeEventGridSubscriptionModel = A.Fake<EventGridSubscriptionModel>();

            var apiDataProcessorService = new ApiDataProcessorService(fakeApiService);

            // act
            var exceptionResult = await Assert.ThrowsAsync<ArgumentNullException>(async () => await apiDataProcessorService.DeleteAsync(null, new Uri("https://somewhere.com"), fakeEventGridSubscriptionModel).ConfigureAwait(false)).ConfigureAwait(false);

            // assert
            A.CallTo(() => fakeApiService.DeleteAsync(A<HttpClient>.Ignored, A<Uri>.Ignored, A<string>.Ignored)).MustNotHaveHappened();
            Assert.Equal("Value cannot be null. (Parameter 'httpClient')", exceptionResult.Message);
        }
    }
}

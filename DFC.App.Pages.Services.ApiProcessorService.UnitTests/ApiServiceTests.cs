using DFC.App.Pages.Services.ApiProcessorService.UnitTests.FakeHttpHandlers;
using FakeItEasy;
using Microsoft.Extensions.Logging;
using System;
using System.Net;
using System.Net.Http;
using System.Net.Mime;
using System.Threading.Tasks;
using Xunit;

namespace DFC.App.Pages.Services.ApiProcessorService.UnitTests
{
    [Trait("Category", "CMS API Processor Service Unit Tests")]
    public class ApiServiceTests
    {
        private readonly ILogger<ApiService> logger;

        public ApiServiceTests()
        {
            logger = A.Fake<ILogger<ApiService>>();
        }

        [Fact]
        public async Task ApiServiceGetReturnsOkStatusCodeForValidUrl()
        {
            // arrange
            const HttpStatusCode expectedResult = HttpStatusCode.OK;
            const string expectedResponse = "Expected response string";
            var httpResponse = new HttpResponseMessage { StatusCode = expectedResult, Content = new StringContent(expectedResponse) };
            var fakeHttpRequestSender = A.Fake<IFakeHttpRequestSender>();
            var fakeHttpMessageHandler = new FakeHttpMessageHandler(fakeHttpRequestSender);
            var httpClient = new HttpClient(fakeHttpMessageHandler);
            var apiService = new ApiService(logger);
            var url = new Uri("https://www.somewhere.com", UriKind.Absolute);

            A.CallTo(() => fakeHttpRequestSender.Send(A<HttpRequestMessage>.Ignored)).Returns(httpResponse);

            // act
            var result = await apiService.GetAsync(httpClient, url, MediaTypeNames.Application.Json).ConfigureAwait(false);

            // assert
            A.CallTo(() => fakeHttpRequestSender.Send(A<HttpRequestMessage>.Ignored)).MustHaveHappenedOnceExactly();
            Assert.Equal(expectedResponse, result);

            httpResponse.Dispose();
            httpClient.Dispose();
            fakeHttpMessageHandler.Dispose();
        }

        [Fact]
        public async Task ApiServiceGetReturnsNotFoundStatusCode()
        {
            // arrange
            const HttpStatusCode expectedResult = HttpStatusCode.NotFound;
            string expectedResponse = string.Empty;
            var httpResponse = new HttpResponseMessage { StatusCode = expectedResult, Content = new StringContent(expectedResponse) };
            var fakeHttpRequestSender = A.Fake<IFakeHttpRequestSender>();
            var fakeHttpMessageHandler = new FakeHttpMessageHandler(fakeHttpRequestSender);
            var httpClient = new HttpClient(fakeHttpMessageHandler);
            var apiService = new ApiService(logger);
            var url = new Uri("https://www.somewhere.com", UriKind.Absolute);

            A.CallTo(() => fakeHttpRequestSender.Send(A<HttpRequestMessage>.Ignored)).Returns(httpResponse);

            // act
            var result = await apiService.GetAsync(httpClient, url, MediaTypeNames.Application.Json).ConfigureAwait(false);

            // assert
            A.CallTo(() => fakeHttpRequestSender.Send(A<HttpRequestMessage>.Ignored)).MustHaveHappenedOnceExactly();
            Assert.Null(result);

            httpResponse.Dispose();
            httpClient.Dispose();
            fakeHttpMessageHandler.Dispose();
        }

        [Fact]
        public async Task ApiServiceGetReturnsNoContentStatusCode()
        {
            // arrange
            const HttpStatusCode expectedResult = HttpStatusCode.NoContent;
            string expectedResponse = string.Empty;
            var httpResponse = new HttpResponseMessage { StatusCode = expectedResult, Content = new StringContent(expectedResponse) };
            var fakeHttpRequestSender = A.Fake<IFakeHttpRequestSender>();
            var fakeHttpMessageHandler = new FakeHttpMessageHandler(fakeHttpRequestSender);
            var httpClient = new HttpClient(fakeHttpMessageHandler);
            var apiService = new ApiService(logger);
            var url = new Uri("https://www.somewhere.com", UriKind.Absolute);

            A.CallTo(() => fakeHttpRequestSender.Send(A<HttpRequestMessage>.Ignored)).Returns(httpResponse);

            // act
            var result = await apiService.GetAsync(httpClient, url, MediaTypeNames.Application.Json).ConfigureAwait(false);

            // assert
            A.CallTo(() => fakeHttpRequestSender.Send(A<HttpRequestMessage>.Ignored)).MustHaveHappenedOnceExactly();
            Assert.Null(result);

            httpResponse.Dispose();
            httpClient.Dispose();
            fakeHttpMessageHandler.Dispose();
        }

        [Fact]
        public async Task ApiServiceGetReturnsExceptionResult()
        {
            // arrange
            var fakeHttpRequestSender = A.Fake<IFakeHttpRequestSender>();
            var fakeHttpMessageHandler = new FakeHttpMessageHandler(fakeHttpRequestSender);
            var httpClient = new HttpClient(fakeHttpMessageHandler);
            var apiService = new ApiService(logger);
            var url = new Uri("https://www.somewhere.com", UriKind.Absolute);

            A.CallTo(() => fakeHttpRequestSender.Send(A<HttpRequestMessage>.Ignored)).Throws(new ArgumentException("fake exception"));

            // act
            var result = await apiService.GetAsync(httpClient, url, MediaTypeNames.Application.Json).ConfigureAwait(false);

            // assert
            A.CallTo(() => fakeHttpRequestSender.Send(A<HttpRequestMessage>.Ignored)).MustHaveHappenedOnceExactly();
            Assert.Null(result);

            httpClient.Dispose();
            fakeHttpMessageHandler.Dispose();
        }

        [Fact]
        public async Task ApiServiceGetReturnsExceptionForNoHttpClient()
        {
            // arrange
            var fakeHttpRequestSender = A.Fake<IFakeHttpRequestSender>();
            var apiService = new ApiService(logger);
            var url = new Uri("https://www.somewhere.com", UriKind.Absolute);

            // act
            var exceptionResult = await Assert.ThrowsAsync<ArgumentNullException>(async () => await apiService.GetAsync(null, url, MediaTypeNames.Application.Json).ConfigureAwait(false)).ConfigureAwait(false);

            // assert
            A.CallTo(() => fakeHttpRequestSender.Send(A<HttpRequestMessage>.Ignored)).MustNotHaveHappened();
            Assert.Equal("Value cannot be null. (Parameter 'httpClient')", exceptionResult.Message);
        }

        [Fact]
        public async Task ApiServicePostReturnsOkStatusCode()
        {
            // arrange
            const HttpStatusCode expectedResult = HttpStatusCode.OK;
            var httpResponse = new HttpResponseMessage { StatusCode = expectedResult };
            var fakeHttpRequestSender = A.Fake<IFakeHttpRequestSender>();
            var fakeHttpMessageHandler = new FakeHttpMessageHandler(fakeHttpRequestSender);
            var httpClient = new HttpClient(fakeHttpMessageHandler);
            var apiService = new ApiService(logger);
            var url = new Uri("https://www.somewhere.com", UriKind.Absolute);

            A.CallTo(() => fakeHttpRequestSender.Send(A<HttpRequestMessage>.Ignored)).Returns(httpResponse);

            // act
            var result = await apiService.PostAsync(httpClient, url, MediaTypeNames.Application.Json).ConfigureAwait(false);

            // assert
            A.CallTo(() => fakeHttpRequestSender.Send(A<HttpRequestMessage>.Ignored)).MustHaveHappenedOnceExactly();
            Assert.Equal(expectedResult, result);

            httpResponse.Dispose();
            httpClient.Dispose();
            fakeHttpMessageHandler.Dispose();
        }

        [Fact]
        public async Task ApiServicePostReturnsNotFoundStatusCode()
        {
            // arrange
            const HttpStatusCode expectedResult = HttpStatusCode.NotFound;
            var httpResponse = new HttpResponseMessage { StatusCode = expectedResult };
            var fakeHttpRequestSender = A.Fake<IFakeHttpRequestSender>();
            var fakeHttpMessageHandler = new FakeHttpMessageHandler(fakeHttpRequestSender);
            var httpClient = new HttpClient(fakeHttpMessageHandler);
            var apiService = new ApiService(logger);
            var url = new Uri("https://www.somewhere.com", UriKind.Absolute);

            A.CallTo(() => fakeHttpRequestSender.Send(A<HttpRequestMessage>.Ignored)).Returns(httpResponse);

            // act
            var result = await apiService.PostAsync(httpClient, url, MediaTypeNames.Application.Json).ConfigureAwait(false);

            // assert
            A.CallTo(() => fakeHttpRequestSender.Send(A<HttpRequestMessage>.Ignored)).MustHaveHappenedOnceExactly();
            Assert.Equal(expectedResult, result);

            httpResponse.Dispose();
            httpClient.Dispose();
            fakeHttpMessageHandler.Dispose();
        }

        [Fact]
        public async Task ApiServicePostReturnsExceptionResult()
        {
            // arrange
            var fakeHttpRequestSender = A.Fake<IFakeHttpRequestSender>();
            var fakeHttpMessageHandler = new FakeHttpMessageHandler(fakeHttpRequestSender);
            var httpClient = new HttpClient(fakeHttpMessageHandler);
            var apiService = new ApiService(logger);
            var url = new Uri("https://www.somewhere.com", UriKind.Absolute);

            A.CallTo(() => fakeHttpRequestSender.Send(A<HttpRequestMessage>.Ignored)).Throws(new ArgumentException("fake exception"));

            // act
            var result = await apiService.PostAsync(httpClient, url, MediaTypeNames.Application.Json).ConfigureAwait(false);

            // assert
            A.CallTo(() => fakeHttpRequestSender.Send(A<HttpRequestMessage>.Ignored)).MustHaveHappenedOnceExactly();
            Assert.Equal(HttpStatusCode.BadRequest, result);

            httpClient.Dispose();
            fakeHttpMessageHandler.Dispose();
        }

        [Fact]
        public async Task ApiServicePostReturnsExceptionForNoHttpClient()
        {
            // arrange
            var fakeHttpRequestSender = A.Fake<IFakeHttpRequestSender>();
            var apiService = new ApiService(logger);
            var url = new Uri("https://www.somewhere.com", UriKind.Absolute);

            // act
            var exceptionResult = await Assert.ThrowsAsync<ArgumentNullException>(async () => await apiService.PostAsync(null, url, MediaTypeNames.Application.Json).ConfigureAwait(false)).ConfigureAwait(false);

            // assert
            A.CallTo(() => fakeHttpRequestSender.Send(A<HttpRequestMessage>.Ignored)).MustNotHaveHappened();
            Assert.Equal("Value cannot be null. (Parameter 'httpClient')", exceptionResult.Message);
        }

        [Fact]
        public async Task ApiServiceDeleteReturnsOkStatusCode()
        {
            // arrange
            const HttpStatusCode expectedResult = HttpStatusCode.OK;
            var httpResponse = new HttpResponseMessage { StatusCode = expectedResult };
            var fakeHttpRequestSender = A.Fake<IFakeHttpRequestSender>();
            var fakeHttpMessageHandler = new FakeHttpMessageHandler(fakeHttpRequestSender);
            var httpClient = new HttpClient(fakeHttpMessageHandler);
            var apiService = new ApiService(logger);
            var url = new Uri("https://www.somewhere.com", UriKind.Absolute);

            A.CallTo(() => fakeHttpRequestSender.Send(A<HttpRequestMessage>.Ignored)).Returns(httpResponse);

            // act
            var result = await apiService.DeleteAsync(httpClient, url).ConfigureAwait(false);

            // assert
            A.CallTo(() => fakeHttpRequestSender.Send(A<HttpRequestMessage>.Ignored)).MustHaveHappenedOnceExactly();
            Assert.Equal(expectedResult, result);

            httpResponse.Dispose();
            httpClient.Dispose();
            fakeHttpMessageHandler.Dispose();
        }

        [Fact]
        public async Task ApiServiceDeleteReturnsNotFoundStatusCode()
        {
            // arrange
            const HttpStatusCode expectedResult = HttpStatusCode.NotFound;
            var httpResponse = new HttpResponseMessage { StatusCode = expectedResult };
            var fakeHttpRequestSender = A.Fake<IFakeHttpRequestSender>();
            var fakeHttpMessageHandler = new FakeHttpMessageHandler(fakeHttpRequestSender);
            var httpClient = new HttpClient(fakeHttpMessageHandler);
            var apiService = new ApiService(logger);
            var url = new Uri("https://www.somewhere.com", UriKind.Absolute);

            A.CallTo(() => fakeHttpRequestSender.Send(A<HttpRequestMessage>.Ignored)).Returns(httpResponse);

            // act
            var result = await apiService.DeleteAsync(httpClient, url).ConfigureAwait(false);

            // assert
            A.CallTo(() => fakeHttpRequestSender.Send(A<HttpRequestMessage>.Ignored)).MustHaveHappenedOnceExactly();
            Assert.Equal(expectedResult, result);

            httpResponse.Dispose();
            httpClient.Dispose();
            fakeHttpMessageHandler.Dispose();
        }

        [Fact]
        public async Task ApiServiceDeleteReturnsExceptionResult()
        {
            // arrange
            var fakeHttpRequestSender = A.Fake<IFakeHttpRequestSender>();
            var fakeHttpMessageHandler = new FakeHttpMessageHandler(fakeHttpRequestSender);
            var httpClient = new HttpClient(fakeHttpMessageHandler);
            var apiService = new ApiService(logger);
            var url = new Uri("https://www.somewhere.com", UriKind.Absolute);

            A.CallTo(() => fakeHttpRequestSender.Send(A<HttpRequestMessage>.Ignored)).Throws(new ArgumentException("fake exception"));

            // act
            var result = await apiService.DeleteAsync(httpClient, url).ConfigureAwait(false);

            // assert
            A.CallTo(() => fakeHttpRequestSender.Send(A<HttpRequestMessage>.Ignored)).MustHaveHappenedOnceExactly();
            Assert.Equal(HttpStatusCode.BadRequest, result);

            httpClient.Dispose();
            fakeHttpMessageHandler.Dispose();
        }

        [Fact]
        public async Task ApiServiceDeleteReturnsExceptionForNoHttpClient()
        {
            // arrange
            var fakeHttpRequestSender = A.Fake<IFakeHttpRequestSender>();
            var apiService = new ApiService(logger);
            var url = new Uri("https://www.somewhere.com", UriKind.Absolute);

            // act
            var exceptionResult = await Assert.ThrowsAsync<ArgumentNullException>(async () => await apiService.DeleteAsync(null, url).ConfigureAwait(false)).ConfigureAwait(false);

            // assert
            A.CallTo(() => fakeHttpRequestSender.Send(A<HttpRequestMessage>.Ignored)).MustNotHaveHappened();
            Assert.Equal("Value cannot be null. (Parameter 'httpClient')", exceptionResult.Message);
        }
    }
}

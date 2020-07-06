using DFC.App.Pages.Data.Contracts;
using DFC.App.Pages.Data.Models.ClientOptions;
using DFC.App.Pages.Data.Models.SubscriptionModels;
using FakeItEasy;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Xunit;

namespace DFC.App.Pages.Services.EventProcessorService.UnitTests
{
    [Trait("Category", "Event Message Service Unit Tests")]
    public class EventGridSubscriptionServiceTests
    {
        private readonly ILogger<EventGridSubscriptionService> fakeLogger = A.Fake<ILogger<EventGridSubscriptionService>>();
        private readonly IApiDataProcessorService fakeApiDataProcessorService = A.Fake<IApiDataProcessorService>();
        private readonly EventGridSubscriptionClientOptions eventGridSubscriptionClientOptions = new EventGridSubscriptionClientOptions
        {
            BaseAddress = new Uri("https://somewhere.com", UriKind.Absolute),
            ApiKey = null,
            Endpoint = "endpoint/",
        };

        private readonly EventGridSubscriptionModel eventGridSubscriptionModel = new EventGridSubscriptionModel
        {
            Name = "unit.tests",
            Endpoint = "endpoint/",
            Filter = new SubscriptionFilterModel
            {
                BeginsWith = "/content/page/",
                IncludeEventTypes = new List<string> { "published", "unpublished", "deleted" },
            },
        };

        [Theory]
        [InlineData(HttpStatusCode.Created)]
        [InlineData(HttpStatusCode.AlreadyReported)]
        public async Task EventGridSubscriptionServiceCreateReturnsStatusCode(HttpStatusCode expectedResult)
        {
            // arrange
            var fakeHttpClient = A.Fake<HttpClient>();

            A.CallTo(() => fakeApiDataProcessorService.PostAsync(A<HttpClient>.Ignored, A<Uri>.Ignored, eventGridSubscriptionModel)).Returns(expectedResult);

            var eventGridSubscriptionService = new EventGridSubscriptionService(fakeLogger, eventGridSubscriptionClientOptions, eventGridSubscriptionModel, fakeApiDataProcessorService, fakeHttpClient);

            // act
            var result = await eventGridSubscriptionService.CreateAsync().ConfigureAwait(false);

            // assert
            A.CallTo(() => fakeApiDataProcessorService.PostAsync(A<HttpClient>.Ignored, A<Uri>.Ignored, A<EventGridSubscriptionModel>.Ignored)).MustHaveHappenedOnceExactly();
            A.Equals(expectedResult, result);
        }

        [Fact]
        public async Task EventGridSubscriptionServiceCreateReturnsContinue()
        {
            // arrange
            var expectedResult = HttpStatusCode.Continue;
            var fakeHttpClient = A.Fake<HttpClient>();

            eventGridSubscriptionClientOptions.BaseAddress = null;

            var eventGridSubscriptionService = new EventGridSubscriptionService(fakeLogger, eventGridSubscriptionClientOptions, eventGridSubscriptionModel, fakeApiDataProcessorService, fakeHttpClient);

            // act
            var result = await eventGridSubscriptionService.CreateAsync().ConfigureAwait(false);

            // assert
            A.CallTo(() => fakeApiDataProcessorService.PostAsync(A<HttpClient>.Ignored, A<Uri>.Ignored, A<EventGridSubscriptionModel>.Ignored)).MustNotHaveHappened();
            A.Equals(expectedResult, result);
        }

        [Theory]
        [InlineData(HttpStatusCode.Created)]
        [InlineData(HttpStatusCode.AlreadyReported)]
        public async Task EventGridSubscriptionServiceDeleteReturnsStatusCode(HttpStatusCode expectedResult)
        {
            // arrange
            var fakeHttpClient = A.Fake<HttpClient>();

            A.CallTo(() => fakeApiDataProcessorService.DeleteAsync(A<HttpClient>.Ignored, A<Uri>.Ignored)).Returns(expectedResult);

            var eventGridSubscriptionService = new EventGridSubscriptionService(fakeLogger, eventGridSubscriptionClientOptions, eventGridSubscriptionModel, fakeApiDataProcessorService, fakeHttpClient);

            // act
            var result = await eventGridSubscriptionService.DeleteAsync().ConfigureAwait(false);

            // assert
            A.CallTo(() => fakeApiDataProcessorService.DeleteAsync(A<HttpClient>.Ignored, A<Uri>.Ignored)).MustHaveHappenedOnceExactly();
            A.Equals(expectedResult, result);
        }

        [Fact]
        public async Task EventGridSubscriptionServiceDeleteReturnsContinue()
        {
            // arrange
            var expectedResult = HttpStatusCode.Continue;
            var fakeHttpClient = A.Fake<HttpClient>();

            eventGridSubscriptionClientOptions.BaseAddress = null;

            var eventGridSubscriptionService = new EventGridSubscriptionService(fakeLogger, eventGridSubscriptionClientOptions, eventGridSubscriptionModel, fakeApiDataProcessorService, fakeHttpClient);

            // act
            var result = await eventGridSubscriptionService.DeleteAsync().ConfigureAwait(false);

            // assert
            A.CallTo(() => fakeApiDataProcessorService.DeleteAsync(A<HttpClient>.Ignored, A<Uri>.Ignored)).MustNotHaveHappened();
            A.Equals(expectedResult, result);
        }
    }
}

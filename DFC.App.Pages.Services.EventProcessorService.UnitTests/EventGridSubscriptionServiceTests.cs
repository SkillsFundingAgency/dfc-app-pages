using AutoMapper;
using DFC.App.Pages.Data.Contracts;
using DFC.App.Pages.Data.Models;
using FakeItEasy;
using Microsoft.Extensions.Logging;
using System;
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
        private readonly IMapper fakeMapper = A.Fake<IMapper>();
        private readonly IApiDataProcessorService fakeApiDataProcessorService = A.Fake<IApiDataProcessorService>();
        private readonly EventGridSubscriptionClientOptions eventGridSubscriptionClientOptions = new EventGridSubscriptionClientOptions { BaseAddress = new Uri("https://somewhere.com", UriKind.Absolute) };

        [Theory]
        [InlineData(HttpStatusCode.Created)]
        [InlineData(HttpStatusCode.AlreadyReported)]
        public async Task EventGridSubscriptionServiceCreateReturnsStatusCode(HttpStatusCode expectedResult)
        {
            // arrange
            var fakeEventGridSubscriptionModel = A.Fake<EventGridSubscriptionModel>();
            var fakeHttpClient = A.Fake<HttpClient>();

            A.CallTo(() => fakeMapper.Map<EventGridSubscriptionModel>(A<EventGridSubscriptionClientOptions>.Ignored)).Returns(fakeEventGridSubscriptionModel);
            A.CallTo(() => fakeApiDataProcessorService.PostAsync(A<HttpClient>.Ignored, A<Uri>.Ignored, fakeEventGridSubscriptionModel)).Returns(expectedResult);

            var eventMessageService = new EventGridSubscriptionService(fakeLogger, fakeMapper, eventGridSubscriptionClientOptions, fakeApiDataProcessorService, fakeHttpClient);

            // act
            var result = await eventMessageService.CreateAsync(Guid.NewGuid()).ConfigureAwait(false);

            // assert
            A.CallTo(() => fakeMapper.Map<EventGridSubscriptionModel>(A<EventGridSubscriptionClientOptions>.Ignored)).MustHaveHappenedOnceExactly();
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

            var eventMessageService = new EventGridSubscriptionService(fakeLogger, fakeMapper, eventGridSubscriptionClientOptions, fakeApiDataProcessorService, fakeHttpClient);

            // act
            var result = await eventMessageService.CreateAsync(Guid.NewGuid()).ConfigureAwait(false);

            // assert
            A.CallTo(() => fakeMapper.Map<EventGridSubscriptionModel>(A<EventGridSubscriptionClientOptions>.Ignored)).MustNotHaveHappened();
            A.CallTo(() => fakeApiDataProcessorService.PostAsync(A<HttpClient>.Ignored, A<Uri>.Ignored, A<EventGridSubscriptionModel>.Ignored)).MustNotHaveHappened();
            A.Equals(expectedResult, result);
        }
    }
}

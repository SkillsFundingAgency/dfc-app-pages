using DFC.App.Pages.Data.Contracts;
using DFC.App.Pages.Data.Enums;
using DFC.App.Pages.Data.Models;
using DFC.App.Pages.Data.Models.ClientOptions;
using FakeItEasy;
using Microsoft.Azure.EventGrid.Models;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;
using Xunit;

namespace DFC.App.Pages.Services.EventProcessorService.UnitTests
{
    [Trait("Category", "Event Grid Service Unit Tests")]
    public class EventGridServiceTests
    {
        private readonly ILogger<EventGridService> fakeLogger = A.Fake<ILogger<EventGridService>>();
        private readonly IEventGridClientService fakeEventGridClientService = A.Fake<IEventGridClientService>();
        private readonly EventGridPublishClientOptions eventGridPublishClientOptions = new EventGridPublishClientOptions
        {
            TopicEndpoint = "http://somewhere.com",
            TopicKey = "a topic key",
            SubjectPrefix = "/a-subject-prefix",
        };

        [Fact]
        public async Task EventGridServiceCompareAndSendEventAsyncReturnsSuccessForNoDifferences()
        {
            // arrange
            var existingContentPageModel = BuildValidContentPageModel();
            var updatedContentPageModel = BuildValidContentPageModel();
            var eventGridService = new EventGridService(fakeLogger, fakeEventGridClientService, eventGridPublishClientOptions);

            // act
            await eventGridService.CompareAndSendEventAsync(existingContentPageModel, updatedContentPageModel).ConfigureAwait(false);

            // assert
            A.CallTo(() => fakeEventGridClientService.SendEventAsync(A<List<EventGridEvent>>.Ignored, A<string>.Ignored, A<string>.Ignored, A<string>.Ignored)).MustNotHaveHappened();
        }

        [Fact]
        public async Task EventGridServiceCompareAndSendEventAsyncReturnsSuccessForDifferentPageLocation()
        {
            // arrange
            var existingContentPageModel = BuildValidContentPageModel();
            var updatedContentPageModel = BuildValidContentPageModel();
            var eventGridService = new EventGridService(fakeLogger, fakeEventGridClientService, eventGridPublishClientOptions);

            updatedContentPageModel.PageLocation = existingContentPageModel.PageLocation + ".with-a-difference";

            // act
            await eventGridService.CompareAndSendEventAsync(existingContentPageModel, updatedContentPageModel).ConfigureAwait(false);

            // assert
            A.CallTo(() => fakeEventGridClientService.SendEventAsync(A<List<EventGridEvent>>.Ignored, A<string>.Ignored, A<string>.Ignored, A<string>.Ignored)).MustHaveHappenedOnceExactly();
        }

        [Fact]
        public async Task EventGridServiceCompareAndSendEventAsyncReturnsSuccessForDifferentRedirectLocations()
        {
            // arrange
            var existingContentPageModel = BuildValidContentPageModel();
            var updatedContentPageModel = BuildValidContentPageModel();
            var eventGridService = new EventGridService(fakeLogger, fakeEventGridClientService, eventGridPublishClientOptions);

            updatedContentPageModel.RedirectLocations = new List<string> { "/location-3", "/location-4" };

            // act
            await eventGridService.CompareAndSendEventAsync(existingContentPageModel, updatedContentPageModel).ConfigureAwait(false);

            // assert
            A.CallTo(() => fakeEventGridClientService.SendEventAsync(A<List<EventGridEvent>>.Ignored, A<string>.Ignored, A<string>.Ignored, A<string>.Ignored)).MustHaveHappenedOnceExactly();
        }

        [Fact]
        public async Task EventGridServiceCompareAndSendEventAsyncReturnsSuccessForNullRedirectLocations1()
        {
            // arrange
            var existingContentPageModel = BuildValidContentPageModel();
            var updatedContentPageModel = BuildValidContentPageModel();
            var eventGridService = new EventGridService(fakeLogger, fakeEventGridClientService, eventGridPublishClientOptions);

            updatedContentPageModel.RedirectLocations = null;

            // act
            await eventGridService.CompareAndSendEventAsync(existingContentPageModel, updatedContentPageModel).ConfigureAwait(false);

            // assert
            A.CallTo(() => fakeEventGridClientService.SendEventAsync(A<List<EventGridEvent>>.Ignored, A<string>.Ignored, A<string>.Ignored, A<string>.Ignored)).MustHaveHappenedOnceExactly();
        }

        [Fact]
        public async Task EventGridServiceCompareAndSendEventAsyncReturnsSuccessForNullRedirectLocations2()
        {
            // arrange
            var existingContentPageModel = BuildValidContentPageModel();
            var updatedContentPageModel = BuildValidContentPageModel();
            var eventGridService = new EventGridService(fakeLogger, fakeEventGridClientService, eventGridPublishClientOptions);

            existingContentPageModel.RedirectLocations = null;

            // act
            await eventGridService.CompareAndSendEventAsync(existingContentPageModel, updatedContentPageModel).ConfigureAwait(false);

            // assert
            A.CallTo(() => fakeEventGridClientService.SendEventAsync(A<List<EventGridEvent>>.Ignored, A<string>.Ignored, A<string>.Ignored, A<string>.Ignored)).MustHaveHappenedOnceExactly();
        }

        [Fact]
        public async Task EventGridServiceCompareAndSendEventAsyncReturnsSuccessForNullExistingContentPageModel()
        {
            // arrange
            ContentPageModel? existingContentPageModel = null;
            var updatedContentPageModel = BuildValidContentPageModel();
            var eventGridService = new EventGridService(fakeLogger, fakeEventGridClientService, eventGridPublishClientOptions);

            // act
            await eventGridService.CompareAndSendEventAsync(existingContentPageModel, updatedContentPageModel).ConfigureAwait(false);

            // assert
            A.CallTo(() => fakeEventGridClientService.SendEventAsync(A<List<EventGridEvent>>.Ignored, A<string>.Ignored, A<string>.Ignored, A<string>.Ignored)).MustHaveHappenedOnceExactly();
        }

        [Fact]
        public async Task EventGridServiceCompareAndSendEventAsyncRaisesExceptionWhenNullContantPageModel()
        {
            // arrange
            var existingContentPageModel = BuildValidContentPageModel();
            ContentPageModel? updatedContentPageModel = null;
            var eventGridService = new EventGridService(fakeLogger, fakeEventGridClientService, eventGridPublishClientOptions);

            // act
            var exceptionResult = await Assert.ThrowsAsync<ArgumentNullException>(async () => await eventGridService.CompareAndSendEventAsync(existingContentPageModel, updatedContentPageModel).ConfigureAwait(false)).ConfigureAwait(false);

            // assert
            A.CallTo(() => fakeEventGridClientService.SendEventAsync(A<List<EventGridEvent>>.Ignored, A<string>.Ignored, A<string>.Ignored, A<string>.Ignored)).MustNotHaveHappened();
            Assert.Equal("Value cannot be null. (Parameter 'updatedContentPageModel')", exceptionResult.Message);
        }

        [Fact]
        public async Task EventGridServiceSendEventAsyncReturnsSuccess()
        {
            // arrange
            var contentPageModel = BuildValidContentPageModel();
            var eventGridService = new EventGridService(fakeLogger, fakeEventGridClientService, eventGridPublishClientOptions);

            // act
            await eventGridService.SendEventAsync(WebhookCacheOperation.CreateOrUpdate, contentPageModel).ConfigureAwait(false);

            // assert
            A.CallTo(() => fakeEventGridClientService.SendEventAsync(A<List<EventGridEvent>>.Ignored, A<string>.Ignored, A<string>.Ignored, A<string>.Ignored)).MustHaveHappenedOnceExactly();
        }

        [Fact]
        public async Task EventGridServiceSendEventAsyncRaisesExceptionWhenNullContentPageModel()
        {
            // arrange
            ContentPageModel? contentPageModel = null;
            var eventGridService = new EventGridService(fakeLogger, fakeEventGridClientService, eventGridPublishClientOptions);

            // act
            var exceptionResult = await Assert.ThrowsAsync<ArgumentNullException>(async () => await eventGridService.SendEventAsync(WebhookCacheOperation.CreateOrUpdate, contentPageModel).ConfigureAwait(false)).ConfigureAwait(false);

            // assert
            A.CallTo(() => fakeEventGridClientService.SendEventAsync(A<List<EventGridEvent>>.Ignored, A<string>.Ignored, A<string>.Ignored, A<string>.Ignored)).MustNotHaveHappened();
            Assert.Equal("Value cannot be null. (Parameter 'updatedContentPageModel')", exceptionResult.Message);
        }

        [Fact]
        public async Task EventGridServiceSendEventAsyncRaisesExceptionWhenNullTopicEndpoint()
        {
            // arrange
            var contentPageModel = BuildValidContentPageModel();
            var eventGridService = new EventGridService(fakeLogger, fakeEventGridClientService, eventGridPublishClientOptions);

            eventGridPublishClientOptions.TopicEndpoint = string.Empty;

            // act
            var exceptionResult = await Assert.ThrowsAsync<DataException>(async () => await eventGridService.SendEventAsync(WebhookCacheOperation.CreateOrUpdate, contentPageModel).ConfigureAwait(false)).ConfigureAwait(false);

            // assert
            A.CallTo(() => fakeEventGridClientService.SendEventAsync(A<List<EventGridEvent>>.Ignored, A<string>.Ignored, A<string>.Ignored, A<string>.Ignored)).MustNotHaveHappened();
            Assert.Equal("EventGridPublishClientOptions is missing a value for: TopicEndpoint", exceptionResult.Message);
        }

        [Fact]
        public async Task EventGridServiceSendEventAsyncRaisesExceptionWhenNullTopicKey()
        {
            // arrange
            var contentPageModel = BuildValidContentPageModel();
            var eventGridService = new EventGridService(fakeLogger, fakeEventGridClientService, eventGridPublishClientOptions);

            eventGridPublishClientOptions.TopicKey = string.Empty;

            // act
            var exceptionResult = await Assert.ThrowsAsync<DataException>(async () => await eventGridService.SendEventAsync(WebhookCacheOperation.CreateOrUpdate, contentPageModel).ConfigureAwait(false)).ConfigureAwait(false);

            // assert
            A.CallTo(() => fakeEventGridClientService.SendEventAsync(A<List<EventGridEvent>>.Ignored, A<string>.Ignored, A<string>.Ignored, A<string>.Ignored)).MustNotHaveHappened();
            Assert.Equal("EventGridPublishClientOptions is missing a value for: TopicKey", exceptionResult.Message);
        }

        [Fact]
        public async Task EventGridServiceSendEventAsyncRaisesExceptionWhenNullSubjectPrefix()
        {
            // arrange
            var contentPageModel = BuildValidContentPageModel();
            var eventGridService = new EventGridService(fakeLogger, fakeEventGridClientService, eventGridPublishClientOptions);

            eventGridPublishClientOptions.SubjectPrefix = string.Empty;

            // act
            var exceptionResult = await Assert.ThrowsAsync<DataException>(async () => await eventGridService.SendEventAsync(WebhookCacheOperation.CreateOrUpdate, contentPageModel).ConfigureAwait(false)).ConfigureAwait(false);

            // assert
            A.CallTo(() => fakeEventGridClientService.SendEventAsync(A<List<EventGridEvent>>.Ignored, A<string>.Ignored, A<string>.Ignored, A<string>.Ignored)).MustNotHaveHappened();
            Assert.Equal("EventGridPublishClientOptions is missing a value for: SubjectPrefix", exceptionResult.Message);
        }

        private ContentPageModel BuildValidContentPageModel()
        {
            return new ContentPageModel
            {
                Id = Guid.NewGuid(),
                CanonicalName = "a-name",
                PageLocation = "/page-location",
                RedirectLocations = new List<string> { "/location-1", "/location-2" },
                Version = Guid.NewGuid(),
                Url = new Uri("https://somewhere.com", UriKind.Absolute),
            };
        }
    }
}

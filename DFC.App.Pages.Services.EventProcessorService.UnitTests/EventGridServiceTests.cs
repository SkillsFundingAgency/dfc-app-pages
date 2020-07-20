using DFC.App.Pages.Data.Contracts;
using DFC.App.Pages.Data.Enums;
using DFC.App.Pages.Data.Models;
using DFC.App.Pages.Data.Models.ClientOptions;
using FakeItEasy;
using Microsoft.Azure.EventGrid.Models;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
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
            ApiEndpoint = new Uri("http://someendpoint", UriKind.Absolute),
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
        public async Task EventGridServiceCompareAndSendEventAsyncReturnsSuccessForDifferences()
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
        public void EventGridServiceContainsDifferencesReturnsNoDifferences()
        {
            // arrange
            const bool expectedResult = false;
            var existingContentPageModel = BuildValidContentPageModel();
            var updatedContentPageModel = BuildValidContentPageModel();

            // act
            var result = EventGridService.ContainsDifferences(existingContentPageModel, updatedContentPageModel);

            // assert
            Assert.Equal(expectedResult, result);
        }

        [Fact]
        public void EventGridServiceContainsDifferencesReturnsDifferencesForDifferentPageLocation()
        {
            // arrange
            const bool expectedResult = true;
            var existingContentPageModel = BuildValidContentPageModel();
            var updatedContentPageModel = BuildValidContentPageModel();

            updatedContentPageModel.PageLocation = existingContentPageModel.PageLocation + ".with-a-difference";

            // act
            var result = EventGridService.ContainsDifferences(existingContentPageModel, updatedContentPageModel);

            // assert
            Assert.Equal(expectedResult, result);
        }

        [Fact]
        public void EventGridServiceContainsDifferencesReturnsDifferencesForDifferentRedirectLocations()
        {
            // arrange
            const bool expectedResult = true;
            var existingContentPageModel = BuildValidContentPageModel();
            var updatedContentPageModel = BuildValidContentPageModel();

            updatedContentPageModel.RedirectLocations = new List<string> { "/location-3", "/location-4" };

            // act
            var result = EventGridService.ContainsDifferences(existingContentPageModel, updatedContentPageModel);

            // assert
            Assert.Equal(expectedResult, result);
        }

        [Fact]
        public void EventGridServiceContainsDifferencesReturnsDifferencesForNullRedirectLocations1()
        {
            // arrange
            const bool expectedResult = true;
            var existingContentPageModel = BuildValidContentPageModel();
            var updatedContentPageModel = BuildValidContentPageModel();

            updatedContentPageModel.RedirectLocations = null;

            // act
            var result = EventGridService.ContainsDifferences(existingContentPageModel, updatedContentPageModel);

            // assert
            Assert.Equal(expectedResult, result);
        }

        [Fact]
        public void EventGridServiceContainsDifferencesReturnsDifferencesForNullRedirectLocations2()
        {
            // arrange
            const bool expectedResult = true;
            var existingContentPageModel = BuildValidContentPageModel();
            var updatedContentPageModel = BuildValidContentPageModel();

            existingContentPageModel.RedirectLocations = null;

            // act
            var result = EventGridService.ContainsDifferences(existingContentPageModel, updatedContentPageModel);

            // assert
            Assert.Equal(expectedResult, result);
        }

        [Fact]
        public void EventGridServiceContainsDifferencesReturnsDifferencesForNullExistingContentPageModel()
        {
            // arrange
            const bool expectedResult = true;
            ContentPageModel? existingContentPageModel = null;
            var updatedContentPageModel = BuildValidContentPageModel();

            // act
            var result = EventGridService.ContainsDifferences(existingContentPageModel, updatedContentPageModel);

            // assert
            Assert.Equal(expectedResult, result);
        }

        [Fact]
        public void EventGridServiceContainsDifferencesRaisesExceptionWhenNullUpdatedContantPageModel()
        {
            // arrange
            var existingContentPageModel = BuildValidContentPageModel();
            ContentPageModel? updatedContentPageModel = null;

            // act
            var exceptionResult = Assert.Throws<ArgumentNullException>(() => EventGridService.ContainsDifferences(existingContentPageModel, updatedContentPageModel));

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
        public async Task EventGridServiceSendEventAsyncRaisesExceptionWhenInvalidEventGridPublishClientOptions()
        {
            // arrange
            var contentPageModel = BuildValidContentPageModel();
            var eventGridService = new EventGridService(fakeLogger, fakeEventGridClientService, eventGridPublishClientOptions);

            eventGridPublishClientOptions.TopicEndpoint = string.Empty;

            // act
            await eventGridService.SendEventAsync(WebhookCacheOperation.CreateOrUpdate, contentPageModel).ConfigureAwait(false);

            // assert
            A.CallTo(() => fakeEventGridClientService.SendEventAsync(A<List<EventGridEvent>>.Ignored, A<string>.Ignored, A<string>.Ignored, A<string>.Ignored)).MustNotHaveHappened();
        }

        [Fact]
        public void EventGridServiceIsValidEventGridPublishClientOptionsReturnsSuccess()
        {
            // arrange
            const bool expectedResult = true;

            // act
            var result = EventGridService.IsValidEventGridPublishClientOptions(fakeLogger, eventGridPublishClientOptions);

            // assert
            Assert.Equal(expectedResult, result);
        }

        [Fact]
        public void EventGridServiceIsValidEventGridPublishClientOptionsReturnsFalseWhenNullTopicEndpoint()
        {
            // arrange
            const bool expectedResult = false;

            eventGridPublishClientOptions.TopicEndpoint = string.Empty;

            // act
            var result = EventGridService.IsValidEventGridPublishClientOptions(fakeLogger, eventGridPublishClientOptions);

            // assert
            Assert.Equal(expectedResult, result);
        }

        [Fact]
        public void EventGridServiceIsValidEventGridPublishClientOptionsReturnsFalsenWhenNullTopicKey()
        {
            // arrange
            const bool expectedResult = false;

            eventGridPublishClientOptions.TopicKey = string.Empty;

            // act
            var result = EventGridService.IsValidEventGridPublishClientOptions(fakeLogger, eventGridPublishClientOptions);

            // assert
            Assert.Equal(expectedResult, result);
        }

        [Fact]
        public void EventGridServiceIsValidEventGridPublishClientOptionsReturnsFalseWhenNullSubjectPrefix()
        {
            // arrange
            const bool expectedResult = false;

            eventGridPublishClientOptions.SubjectPrefix = string.Empty;

            // act
            var result = EventGridService.IsValidEventGridPublishClientOptions(fakeLogger, eventGridPublishClientOptions);

            // assert
            Assert.Equal(expectedResult, result);
        }

        [Fact]
        public void EventGridServiceIsValidEventGridPublishClientOptionsReturnsFalseWhenNullApiEndpoint()
        {
            // arrange
            const bool expectedResult = false;

            eventGridPublishClientOptions.ApiEndpoint = null;

            // act
            var result = EventGridService.IsValidEventGridPublishClientOptions(fakeLogger, eventGridPublishClientOptions);

            // assert
            Assert.Equal(expectedResult, result);
        }

        [Fact]
        public void EventGridServiceIsValidEventGridPublishClientOptionsRaisesExceptionWhenNullEventGridPublishClientOptions()
        {
            // arrange

            // act
            var exceptionResult = Assert.Throws<ArgumentNullException>(() => EventGridService.IsValidEventGridPublishClientOptions(fakeLogger, null));

            // assert
            A.CallTo(() => fakeEventGridClientService.SendEventAsync(A<List<EventGridEvent>>.Ignored, A<string>.Ignored, A<string>.Ignored, A<string>.Ignored)).MustNotHaveHappened();
            Assert.Equal("Value cannot be null. (Parameter 'eventGridPublishClientOptions')", exceptionResult.Message);
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

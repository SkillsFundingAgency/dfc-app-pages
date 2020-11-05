using DFC.App.Pages.Data.Enums;
using DFC.App.Pages.Data.Models;
using FakeItEasy;
using System;
using System.Net;
using System.Threading.Tasks;
using Xunit;

namespace DFC.App.Pages.Services.CacheContentService.UnitTests.WebhookContentProcessorTests
{
    [Trait("Category", "WebhookContentProcessor - DeleteContent Unit Tests")]
    public class WebhookContentProcessorDeleteContentTests : BaseWebhookContentProcessor
    {
        [Fact]
        public async Task WebhookContentProcessorDeleteContentAsyncReturnsSuccess()
        {
            // Arrange
            const HttpStatusCode expectedResponse = HttpStatusCode.OK;
            var expectedValidContentPageModel = BuildValidContentPageModel();
            var service = BuildWebhookContentProcessor();

            A.CallTo(() => FakeContentPageService.GetByIdAsync(A<Guid>.Ignored, A<string>.Ignored)).Returns(expectedValidContentPageModel);
            A.CallTo(() => FakeEventMessageService.DeleteAsync(A<Guid>.Ignored)).Returns(expectedResponse);
            A.CallTo(() => FakeContentCacheService.Remove(A<Guid>.Ignored));

            // Act
            var result = await service.DeleteContentAsync(ContentIdForDelete).ConfigureAwait(false);

            // Assert
            A.CallTo(() => FakeContentPageService.GetByIdAsync(A<Guid>.Ignored, A<string>.Ignored)).MustHaveHappenedOnceExactly();
            A.CallTo(() => FakeEventMessageService.DeleteAsync(A<Guid>.Ignored)).MustHaveHappenedOnceExactly();
            A.CallTo(() => FakeEventGridService.SendEventAsync(A<WebhookCacheOperation>.Ignored, A<ContentPageModel>.Ignored)).MustHaveHappenedOnceExactly();
            A.CallTo(() => FakeContentCacheService.Remove(A<Guid>.Ignored)).MustHaveHappenedOnceExactly();

            Assert.Equal(expectedResponse, result);
        }

        [Fact]
        public async Task WebhookContentProcessorDeleteContentAsyncForCreateReturnsNoContent()
        {
            // Arrange
            const HttpStatusCode expectedResponse = HttpStatusCode.NoContent;
            ContentPageModel? nullContentPageModel = null;
            var service = BuildWebhookContentProcessor();

            A.CallTo(() => FakeContentPageService.GetByIdAsync(A<Guid>.Ignored, A<string>.Ignored)).Returns(nullContentPageModel);
            A.CallTo(() => FakeEventMessageService.DeleteAsync(A<Guid>.Ignored)).Returns(expectedResponse);
            A.CallTo(() => FakeContentCacheService.Remove(A<Guid>.Ignored));

            // Act
            var result = await service.DeleteContentAsync(ContentIdForDelete).ConfigureAwait(false);

            // Assert
            A.CallTo(() => FakeContentPageService.GetByIdAsync(A<Guid>.Ignored, A<string>.Ignored)).MustHaveHappenedOnceExactly();
            A.CallTo(() => FakeEventMessageService.DeleteAsync(A<Guid>.Ignored)).MustHaveHappenedOnceExactly();
            A.CallTo(() => FakeEventGridService.SendEventAsync(A<WebhookCacheOperation>.Ignored, A<ContentPageModel>.Ignored)).MustNotHaveHappened();
            A.CallTo(() => FakeContentCacheService.Remove(A<Guid>.Ignored)).MustNotHaveHappened();

            Assert.Equal(expectedResponse, result);
        }
    }
}

using DFC.App.Pages.Data.Enums;
using DFC.Content.Pkg.Netcore.Data.Enums;
using FakeItEasy;
using System;
using System.IO;
using System.Net;
using System.Threading.Tasks;
using Xunit;

namespace DFC.App.Pages.Services.CacheContentService.UnitTests.WebhooksServiceTests
{
    [Trait("Category", "Webhooks Service ProcessMessageAsync Unit Tests")]
    public class WebhooksServiceProcessMessageTests : BaseWebhooksServiceTests
    {
        [Fact]
        public async Task WebhooksServiceProcessMessageAsyncNoneOptionReturnsBadRequest()
        {
            // Arrange
            const ContentCacheStatus isContentItem = ContentCacheStatus.ContentItem;
            const HttpStatusCode expectedResponse = HttpStatusCode.BadRequest;
            var url = "https://somewhere.com";
            var service = BuildWebhooksService();

            A.CallTo(() => FakeContentCacheService.CheckIsContentItem(A<Guid>.Ignored)).Returns(isContentItem);

            // Act
            var result = await service.ProcessMessageAsync(WebhookCacheOperation.None, Guid.NewGuid(), ContentIdForCreate, url).ConfigureAwait(false);

            // Assert
            A.CallTo(() => FakeContentCacheService.CheckIsContentItem(A<Guid>.Ignored)).MustHaveHappenedOnceExactly();
            A.CallTo(() => FakeWebhookContentProcessor.DeleteContentItemAsync(A<Guid>.Ignored)).MustNotHaveHappened();
            A.CallTo(() => FakeWebhookContentProcessor.DeleteContentAsync(A<Guid>.Ignored)).MustNotHaveHappened();
            A.CallTo(() => FakeWebhookContentProcessor.ProcessContentItemAsync(A<Uri>.Ignored, A<Guid>.Ignored)).MustNotHaveHappened();
            A.CallTo(() => FakeWebhookContentProcessor.ProcessContentAsync(A<Uri>.Ignored, A<Guid>.Ignored)).MustNotHaveHappened();

            Assert.Equal(expectedResponse, result);
        }

        [Fact]
        public async Task WebhooksServiceProcessMessageAsyncContentThrowsErrorForInvalidUrl()
        {
            // Arrange
            const ContentCacheStatus isContentItem = ContentCacheStatus.ContentItem;
            var url = "/somewhere.com";
            var service = BuildWebhooksService();

            A.CallTo(() => FakeContentCacheService.CheckIsContentItem(A<Guid>.Ignored)).Returns(isContentItem);

            // Act
            await Assert.ThrowsAsync<InvalidDataException>(async () => await service.ProcessMessageAsync(WebhookCacheOperation.CreateOrUpdate, Guid.NewGuid(), ContentIdForCreate, url).ConfigureAwait(false)).ConfigureAwait(false);

            // Assert
            A.CallTo(() => FakeContentCacheService.CheckIsContentItem(A<Guid>.Ignored)).MustHaveHappenedOnceExactly();
            A.CallTo(() => FakeWebhookContentProcessor.DeleteContentItemAsync(A<Guid>.Ignored)).MustNotHaveHappened();
            A.CallTo(() => FakeWebhookContentProcessor.DeleteContentAsync(A<Guid>.Ignored)).MustNotHaveHappened();
            A.CallTo(() => FakeWebhookContentProcessor.ProcessContentItemAsync(A<Uri>.Ignored, A<Guid>.Ignored)).MustNotHaveHappened();
            A.CallTo(() => FakeWebhookContentProcessor.ProcessContentAsync(A<Uri>.Ignored, A<Guid>.Ignored)).MustNotHaveHappened();
        }

        [Fact]
        public async Task WebhooksServiceProcessMessageAsyncContentCreateReturnsSuccess()
        {
            // Arrange
            const ContentCacheStatus isContentItem = ContentCacheStatus.Content;
            const HttpStatusCode expectedResponse = HttpStatusCode.Created;
            var url = "https://somewhere.com";
            var service = BuildWebhooksService();

            A.CallTo(() => FakeContentCacheService.CheckIsContentItem(A<Guid>.Ignored)).Returns(isContentItem);
            A.CallTo(() => FakeWebhookContentProcessor.ProcessContentAsync(A<Uri>.Ignored, A<Guid>.Ignored)).Returns(expectedResponse);

            // Act
            var result = await service.ProcessMessageAsync(WebhookCacheOperation.CreateOrUpdate, Guid.NewGuid(), ContentIdForCreate, url).ConfigureAwait(false);

            // Assert
            A.CallTo(() => FakeContentCacheService.CheckIsContentItem(A<Guid>.Ignored)).MustHaveHappenedOnceExactly();
            A.CallTo(() => FakeWebhookContentProcessor.DeleteContentItemAsync(A<Guid>.Ignored)).MustNotHaveHappened();
            A.CallTo(() => FakeWebhookContentProcessor.DeleteContentAsync(A<Guid>.Ignored)).MustNotHaveHappened();
            A.CallTo(() => FakeWebhookContentProcessor.ProcessContentItemAsync(A<Uri>.Ignored, A<Guid>.Ignored)).MustNotHaveHappened();
            A.CallTo(() => FakeWebhookContentProcessor.ProcessContentAsync(A<Uri>.Ignored, A<Guid>.Ignored)).MustHaveHappenedOnceExactly();

            Assert.Equal(expectedResponse, result);
        }

        [Fact]
        public async Task WebhooksServiceProcessMessageAsyncContentUpdateReturnsSuccess()
        {
            // Arrange
            const ContentCacheStatus isContentItem = ContentCacheStatus.Content;
            const HttpStatusCode expectedResponse = HttpStatusCode.OK;
            var url = "https://somewhere.com";
            var service = BuildWebhooksService();

            A.CallTo(() => FakeContentCacheService.CheckIsContentItem(A<Guid>.Ignored)).Returns(isContentItem);
            A.CallTo(() => FakeWebhookContentProcessor.ProcessContentAsync(A<Uri>.Ignored, A<Guid>.Ignored)).Returns(expectedResponse);

            // Act
            var result = await service.ProcessMessageAsync(WebhookCacheOperation.CreateOrUpdate, Guid.NewGuid(), ContentIdForUpdate, url).ConfigureAwait(false);

            // Assert
            A.CallTo(() => FakeContentCacheService.CheckIsContentItem(A<Guid>.Ignored)).MustHaveHappenedOnceExactly();
            A.CallTo(() => FakeWebhookContentProcessor.DeleteContentItemAsync(A<Guid>.Ignored)).MustNotHaveHappened();
            A.CallTo(() => FakeWebhookContentProcessor.DeleteContentAsync(A<Guid>.Ignored)).MustNotHaveHappened();
            A.CallTo(() => FakeWebhookContentProcessor.ProcessContentItemAsync(A<Uri>.Ignored, A<Guid>.Ignored)).MustNotHaveHappened();
            A.CallTo(() => FakeWebhookContentProcessor.ProcessContentAsync(A<Uri>.Ignored, A<Guid>.Ignored)).MustHaveHappenedOnceExactly();

            Assert.Equal(expectedResponse, result);
        }

        [Fact]
        public async Task WebhooksServiceProcessMessageAsyncContentDeleteReturnsSuccess()
        {
            // Arrange
            const ContentCacheStatus isContentItem = ContentCacheStatus.Content;
            const HttpStatusCode expectedResponse = HttpStatusCode.OK;
            var url = "https://somewhere.com";
            var service = BuildWebhooksService();

            A.CallTo(() => FakeContentCacheService.CheckIsContentItem(A<Guid>.Ignored)).Returns(isContentItem);
            A.CallTo(() => FakeWebhookContentProcessor.DeleteContentAsync(A<Guid>.Ignored)).Returns(expectedResponse);

            // Act
            var result = await service.ProcessMessageAsync(WebhookCacheOperation.Delete, Guid.NewGuid(), ContentIdForDelete, url).ConfigureAwait(false);

            // Assert
            A.CallTo(() => FakeContentCacheService.CheckIsContentItem(A<Guid>.Ignored)).MustHaveHappenedOnceExactly();
            A.CallTo(() => FakeWebhookContentProcessor.DeleteContentItemAsync(A<Guid>.Ignored)).MustNotHaveHappened();
            A.CallTo(() => FakeWebhookContentProcessor.DeleteContentAsync(A<Guid>.Ignored)).MustHaveHappenedOnceExactly();
            A.CallTo(() => FakeWebhookContentProcessor.ProcessContentItemAsync(A<Uri>.Ignored, A<Guid>.Ignored)).MustNotHaveHappened();
            A.CallTo(() => FakeWebhookContentProcessor.ProcessContentAsync(A<Uri>.Ignored, A<Guid>.Ignored)).MustNotHaveHappened();

            Assert.Equal(expectedResponse, result);
        }

        [Fact]
        public async Task WebhooksServiceProcessMessageAsyncContentItemDeleteReturnsSuccess()
        {
            // Arrange
            const ContentCacheStatus isContentItem = ContentCacheStatus.ContentItem;
            const HttpStatusCode expectedResponse = HttpStatusCode.OK;
            var url = "https://somewhere.com";
            var service = BuildWebhooksService();

            A.CallTo(() => FakeContentCacheService.CheckIsContentItem(A<Guid>.Ignored)).Returns(isContentItem);
            A.CallTo(() => FakeWebhookContentProcessor.DeleteContentItemAsync(A<Guid>.Ignored)).Returns(expectedResponse);

            // Act
            var result = await service.ProcessMessageAsync(WebhookCacheOperation.Delete, Guid.NewGuid(), ContentItemIdForDelete, url).ConfigureAwait(false);

            // Assert
            A.CallTo(() => FakeContentCacheService.CheckIsContentItem(A<Guid>.Ignored)).MustHaveHappenedOnceExactly();
            A.CallTo(() => FakeWebhookContentProcessor.DeleteContentItemAsync(A<Guid>.Ignored)).MustHaveHappenedOnceExactly();
            A.CallTo(() => FakeWebhookContentProcessor.DeleteContentAsync(A<Guid>.Ignored)).MustNotHaveHappened();
            A.CallTo(() => FakeWebhookContentProcessor.ProcessContentItemAsync(A<Uri>.Ignored, A<Guid>.Ignored)).MustNotHaveHappened();
            A.CallTo(() => FakeWebhookContentProcessor.ProcessContentAsync(A<Uri>.Ignored, A<Guid>.Ignored)).MustNotHaveHappened();

            Assert.Equal(expectedResponse, result);
        }
    }
}

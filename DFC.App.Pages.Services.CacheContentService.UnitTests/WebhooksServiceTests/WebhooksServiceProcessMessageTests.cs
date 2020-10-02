using DFC.App.Pages.Data.Enums;
using DFC.App.Pages.Data.Models;
using DFC.App.Pages.Data.Models.CmsApiModels;
using DFC.Content.Pkg.Netcore.Data.Enums;
using FakeItEasy;
using System;
using System.Collections.Generic;
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
        public async Task WebhooksServiceProcessMessageAsyncNoneOptionReturnsSuccess()
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
            A.CallTo(() => FakeCmsApiService.GetItemAsync<CmsApiDataModel>(A<Uri>.Ignored)).MustNotHaveHappened();
            A.CallTo(() => FakeMapper.Map<ContentPageModel>(A<CmsApiDataModel>.Ignored)).MustNotHaveHappened();
            A.CallTo(() => FakeEventMessageService.UpdateAsync(A<ContentPageModel>.Ignored)).MustNotHaveHappened();
            A.CallTo(() => FakeEventMessageService.CreateAsync(A<ContentPageModel>.Ignored)).MustNotHaveHappened();
            A.CallTo(() => FakeEventMessageService.DeleteAsync(A<Guid>.Ignored)).MustNotHaveHappened();

            Assert.Equal(expectedResponse, result);
        }

        [Fact]
        public async Task WebhooksServiceProcessMessageAsyncContentThrowsErrorForInvalidUrl()
        {
            // Arrange
            const ContentCacheStatus isContentItem = ContentCacheStatus.ContentItem;
            var expectedValidApiContentModel = BuildValidPagesApiContentModel();
            var expectedValidContentPageModel = BuildValidContentPageModel();
            var url = "/somewhere.com";
            var service = BuildWebhooksService();

            A.CallTo(() => FakeContentCacheService.CheckIsContentItem(A<Guid>.Ignored)).Returns(isContentItem);
            A.CallTo(() => FakeCmsApiService.GetItemAsync<CmsApiDataModel>(A<Uri>.Ignored)).Returns(expectedValidApiContentModel);
            A.CallTo(() => FakeMapper.Map<ContentPageModel>(A<CmsApiDataModel>.Ignored)).Returns(expectedValidContentPageModel);
            A.CallTo(() => FakeEventMessageService.UpdateAsync(A<ContentPageModel>.Ignored)).Returns(HttpStatusCode.NotFound);
            A.CallTo(() => FakeEventMessageService.CreateAsync(A<ContentPageModel>.Ignored)).Returns(HttpStatusCode.Created);

            // Act
            await Assert.ThrowsAsync<InvalidDataException>(async () => await service.ProcessMessageAsync(WebhookCacheOperation.CreateOrUpdate, Guid.NewGuid(), ContentIdForCreate, url).ConfigureAwait(false)).ConfigureAwait(false);
        }

        [Fact]
        public async Task WebhooksServiceProcessMessageAsyncContentCreateReturnsSuccess()
        {
            // Arrange
            const ContentCacheStatus isContentItem = ContentCacheStatus.Content;
            const HttpStatusCode expectedResponse = HttpStatusCode.Created;
            var expectedValidApiContentModel = BuildValidPagesApiContentModel();
            var expectedValidContentPageModel = BuildValidContentPageModel();
            var url = "https://somewhere.com";
            var service = BuildWebhooksService();

            A.CallTo(() => FakeContentCacheService.CheckIsContentItem(A<Guid>.Ignored)).Returns(isContentItem);
            A.CallTo(() => FakeCmsApiService.GetItemAsync<CmsApiDataModel>(A<Uri>.Ignored)).Returns(expectedValidApiContentModel);
            A.CallTo(() => FakeMapper.Map<ContentPageModel>(A<CmsApiDataModel>.Ignored)).Returns(expectedValidContentPageModel);
            A.CallTo(() => FakeEventMessageService.UpdateAsync(A<ContentPageModel>.Ignored)).Returns(HttpStatusCode.NotFound);
            A.CallTo(() => FakeEventMessageService.CreateAsync(A<ContentPageModel>.Ignored)).Returns(HttpStatusCode.Created);

            // Act
            var result = await service.ProcessMessageAsync(WebhookCacheOperation.CreateOrUpdate, Guid.NewGuid(), ContentIdForCreate, url).ConfigureAwait(false);

            // Assert
            A.CallTo(() => FakeContentCacheService.CheckIsContentItem(A<Guid>.Ignored)).MustHaveHappenedOnceExactly();
            A.CallTo(() => FakeCmsApiService.GetItemAsync<CmsApiDataModel>(A<Uri>.Ignored)).MustHaveHappenedOnceExactly();
            A.CallTo(() => FakeMapper.Map<ContentPageModel>(A<CmsApiDataModel>.Ignored)).MustHaveHappenedOnceExactly();
            A.CallTo(() => FakeEventMessageService.UpdateAsync(A<ContentPageModel>.Ignored)).MustHaveHappenedOnceExactly();
            A.CallTo(() => FakeEventMessageService.CreateAsync(A<ContentPageModel>.Ignored)).MustHaveHappenedOnceExactly();
            A.CallTo(() => FakeEventMessageService.DeleteAsync(A<Guid>.Ignored)).MustNotHaveHappened();

            Assert.Equal(expectedResponse, result);
        }

        [Fact]
        public async Task WebhooksServiceProcessMessageAsyncContentUpdateReturnsSuccess()
        {
            // Arrange
            const ContentCacheStatus isContentItem = ContentCacheStatus.Content;
            const HttpStatusCode expectedResponse = HttpStatusCode.OK;
            var expectedValidApiContentModel = BuildValidPagesApiContentModel();
            var expectedValidContentPageModel = BuildValidContentPageModel();
            var url = "https://somewhere.com";
            var service = BuildWebhooksService();

            A.CallTo(() => FakeContentCacheService.CheckIsContentItem(A<Guid>.Ignored)).Returns(isContentItem);
            A.CallTo(() => FakeCmsApiService.GetItemAsync<CmsApiDataModel>(A<Uri>.Ignored)).Returns(expectedValidApiContentModel);
            A.CallTo(() => FakeMapper.Map<ContentPageModel>(A<CmsApiDataModel>.Ignored)).Returns(expectedValidContentPageModel);
            A.CallTo(() => FakeEventMessageService.UpdateAsync(A<ContentPageModel>.Ignored)).Returns(HttpStatusCode.OK);

            // Act
            var result = await service.ProcessMessageAsync(WebhookCacheOperation.CreateOrUpdate, Guid.NewGuid(), ContentIdForUpdate, url).ConfigureAwait(false);

            // Assert
            A.CallTo(() => FakeContentCacheService.CheckIsContentItem(A<Guid>.Ignored)).MustHaveHappenedOnceExactly();
            A.CallTo(() => FakeCmsApiService.GetItemAsync<CmsApiDataModel>(A<Uri>.Ignored)).MustHaveHappenedOnceExactly();
            A.CallTo(() => FakeMapper.Map<ContentPageModel>(A<CmsApiDataModel>.Ignored)).MustHaveHappenedOnceExactly();
            A.CallTo(() => FakeEventMessageService.UpdateAsync(A<ContentPageModel>.Ignored)).MustHaveHappenedOnceExactly();
            A.CallTo(() => FakeEventMessageService.CreateAsync(A<ContentPageModel>.Ignored)).MustNotHaveHappened();
            A.CallTo(() => FakeEventMessageService.DeleteAsync(A<Guid>.Ignored)).MustNotHaveHappened();

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
            A.CallTo(() => FakeEventMessageService.DeleteAsync(A<Guid>.Ignored)).Returns(HttpStatusCode.OK);

            // Act
            var result = await service.ProcessMessageAsync(WebhookCacheOperation.Delete, Guid.NewGuid(), ContentIdForDelete, url).ConfigureAwait(false);

            // Assert
            A.CallTo(() => FakeContentCacheService.CheckIsContentItem(A<Guid>.Ignored)).MustHaveHappenedOnceExactly();
            A.CallTo(() => FakeCmsApiService.GetItemAsync<CmsApiDataModel>(A<Uri>.Ignored)).MustNotHaveHappened();
            A.CallTo(() => FakeEventMessageService.UpdateAsync(A<ContentPageModel>.Ignored)).MustNotHaveHappened();
            A.CallTo(() => FakeEventMessageService.CreateAsync(A<ContentPageModel>.Ignored)).MustNotHaveHappened();
            A.CallTo(() => FakeEventMessageService.DeleteAsync(A<Guid>.Ignored)).MustHaveHappenedOnceExactly();

            Assert.Equal(expectedResponse, result);
        }

        [Fact]
        public async Task WebhooksServiceProcessMessageAsyncPageLocationCreateReturnsSuccess()
        {
            // Arrange
            const ContentCacheStatus isContentItem = ContentCacheStatus.ContentItem;
            const HttpStatusCode expectedResponse = HttpStatusCode.OK;
            var expectedValidApiPageLocationModel = BuildValidPagesApiPageLocationModel(PageLocationIdForCreate);
            var expectedValidContentPageModel = BuildValidContentPageModel();
            var url = "https://somewhere.com";
            var service = BuildWebhooksService();

            A.CallTo(() => FakeContentCacheService.CheckIsContentItem(A<Guid>.Ignored)).Returns(isContentItem);
            A.CallTo(() => FakeContentCacheService.GetContentIdsContainingContentItemId(A<Guid>.Ignored)).Returns(new List<Guid> { PageLocationIdForCreate, Guid.NewGuid() });
            A.CallTo(() => FakeCmsApiService.GetContentItemAsync<CmsApiPageLocationModel>(A<Uri>.Ignored)).Returns(expectedValidApiPageLocationModel);
            A.CallTo(() => FakeContentPageService.GetByIdAsync(A<Guid>.Ignored, A<string>.Ignored)).Returns(expectedValidContentPageModel);
            A.CallTo(() => FakeEventMessageService.UpdateAsync(A<ContentPageModel>.Ignored)).Returns(HttpStatusCode.NotFound);
            A.CallTo(() => FakeEventMessageService.CreateAsync(A<ContentPageModel>.Ignored)).Returns(HttpStatusCode.Created);

            // Act
            var result = await service.ProcessMessageAsync(WebhookCacheOperation.CreateOrUpdate, Guid.NewGuid(), PageLocationIdForCreate, url).ConfigureAwait(false);

            // Assert
            A.CallTo(() => FakeContentCacheService.CheckIsContentItem(A<Guid>.Ignored)).MustHaveHappenedOnceExactly();
            A.CallTo(() => FakeContentCacheService.GetContentIdsContainingContentItemId(A<Guid>.Ignored)).MustHaveHappenedOnceExactly();
            A.CallTo(() => FakeCmsApiService.GetContentItemAsync<CmsApiPageLocationModel>(A<Uri>.Ignored)).MustHaveHappenedOnceOrMore();
            A.CallTo(() => FakeCmsApiService.GetContentItemAsync<CmsApiHtmlModel>(A<Uri>.Ignored)).MustNotHaveHappened();
            A.CallTo(() => FakeCmsApiService.GetContentItemAsync<CmsApiHtmlSharedModel>(A<Uri>.Ignored)).MustNotHaveHappened();
            A.CallTo(() => FakeCmsApiService.GetContentItemAsync<CmsApiSharedContentModel>(A<Uri>.Ignored)).MustNotHaveHappened();
            A.CallTo(() => FakeContentPageService.GetByIdAsync(A<Guid>.Ignored, A<string>.Ignored)).MustHaveHappenedOnceOrMore();
            A.CallTo(() => FakeEventMessageService.UpdateAsync(A<ContentPageModel>.Ignored)).MustHaveHappenedOnceOrMore();
            A.CallTo(() => FakeEventMessageService.CreateAsync(A<ContentPageModel>.Ignored)).MustNotHaveHappened();
            A.CallTo(() => FakeEventMessageService.DeleteAsync(A<Guid>.Ignored)).MustNotHaveHappened();

            Assert.Equal(expectedResponse, result);
        }

        [Fact]
        public async Task WebhooksServiceProcessMessageAsyncPageLocationUpdateReturnsSuccess()
        {
            // Arrange
            const ContentCacheStatus isContentItem = ContentCacheStatus.ContentItem;
            const HttpStatusCode expectedResponse = HttpStatusCode.OK;
            var expectedValidApiPageLocationModel = BuildValidPagesApiPageLocationModel(PageLocationIdForUpdate);
            var expectedValidContentPageModel = BuildValidContentPageModel();
            var url = "https://somewhere.com";
            var service = BuildWebhooksService();

            A.CallTo(() => FakeContentCacheService.CheckIsContentItem(A<Guid>.Ignored)).Returns(isContentItem);
            A.CallTo(() => FakeContentCacheService.GetContentIdsContainingContentItemId(A<Guid>.Ignored)).Returns(new List<Guid> { PageLocationIdForUpdate, Guid.NewGuid() });
            A.CallTo(() => FakeCmsApiService.GetContentItemAsync<CmsApiPageLocationModel>(A<Uri>.Ignored)).Returns(expectedValidApiPageLocationModel);
            A.CallTo(() => FakeContentPageService.GetByIdAsync(A<Guid>.Ignored, A<string>.Ignored)).Returns(expectedValidContentPageModel);
            A.CallTo(() => FakeEventMessageService.UpdateAsync(A<ContentPageModel>.Ignored)).Returns(HttpStatusCode.OK);

            // Act
            var result = await service.ProcessMessageAsync(WebhookCacheOperation.CreateOrUpdate, Guid.NewGuid(), PageLocationIdForUpdate, url).ConfigureAwait(false);

            // Assert
            A.CallTo(() => FakeContentCacheService.CheckIsContentItem(A<Guid>.Ignored)).MustHaveHappenedOnceExactly();
            A.CallTo(() => FakeContentCacheService.GetContentIdsContainingContentItemId(A<Guid>.Ignored)).MustHaveHappenedOnceExactly();
            A.CallTo(() => FakeCmsApiService.GetContentItemAsync<CmsApiPageLocationModel>(A<Uri>.Ignored)).MustHaveHappenedOnceOrMore();
            A.CallTo(() => FakeCmsApiService.GetContentItemAsync<CmsApiHtmlModel>(A<Uri>.Ignored)).MustNotHaveHappened();
            A.CallTo(() => FakeCmsApiService.GetContentItemAsync<CmsApiHtmlSharedModel>(A<Uri>.Ignored)).MustNotHaveHappened();
            A.CallTo(() => FakeCmsApiService.GetContentItemAsync<CmsApiSharedContentModel>(A<Uri>.Ignored)).MustNotHaveHappened();
            A.CallTo(() => FakeContentPageService.GetByIdAsync(A<Guid>.Ignored, A<string>.Ignored)).MustHaveHappenedOnceOrMore();
            A.CallTo(() => FakeEventMessageService.UpdateAsync(A<ContentPageModel>.Ignored)).MustHaveHappenedOnceOrMore();
            A.CallTo(() => FakeEventMessageService.CreateAsync(A<ContentPageModel>.Ignored)).MustNotHaveHappened();
            A.CallTo(() => FakeEventMessageService.DeleteAsync(A<Guid>.Ignored)).MustNotHaveHappened();

            Assert.Equal(expectedResponse, result);
        }

        [Fact]
        public async Task WebhooksServiceProcessMessageAsyncContentItemDeleteReturnsSuccess()
        {
            // Arrange
            const ContentCacheStatus isContentItem = ContentCacheStatus.ContentItem;
            const HttpStatusCode expectedResponse = HttpStatusCode.OK;
            var expectedValidContentPageModel = BuildValidContentPageModel();
            var url = "https://somewhere.com";
            var service = BuildWebhooksService();

            A.CallTo(() => FakeContentCacheService.CheckIsContentItem(A<Guid>.Ignored)).Returns(isContentItem);
            A.CallTo(() => FakeContentCacheService.GetContentIdsContainingContentItemId(A<Guid>.Ignored)).Returns(new List<Guid> { ContentIdForDelete, Guid.NewGuid() });
            A.CallTo(() => FakeContentPageService.GetByIdAsync(A<Guid>.Ignored, A<string>.Ignored)).Returns(expectedValidContentPageModel);
            A.CallTo(() => FakeEventMessageService.UpdateAsync(A<ContentPageModel>.Ignored)).Returns(HttpStatusCode.OK);

            // Act
            var result = await service.ProcessMessageAsync(WebhookCacheOperation.Delete, Guid.NewGuid(), ContentItemIdForDelete, url).ConfigureAwait(false);

            // Assert
            A.CallTo(() => FakeContentCacheService.CheckIsContentItem(A<Guid>.Ignored)).MustHaveHappenedOnceExactly();
            A.CallTo(() => FakeContentCacheService.GetContentIdsContainingContentItemId(A<Guid>.Ignored)).MustHaveHappenedOnceExactly();
            A.CallTo(() => FakeContentPageService.GetByIdAsync(A<Guid>.Ignored, A<string>.Ignored)).MustHaveHappenedOnceOrMore();
            A.CallTo(() => FakeEventMessageService.UpdateAsync(A<ContentPageModel>.Ignored)).MustHaveHappenedOnceOrMore();
            A.CallTo(() => FakeEventMessageService.CreateAsync(A<ContentPageModel>.Ignored)).MustNotHaveHappened();
            A.CallTo(() => FakeEventMessageService.DeleteAsync(A<Guid>.Ignored)).MustNotHaveHappened();

            Assert.Equal(expectedResponse, result);
        }
    }
}

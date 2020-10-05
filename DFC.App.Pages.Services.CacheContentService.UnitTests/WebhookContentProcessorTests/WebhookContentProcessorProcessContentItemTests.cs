using DFC.App.Pages.Data.Common;
using DFC.App.Pages.Data.Models;
using FakeItEasy;
using System;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using Xunit;

namespace DFC.App.Pages.Services.CacheContentService.UnitTests.WebhookContentProcessorTests
{
    [Trait("Category", "WebhookContentProcessor - ProcessContentItem Unit Tests")]
    public class WebhookContentProcessorProcessContentItemTests : BaseWebhookContentProcessor
    {
        [Fact]
        public async Task WebhookContentProcessorProcessPageLocationAsyncReturnsSuccess()
        {
            // Arrange
            const HttpStatusCode expectedResponse = HttpStatusCode.OK;
            var exptectedGuidList = new List<Guid> { ContentIdForUpdate, Guid.NewGuid() };
            var expectedValidContentPageModel = BuildValidContentPageModel();
            var url = new Uri("https://somewhere.com");
            var service = BuildWebhookContentProcessor();

            A.CallTo(() => FakeContentCacheService.GetContentIdsContainingContentItemId(A<Guid>.Ignored)).Returns(exptectedGuidList);
            A.CallTo(() => FakeContentPageService.GetByIdAsync(A<Guid>.Ignored, A<string>.Ignored)).Returns(expectedValidContentPageModel);
            A.CallTo(() => FakePageLocatonUpdater.FindAndUpdateAsync(A<Uri>.Ignored, A<Guid>.Ignored, A<List<PageLocationModel>>.Ignored)).Returns(true);
            A.CallTo(() => FakeEventMessageService.UpdateAsync(A<ContentPageModel>.Ignored)).Returns(HttpStatusCode.OK);

            // Act
            var result = await service.ProcessContentItemAsync(url, PageLocationIdForCreate).ConfigureAwait(false);

            // Assert
            A.CallTo(() => FakeContentCacheService.GetContentIdsContainingContentItemId(A<Guid>.Ignored)).MustHaveHappenedOnceExactly();
            A.CallTo(() => FakeContentPageService.GetByIdAsync(A<Guid>.Ignored, A<string>.Ignored)).MustHaveHappenedOnceOrMore();
            A.CallTo(() => FakePageLocatonUpdater.FindAndUpdateAsync(A<Uri>.Ignored, A<Guid>.Ignored, A<List<PageLocationModel>>.Ignored)).MustHaveHappenedOnceOrMore();
            A.CallTo(() => FakeContentItemUpdater.FindAndUpdateAsync(A<Uri>.Ignored, A<Guid>.Ignored, A<List<ContentItemModel>>.Ignored)).MustNotHaveHappened();
            A.CallTo(() => FakeEventMessageService.UpdateAsync(A<ContentPageModel>.Ignored)).MustHaveHappenedOnceOrMore();
            A.CallTo(() => FakeEventMessageService.CreateAsync(A<ContentPageModel>.Ignored)).MustNotHaveHappened();
            A.CallTo(() => FakeEventMessageService.DeleteAsync(A<Guid>.Ignored)).MustNotHaveHappened();
            A.CallTo(() => FakeEventGridService.CompareAndSendEventAsync(A<ContentPageModel>.Ignored, A<ContentPageModel>.Ignored)).MustHaveHappenedOnceOrMore();

            Assert.Equal(expectedResponse, result);
        }

        [Fact]
        public async Task WebhookContentProcessorProcessContentitemAsyncReturnsSuccess()
        {
            // Arrange
            const HttpStatusCode expectedResponse = HttpStatusCode.OK;
            var exptectedGuidList = new List<Guid> { ContentIdForUpdate, Guid.NewGuid() };
            var expectedValidContentPageModel = BuildValidContentPageModel(Constants.ContentTypeSharedContent);
            var url = new Uri("https://somewhere.com");
            var service = BuildWebhookContentProcessor();

            A.CallTo(() => FakeContentCacheService.GetContentIdsContainingContentItemId(A<Guid>.Ignored)).Returns(exptectedGuidList);
            A.CallTo(() => FakeContentPageService.GetByIdAsync(A<Guid>.Ignored, A<string>.Ignored)).Returns(expectedValidContentPageModel);
            A.CallTo(() => FakeContentItemUpdater.FindAndUpdateAsync(A<Uri>.Ignored, A<Guid>.Ignored, A<List<ContentItemModel>>.Ignored)).Returns(true);
            A.CallTo(() => FakeEventMessageService.UpdateAsync(A<ContentPageModel>.Ignored)).Returns(HttpStatusCode.OK);

            // Act
            var result = await service.ProcessContentItemAsync(url, ContentItemIdForCreate).ConfigureAwait(false);

            // Assert
            A.CallTo(() => FakeContentCacheService.GetContentIdsContainingContentItemId(A<Guid>.Ignored)).MustHaveHappenedOnceExactly();
            A.CallTo(() => FakeContentPageService.GetByIdAsync(A<Guid>.Ignored, A<string>.Ignored)).MustHaveHappenedOnceOrMore();
            A.CallTo(() => FakePageLocatonUpdater.FindAndUpdateAsync(A<Uri>.Ignored, A<Guid>.Ignored, A<List<PageLocationModel>>.Ignored)).MustNotHaveHappened();
            A.CallTo(() => FakeContentItemUpdater.FindAndUpdateAsync(A<Uri>.Ignored, A<Guid>.Ignored, A<List<ContentItemModel>>.Ignored)).MustHaveHappenedOnceOrMore();
            A.CallTo(() => FakeEventMessageService.UpdateAsync(A<ContentPageModel>.Ignored)).MustHaveHappenedOnceOrMore();
            A.CallTo(() => FakeEventMessageService.CreateAsync(A<ContentPageModel>.Ignored)).MustNotHaveHappened();
            A.CallTo(() => FakeEventMessageService.DeleteAsync(A<Guid>.Ignored)).MustNotHaveHappened();
            A.CallTo(() => FakeEventGridService.CompareAndSendEventAsync(A<ContentPageModel>.Ignored, A<ContentPageModel>.Ignored)).MustHaveHappenedOnceOrMore();

            Assert.Equal(expectedResponse, result);
        }

        [Fact]
        public async Task WebhookContentProcessorProcessContentItemAsyncReturnsNoContentWhenNoContentIds()
        {
            // Arrange
            const HttpStatusCode expectedResponse = HttpStatusCode.NoContent;
            var exptectedEmptyGuidList = new List<Guid>();
            var url = new Uri("https://somewhere.com");
            var service = BuildWebhookContentProcessor();

            A.CallTo(() => FakeContentCacheService.GetContentIdsContainingContentItemId(A<Guid>.Ignored)).Returns(exptectedEmptyGuidList);

            // Act
            var result = await service.ProcessContentItemAsync(url, ContentItemIdForUpdate).ConfigureAwait(false);

            // Assert
            A.CallTo(() => FakeContentCacheService.GetContentIdsContainingContentItemId(A<Guid>.Ignored)).MustHaveHappenedOnceExactly();
            A.CallTo(() => FakeContentPageService.GetByIdAsync(A<Guid>.Ignored, A<string>.Ignored)).MustNotHaveHappened();
            A.CallTo(() => FakePageLocatonUpdater.FindAndUpdateAsync(A<Uri>.Ignored, A<Guid>.Ignored, A<List<PageLocationModel>>.Ignored)).MustNotHaveHappened();
            A.CallTo(() => FakeContentItemUpdater.FindAndUpdateAsync(A<Uri>.Ignored, A<Guid>.Ignored, A<List<ContentItemModel>>.Ignored)).MustNotHaveHappened();
            A.CallTo(() => FakeEventMessageService.UpdateAsync(A<ContentPageModel>.Ignored)).MustNotHaveHappened();
            A.CallTo(() => FakeEventMessageService.CreateAsync(A<ContentPageModel>.Ignored)).MustNotHaveHappened();
            A.CallTo(() => FakeEventMessageService.DeleteAsync(A<Guid>.Ignored)).MustNotHaveHappened();
            A.CallTo(() => FakeEventGridService.CompareAndSendEventAsync(A<ContentPageModel>.Ignored, A<ContentPageModel>.Ignored)).MustNotHaveHappened();

            Assert.Equal(expectedResponse, result);
        }

        [Fact]
        public async Task WebhookContentProcessorProcessPageLocationAsyncReturnsNoContentWhenNoApiDataResponse()
        {
            // Arrange
            const HttpStatusCode expectedResponse = HttpStatusCode.NoContent;
            var exptectedGuidList = new List<Guid> { ContentIdForUpdate, Guid.NewGuid() };
            ContentPageModel? expectedNullContentPageModel = null;
            var url = new Uri("https://somewhere.com");
            var service = BuildWebhookContentProcessor();

            A.CallTo(() => FakeContentCacheService.GetContentIdsContainingContentItemId(A<Guid>.Ignored)).Returns(exptectedGuidList);
            A.CallTo(() => FakeContentPageService.GetByIdAsync(A<Guid>.Ignored, A<string>.Ignored)).Returns(expectedNullContentPageModel);

            // Act
            var result = await service.ProcessContentItemAsync(url, PageLocationIdForUpdate).ConfigureAwait(false);

            // Assert
            A.CallTo(() => FakeContentCacheService.GetContentIdsContainingContentItemId(A<Guid>.Ignored)).MustHaveHappenedOnceExactly();
            A.CallTo(() => FakeContentPageService.GetByIdAsync(A<Guid>.Ignored, A<string>.Ignored)).MustHaveHappenedOnceOrMore();
            A.CallTo(() => FakePageLocatonUpdater.FindAndUpdateAsync(A<Uri>.Ignored, A<Guid>.Ignored, A<List<PageLocationModel>>.Ignored)).MustNotHaveHappened();
            A.CallTo(() => FakeContentItemUpdater.FindAndUpdateAsync(A<Uri>.Ignored, A<Guid>.Ignored, A<List<ContentItemModel>>.Ignored)).MustNotHaveHappened();
            A.CallTo(() => FakeEventMessageService.UpdateAsync(A<ContentPageModel>.Ignored)).MustNotHaveHappened();
            A.CallTo(() => FakeEventMessageService.CreateAsync(A<ContentPageModel>.Ignored)).MustNotHaveHappened();
            A.CallTo(() => FakeEventMessageService.DeleteAsync(A<Guid>.Ignored)).MustNotHaveHappened();
            A.CallTo(() => FakeEventGridService.CompareAndSendEventAsync(A<ContentPageModel>.Ignored, A<ContentPageModel>.Ignored)).MustNotHaveHappened();

            Assert.Equal(expectedResponse, result);
        }
    }
}

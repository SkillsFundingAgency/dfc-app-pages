using DFC.App.Pages.Data.Models;
using FakeItEasy;
using System;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using Xunit;

namespace DFC.App.Pages.Services.CacheContentService.UnitTests.WebhookContentProcessorTests
{
    [Trait("Category", "WebhookContentProcessor - DeleteContentItem Unit Tests")]
    public class WebhookContentProcessorDeleteContentItemTests : BaseWebhookContentProcessor
    {
        [Fact]
        public async Task WebhookContentProcessorDeleteContentItemAsyncForCreateReturnsSuccess()
        {
            // Arrange
            const HttpStatusCode expectedResponse = HttpStatusCode.OK;
            var exptectedGuidList = new List<Guid> { ContentIdForCreate, Guid.NewGuid() };
            var expectedValidContentPageModel = BuildValidContentPageModel();
            var service = BuildWebhookContentProcessor();

            A.CallTo(() => FakeContentCacheService.GetContentIdsContainingContentItemId(A<Guid>.Ignored)).Returns(exptectedGuidList);
            A.CallTo(() => FakeContentPageService.GetByIdAsync(A<Guid>.Ignored, A<string>.Ignored)).Returns(expectedValidContentPageModel);
            A.CallTo(() => FakeEventMessageService.UpdateAsync(A<ContentPageModel>.Ignored)).Returns(HttpStatusCode.OK);
            A.CallTo(() => FakeContentCacheService.RemoveContentItem(A<Guid>.Ignored, A<Guid>.Ignored));

            // Act
            var result = await service.DeleteContentItemAsync(ContentItemIdForDelete).ConfigureAwait(false);

            // Assert
            A.CallTo(() => FakeContentCacheService.GetContentIdsContainingContentItemId(A<Guid>.Ignored)).MustHaveHappenedOnceExactly();
            A.CallTo(() => FakeContentPageService.GetByIdAsync(A<Guid>.Ignored, A<string>.Ignored)).MustHaveHappenedOnceOrMore();
            A.CallTo(() => FakeEventMessageService.UpdateAsync(A<ContentPageModel>.Ignored)).MustHaveHappenedOnceOrMore();
            A.CallTo(() => FakeEventMessageService.CreateAsync(A<ContentPageModel>.Ignored)).MustNotHaveHappened();
            A.CallTo(() => FakeEventMessageService.DeleteAsync(A<Guid>.Ignored)).MustNotHaveHappened();
            A.CallTo(() => FakeContentCacheService.RemoveContentItem(A<Guid>.Ignored, A<Guid>.Ignored)).MustHaveHappenedOnceExactly();

            Assert.Equal(expectedResponse, result);
        }

        [Fact]
        public async Task WebhookContentProcessorDeletePageLocationAsyncForCreateReturnsSuccess()
        {
            // Arrange
            const HttpStatusCode expectedResponse = HttpStatusCode.OK;
            var exptectedGuidList = new List<Guid> { PageLocationIdForCreate, Guid.NewGuid() };
            var expectedValidContentPageModel = BuildValidContentPageModel();
            var service = BuildWebhookContentProcessor();

            A.CallTo(() => FakeContentCacheService.GetContentIdsContainingContentItemId(A<Guid>.Ignored)).Returns(exptectedGuidList);
            A.CallTo(() => FakeContentPageService.GetByIdAsync(A<Guid>.Ignored, A<string>.Ignored)).Returns(expectedValidContentPageModel);
            A.CallTo(() => FakeEventMessageService.UpdateAsync(A<ContentPageModel>.Ignored)).Returns(HttpStatusCode.OK);
            A.CallTo(() => FakeContentCacheService.RemoveContentItem(A<Guid>.Ignored, A<Guid>.Ignored));

            // Act
            var result = await service.DeleteContentItemAsync(PageLocationIdForDelete).ConfigureAwait(false);

            // Assert
            A.CallTo(() => FakeContentCacheService.GetContentIdsContainingContentItemId(A<Guid>.Ignored)).MustHaveHappenedOnceExactly();
            A.CallTo(() => FakeContentPageService.GetByIdAsync(A<Guid>.Ignored, A<string>.Ignored)).MustHaveHappenedOnceOrMore();
            A.CallTo(() => FakeEventMessageService.UpdateAsync(A<ContentPageModel>.Ignored)).MustHaveHappenedOnceOrMore();
            A.CallTo(() => FakeEventMessageService.CreateAsync(A<ContentPageModel>.Ignored)).MustNotHaveHappened();
            A.CallTo(() => FakeEventMessageService.DeleteAsync(A<Guid>.Ignored)).MustNotHaveHappened();
            A.CallTo(() => FakeContentCacheService.RemoveContentItem(A<Guid>.Ignored, A<Guid>.Ignored)).MustHaveHappenedOnceExactly();

            Assert.Equal(expectedResponse, result);
        }

        [Fact]
        public async Task WebhookContentProcessorDeleteContentItemAsyncForUpdateReturnsNoContent()
        {
            // Arrange
            const HttpStatusCode expectedResponse = HttpStatusCode.NoContent;
            var exptectedEmptyGuidList = new List<Guid>();
            var service = BuildWebhookContentProcessor();

            A.CallTo(() => FakeContentCacheService.GetContentIdsContainingContentItemId(A<Guid>.Ignored)).Returns(exptectedEmptyGuidList);

            // Act
            var result = await service.DeleteContentItemAsync(ContentItemIdForDelete).ConfigureAwait(false);

            // Assert
            A.CallTo(() => FakeContentCacheService.GetContentIdsContainingContentItemId(A<Guid>.Ignored)).MustHaveHappenedOnceExactly();
            A.CallTo(() => FakeContentPageService.GetByIdAsync(A<Guid>.Ignored, A<string>.Ignored)).MustNotHaveHappened();
            A.CallTo(() => FakeEventMessageService.UpdateAsync(A<ContentPageModel>.Ignored)).MustNotHaveHappened();
            A.CallTo(() => FakeEventMessageService.CreateAsync(A<ContentPageModel>.Ignored)).MustNotHaveHappened();
            A.CallTo(() => FakeEventMessageService.DeleteAsync(A<Guid>.Ignored)).MustNotHaveHappened();
            A.CallTo(() => FakeContentCacheService.RemoveContentItem(A<Guid>.Ignored, A<Guid>.Ignored)).MustNotHaveHappened();

            Assert.Equal(expectedResponse, result);
        }
    }
}

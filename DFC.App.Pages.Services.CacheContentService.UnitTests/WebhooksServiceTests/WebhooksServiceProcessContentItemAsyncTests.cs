using DFC.App.Pages.Data.Common;
using DFC.App.Pages.Data.Models;
using DFC.App.Pages.Data.Models.CmsApiModels;
using FakeItEasy;
using System;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using Xunit;

namespace DFC.App.Pages.Services.CacheContentService.UnitTests.WebhooksServiceTests
{
    [Trait("Category", "Webhooks Service ProcessContentItemAsync Unit Tests")]
    public class WebhooksServiceProcessContentItemAsyncTests : BaseWebhooksServiceTests
    {
        [Fact]
        public async Task WebhooksServiceProcessPageLocationAsyncForCreateReturnsSuccess()
        {
            // Arrange
            const HttpStatusCode expectedResponse = HttpStatusCode.OK;
            var exptectedGuidList = new List<Guid> { ContentIdForUpdate, Guid.NewGuid() };
            var expectedValidApiPageLocationModel = BuildValidPagesApiPageLocationModel(PageLocationIdForCreate);
            var expectedValidContentPageModel = BuildValidContentPageModel();
            var url = new Uri("https://somewhere.com");
            var service = BuildWebhooksService();

            A.CallTo(() => FakeContentCacheService.GetContentIdsContainingContentItemId(A<Guid>.Ignored)).Returns(exptectedGuidList);
            A.CallTo(() => FakeCmsApiService.GetContentItemAsync<CmsApiPageLocationModel>(A<Uri>.Ignored)).Returns(expectedValidApiPageLocationModel);
            A.CallTo(() => FakeContentPageService.GetByIdAsync(A<Guid>.Ignored, A<string>.Ignored)).Returns(expectedValidContentPageModel);
            A.CallTo(() => FakeEventMessageService.UpdateAsync(A<ContentPageModel>.Ignored)).Returns(HttpStatusCode.OK);

            // Act
            var result = await service.ProcessContentItemAsync(url, PageLocationIdForCreate).ConfigureAwait(false);

            // Assert
            A.CallTo(() => FakeContentCacheService.GetContentIdsContainingContentItemId(A<Guid>.Ignored)).MustHaveHappenedOnceExactly();
            A.CallTo(() => FakeCmsApiService.GetContentItemAsync<CmsApiPageLocationModel>(A<Uri>.Ignored)).MustHaveHappenedOnceOrMore();
            A.CallTo(() => FakeCmsApiService.GetContentItemAsync<CmsApiHtmlModel>(A<Uri>.Ignored)).MustNotHaveHappened();
            A.CallTo(() => FakeCmsApiService.GetContentItemAsync<CmsApiHtmlSharedModel>(A<Uri>.Ignored)).MustNotHaveHappened();
            A.CallTo(() => FakeCmsApiService.GetContentItemAsync<CmsApiSharedContentModel>(A<Uri>.Ignored)).MustNotHaveHappened();
            A.CallTo(() => FakeContentPageService.GetByIdAsync(A<Guid>.Ignored, A<string>.Ignored)).MustHaveHappenedOnceOrMore();
            A.CallTo(() => FakeEventMessageService.UpdateAsync(A<ContentPageModel>.Ignored)).MustHaveHappenedOnceOrMore();
            A.CallTo(() => FakeEventMessageService.CreateAsync(A<ContentPageModel>.Ignored)).MustNotHaveHappened();
            A.CallTo(() => FakeEventMessageService.DeleteAsync(A<Guid>.Ignored)).MustNotHaveHappened();
            A.CallTo(() => FakeEventGridService.CompareAndSendEventAsync(A<ContentPageModel>.Ignored, A<ContentPageModel>.Ignored)).MustHaveHappenedOnceOrMore();

            Assert.Equal(expectedResponse, result);
        }

        [Fact]
        public async Task WebhooksServiceProcessPageLocationAsyncForUpdateReturnsSuccess()
        {
            // Arrange
            const HttpStatusCode expectedResponse = HttpStatusCode.OK;
            var exptectedGuidList = new List<Guid> { ContentIdForUpdate, Guid.NewGuid() };
            var expectedValidApiPageLocationModel = BuildValidPagesApiPageLocationModel(PageLocationIdForUpdate);
            var expectedValidContentPageModel = BuildValidContentPageModel(Constants.ContentTypeSharedContent);
            var url = new Uri("https://somewhere.com");
            var service = BuildWebhooksService();

            A.CallTo(() => FakeContentCacheService.GetContentIdsContainingContentItemId(A<Guid>.Ignored)).Returns(exptectedGuidList);
            A.CallTo(() => FakeCmsApiService.GetContentItemAsync<CmsApiPageLocationModel>(A<Uri>.Ignored)).Returns(expectedValidApiPageLocationModel);
            A.CallTo(() => FakeContentPageService.GetByIdAsync(A<Guid>.Ignored, A<string>.Ignored)).Returns(expectedValidContentPageModel);
            A.CallTo(() => FakeEventMessageService.UpdateAsync(A<ContentPageModel>.Ignored)).Returns(HttpStatusCode.OK);

            // Act
            var result = await service.ProcessContentItemAsync(url, PageLocationIdForUpdate).ConfigureAwait(false);

            // Assert
            A.CallTo(() => FakeContentCacheService.GetContentIdsContainingContentItemId(A<Guid>.Ignored)).MustHaveHappenedOnceExactly();
            A.CallTo(() => FakeCmsApiService.GetContentItemAsync<CmsApiPageLocationModel>(A<Uri>.Ignored)).MustHaveHappenedOnceOrMore();
            A.CallTo(() => FakeCmsApiService.GetContentItemAsync<CmsApiHtmlModel>(A<Uri>.Ignored)).MustNotHaveHappened();
            A.CallTo(() => FakeCmsApiService.GetContentItemAsync<CmsApiHtmlSharedModel>(A<Uri>.Ignored)).MustNotHaveHappened();
            A.CallTo(() => FakeCmsApiService.GetContentItemAsync<CmsApiSharedContentModel>(A<Uri>.Ignored)).MustNotHaveHappened();
            A.CallTo(() => FakeContentPageService.GetByIdAsync(A<Guid>.Ignored, A<string>.Ignored)).MustHaveHappenedOnceOrMore();
            A.CallTo(() => FakeMapper.Map(A<CmsApiHtmlModel>.Ignored, A<ContentItemModel>.Ignored)).MustNotHaveHappened();
            A.CallTo(() => FakeEventMessageService.UpdateAsync(A<ContentPageModel>.Ignored)).MustHaveHappenedOnceOrMore();
            A.CallTo(() => FakeEventMessageService.CreateAsync(A<ContentPageModel>.Ignored)).MustNotHaveHappened();
            A.CallTo(() => FakeEventMessageService.DeleteAsync(A<Guid>.Ignored)).MustNotHaveHappened();
            A.CallTo(() => FakeEventGridService.CompareAndSendEventAsync(A<ContentPageModel>.Ignored, A<ContentPageModel>.Ignored)).MustHaveHappenedOnceOrMore();

            Assert.Equal(expectedResponse, result);
        }

        [Fact]
        public async Task WebhooksServiceProcessContentItemAsyncForUpdateReturnsNoContentWhenNoContentIds()
        {
            // Arrange
            const HttpStatusCode expectedResponse = HttpStatusCode.NoContent;
            var exptectedEmptyGuidList = new List<Guid>();
            var url = new Uri("https://somewhere.com");
            var service = BuildWebhooksService();

            A.CallTo(() => FakeContentCacheService.GetContentIdsContainingContentItemId(A<Guid>.Ignored)).Returns(exptectedEmptyGuidList);

            // Act
            var result = await service.ProcessContentItemAsync(url, ContentItemIdForUpdate).ConfigureAwait(false);

            // Assert
            A.CallTo(() => FakeContentCacheService.GetContentIdsContainingContentItemId(A<Guid>.Ignored)).MustHaveHappenedOnceExactly();
            A.CallTo(() => FakeCmsApiService.GetContentItemAsync<CmsApiHtmlModel>(A<Uri>.Ignored)).MustNotHaveHappened();
            A.CallTo(() => FakeContentPageService.GetByIdAsync(A<Guid>.Ignored, A<string>.Ignored)).MustNotHaveHappened();
            A.CallTo(() => FakeMapper.Map(A<CmsApiHtmlModel>.Ignored, A<ContentItemModel>.Ignored)).MustNotHaveHappened();
            A.CallTo(() => FakeEventMessageService.UpdateAsync(A<ContentPageModel>.Ignored)).MustNotHaveHappened();
            A.CallTo(() => FakeEventMessageService.CreateAsync(A<ContentPageModel>.Ignored)).MustNotHaveHappened();
            A.CallTo(() => FakeEventMessageService.DeleteAsync(A<Guid>.Ignored)).MustNotHaveHappened();

            Assert.Equal(expectedResponse, result);
        }

        [Fact]
        public async Task WebhooksServiceProcessPageLocationAsyncForUpdateReturnsNoContentWhenNoApiDataResponse()
        {
            // Arrange
            const HttpStatusCode expectedResponse = HttpStatusCode.NoContent;
            var exptectedGuidList = new List<Guid> { ContentIdForUpdate, Guid.NewGuid() };
            CmsApiHtmlModel? expectedNullPagesApiContentItemModel = null;
            var url = new Uri("https://somewhere.com");
            var service = BuildWebhooksService();

            A.CallTo(() => FakeContentCacheService.GetContentIdsContainingContentItemId(A<Guid>.Ignored)).Returns(exptectedGuidList);
            A.CallTo(() => FakeCmsApiService.GetContentItemAsync<CmsApiHtmlModel>(A<Uri>.Ignored)).Returns(expectedNullPagesApiContentItemModel);

            // Act
            var result = await service.ProcessContentItemAsync(url, PageLocationIdForUpdate).ConfigureAwait(false);

            // Assert
            A.CallTo(() => FakeContentCacheService.GetContentIdsContainingContentItemId(A<Guid>.Ignored)).MustHaveHappenedOnceExactly();
            A.CallTo(() => FakeCmsApiService.GetContentItemAsync<CmsApiHtmlModel>(A<Uri>.Ignored)).MustNotHaveHappened();
            A.CallTo(() => FakeContentPageService.GetByIdAsync(A<Guid>.Ignored, A<string>.Ignored)).MustHaveHappenedOnceOrMore();
            A.CallTo(() => FakeMapper.Map(A<CmsApiHtmlModel>.Ignored, A<ContentItemModel>.Ignored)).MustNotHaveHappened();
            A.CallTo(() => FakeEventMessageService.UpdateAsync(A<ContentPageModel>.Ignored)).MustNotHaveHappened();
            A.CallTo(() => FakeEventMessageService.CreateAsync(A<ContentPageModel>.Ignored)).MustNotHaveHappened();
            A.CallTo(() => FakeEventMessageService.DeleteAsync(A<Guid>.Ignored)).MustNotHaveHappened();

            Assert.Equal(expectedResponse, result);
        }
    }
}

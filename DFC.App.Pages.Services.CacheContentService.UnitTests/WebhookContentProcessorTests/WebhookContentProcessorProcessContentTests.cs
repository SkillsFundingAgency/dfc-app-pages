using DFC.App.Pages.Data.Models;
using DFC.App.Pages.Data.Models.CmsApiModels;
using FakeItEasy;
using System;
using System.Net;
using System.Threading.Tasks;
using Xunit;

namespace DFC.App.Pages.Services.CacheContentService.UnitTests.WebhookContentProcessorTests
{
    [Trait("Category", "WebhookContentProcessor - ProcessContent Unit Tests")]
    public class WebhookContentProcessorProcessContentTests : BaseWebhookContentProcessor
    {
        [Fact]
        public async Task WebhookContentProcessorProcessContentAsyncForCreateReturnsSuccess()
        {
            // Arrange
            const HttpStatusCode expectedResponse = HttpStatusCode.Created;
            var expectedValidApiContentModel = BuildValidPagesApiContentModel();
            var expectedValidContentPageModel = BuildValidContentPageModel();
            var url = new Uri("https://somewhere.com");
            var service = BuildWebhookContentProcessor();

            A.CallTo(() => FakeCmsApiService.GetItemAsync<CmsApiDataModel>(A<Uri>.Ignored)).Returns(expectedValidApiContentModel);
            A.CallTo(() => FakeMapper.Map<ContentPageModel>(A<CmsApiDataModel>.Ignored)).Returns(expectedValidContentPageModel);
            A.CallTo(() => FakeContentPageService.GetByIdAsync(A<Guid>.Ignored, A<string>.Ignored)).Returns(expectedValidContentPageModel);
            A.CallTo(() => FakeEventMessageService.UpdateAsync(A<ContentPageModel>.Ignored)).Returns(HttpStatusCode.NotFound);
            A.CallTo(() => FakeEventMessageService.CreateAsync(A<ContentPageModel>.Ignored)).Returns(HttpStatusCode.Created);

            // Act
            var result = await service.ProcessContentAsync(url, ContentIdForCreate).ConfigureAwait(false);

            // Assert
            A.CallTo(() => FakeCmsApiService.GetItemAsync<CmsApiDataModel>(A<Uri>.Ignored)).MustHaveHappenedOnceExactly();
            A.CallTo(() => FakeMapper.Map<ContentPageModel>(A<CmsApiDataModel>.Ignored)).MustHaveHappenedOnceExactly();
            A.CallTo(() => FakeContentPageService.GetByIdAsync(A<Guid>.Ignored, A<string>.Ignored)).MustHaveHappenedOnceExactly();
            A.CallTo(() => FakeEventMessageService.UpdateAsync(A<ContentPageModel>.Ignored)).MustHaveHappenedOnceExactly();
            A.CallTo(() => FakeEventMessageService.CreateAsync(A<ContentPageModel>.Ignored)).MustHaveHappenedOnceExactly();
            A.CallTo(() => FakeEventGridService.CompareAndSendEventAsync(A<ContentPageModel>.Ignored, A<ContentPageModel>.Ignored)).MustHaveHappenedOnceExactly();
            A.CallTo(() => FakeEventMessageService.DeleteAsync(A<Guid>.Ignored)).MustNotHaveHappened();

            Assert.Equal(expectedResponse, result);
        }

        [Fact]
        public async Task WebhookContentProcessorProcessContentAsyncForUpdateReturnsSuccess()
        {
            // Arrange
            const HttpStatusCode expectedResponse = HttpStatusCode.OK;
            var expectedValidApiContentModel = BuildValidPagesApiContentModel();
            var expectedValidContentPageModel = BuildValidContentPageModel();
            var url = new Uri("https://somewhere.com");
            var service = BuildWebhookContentProcessor();

            A.CallTo(() => FakeCmsApiService.GetItemAsync<CmsApiDataModel>(A<Uri>.Ignored)).Returns(expectedValidApiContentModel);
            A.CallTo(() => FakeMapper.Map<ContentPageModel>(A<CmsApiDataModel>.Ignored)).Returns(expectedValidContentPageModel);
            A.CallTo(() => FakeContentPageService.GetByIdAsync(A<Guid>.Ignored, A<string>.Ignored)).Returns(expectedValidContentPageModel);
            A.CallTo(() => FakeEventMessageService.UpdateAsync(A<ContentPageModel>.Ignored)).Returns(HttpStatusCode.OK);

            // Act
            var result = await service.ProcessContentAsync(url, ContentIdForCreate).ConfigureAwait(false);

            // Assert
            A.CallTo(() => FakeCmsApiService.GetItemAsync<CmsApiDataModel>(A<Uri>.Ignored)).MustHaveHappenedOnceExactly();
            A.CallTo(() => FakeMapper.Map<ContentPageModel>(A<CmsApiDataModel>.Ignored)).MustHaveHappenedOnceExactly();
            A.CallTo(() => FakeContentPageService.GetByIdAsync(A<Guid>.Ignored, A<string>.Ignored)).MustHaveHappenedOnceExactly();
            A.CallTo(() => FakeEventMessageService.UpdateAsync(A<ContentPageModel>.Ignored)).MustHaveHappenedOnceExactly();
            A.CallTo(() => FakeEventMessageService.CreateAsync(A<ContentPageModel>.Ignored)).MustNotHaveHappened();
            A.CallTo(() => FakeEventGridService.CompareAndSendEventAsync(A<ContentPageModel>.Ignored, A<ContentPageModel>.Ignored)).MustHaveHappenedOnceExactly();
            A.CallTo(() => FakeEventMessageService.DeleteAsync(A<Guid>.Ignored)).MustNotHaveHappened();

            Assert.Equal(expectedResponse, result);
        }

        [Fact]
        public async Task WebhookContentProcessorProcessContentAsyncForUpdateReturnsNoContent()
        {
            // Arrange
            const HttpStatusCode expectedResponse = HttpStatusCode.NoContent;
            var expectedValidApiContentModel = BuildValidPagesApiContentModel();
            ContentPageModel? expectedValidContentPageModel = default;
            var url = new Uri("https://somewhere.com");
            var service = BuildWebhookContentProcessor();

            A.CallTo(() => FakeCmsApiService.GetItemAsync<CmsApiDataModel>(A<Uri>.Ignored)).Returns(expectedValidApiContentModel);
            A.CallTo(() => FakeMapper.Map<ContentPageModel?>(A<CmsApiDataModel>.Ignored)).Returns(expectedValidContentPageModel);

            // Act
            var result = await service.ProcessContentAsync(url, ContentIdForCreate).ConfigureAwait(false);

            // Assert
            A.CallTo(() => FakeCmsApiService.GetItemAsync<CmsApiDataModel>(A<Uri>.Ignored)).MustHaveHappenedOnceExactly();
            A.CallTo(() => FakeMapper.Map<ContentPageModel>(A<CmsApiDataModel>.Ignored)).MustHaveHappenedOnceExactly();
            A.CallTo(() => FakeContentPageService.GetByIdAsync(A<Guid>.Ignored, A<string>.Ignored)).MustNotHaveHappened();
            A.CallTo(() => FakeEventMessageService.UpdateAsync(A<ContentPageModel>.Ignored)).MustNotHaveHappened();
            A.CallTo(() => FakeEventMessageService.CreateAsync(A<ContentPageModel>.Ignored)).MustNotHaveHappened();
            A.CallTo(() => FakeEventGridService.CompareAndSendEventAsync(A<ContentPageModel>.Ignored, A<ContentPageModel>.Ignored)).MustNotHaveHappened();
            A.CallTo(() => FakeEventMessageService.DeleteAsync(A<Guid>.Ignored)).MustNotHaveHappened();

            Assert.Equal(expectedResponse, result);
        }

        [Fact]
        public async Task WebhookContentProcessorProcessContentAsyncForUpdateReturnsBadRequest()
        {
            // Arrange
            const HttpStatusCode expectedResponse = HttpStatusCode.BadRequest;
            var expectedValidApiContentModel = BuildValidPagesApiContentModel();
            var expectedValidContentPageModel = new ContentPageModel();
            var url = new Uri("https://somewhere.com");
            var service = BuildWebhookContentProcessor();

            A.CallTo(() => FakeCmsApiService.GetItemAsync<CmsApiDataModel>(A<Uri>.Ignored)).Returns(expectedValidApiContentModel);
            A.CallTo(() => FakeMapper.Map<ContentPageModel>(A<CmsApiDataModel>.Ignored)).Returns(expectedValidContentPageModel);

            // Act
            var result = await service.ProcessContentAsync(url, ContentIdForCreate).ConfigureAwait(false);

            // Assert
            A.CallTo(() => FakeCmsApiService.GetItemAsync<CmsApiDataModel>(A<Uri>.Ignored)).MustHaveHappenedOnceExactly();
            A.CallTo(() => FakeMapper.Map<ContentPageModel>(A<CmsApiDataModel>.Ignored)).MustHaveHappenedOnceExactly();
            A.CallTo(() => FakeContentPageService.GetByIdAsync(A<Guid>.Ignored, A<string>.Ignored)).MustNotHaveHappened();
            A.CallTo(() => FakeEventMessageService.UpdateAsync(A<ContentPageModel>.Ignored)).MustNotHaveHappened();
            A.CallTo(() => FakeEventMessageService.CreateAsync(A<ContentPageModel>.Ignored)).MustNotHaveHappened();
            A.CallTo(() => FakeEventGridService.CompareAndSendEventAsync(A<ContentPageModel>.Ignored, A<ContentPageModel>.Ignored)).MustNotHaveHappened();
            A.CallTo(() => FakeEventMessageService.DeleteAsync(A<Guid>.Ignored)).MustNotHaveHappened();

            Assert.Equal(expectedResponse, result);
        }
    }
}

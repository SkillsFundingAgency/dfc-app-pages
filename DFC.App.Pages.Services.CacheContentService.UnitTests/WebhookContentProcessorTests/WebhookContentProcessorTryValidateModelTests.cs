using DFC.App.Pages.Data.Models;
using System;
using Xunit;

namespace DFC.App.Pages.Services.CacheContentService.UnitTests.WebhookContentProcessorTests
{
    [Trait("Category", "WebhookContentProcessor - TryValidateModel Unit Tests")]
    public class WebhookContentProcessorTryValidateModelTests : BaseWebhookContentProcessor
    {
        [Fact]
        public void WebhookContentProcessorTryValidateModelForCreateReturnsSuccess()
        {
            // Arrange
            const bool expectedResponse = true;
            var expectedValidContentPageModel = BuildValidContentPageModel();
            var service = BuildWebhookContentProcessor();

            // Act
            var result = service.TryValidateModel(expectedValidContentPageModel);

            // Assert
            Assert.Equal(expectedResponse, result);
        }

        [Fact]
        public void WebhookContentProcessorTryValidateModelForUpdateReturnsFailure()
        {
            // Arrange
            const bool expectedResponse = false;
            var expectedInvalidContentPageModel = new ContentPageModel();
            var service = BuildWebhookContentProcessor();

            // Act
            var result = service.TryValidateModel(expectedInvalidContentPageModel);

            // Assert
            Assert.Equal(expectedResponse, result);
        }

        [Fact]
        public void WebhookContentProcessorTryValidateModelRaisesExceptionForNullContentPageModel()
        {
            // Arrange
            ContentPageModel? nullContentPageModel = null;
            var service = BuildWebhookContentProcessor();

            // Act
            var exceptionResult = Assert.Throws<ArgumentNullException>(() => service.TryValidateModel(nullContentPageModel));

            // Assert
            Assert.Equal("Value cannot be null. (Parameter 'contentPageModel')", exceptionResult.Message);
        }
    }
}

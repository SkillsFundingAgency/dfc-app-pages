using DFC.App.Pages.Data.Models;
using System;
using Xunit;

namespace DFC.App.Pages.Services.CacheContentService.UnitTests.WebhooksServiceTests
{
    [Trait("Category", "Webhooks Service TryValidateModel Unit Tests")]
    public class WebhooksServiceTryValidateModelTests : BaseWebhooksServiceTests
    {
        [Fact]
        public void WebhooksServiceTryValidateModelForCreateReturnsSuccess()
        {
            // Arrange
            const bool expectedResponse = true;
            var expectedValidContentPageModel = BuildValidContentPageModel();
            var service = BuildWebhooksService();

            // Act
            var result = service.TryValidateModel(expectedValidContentPageModel);

            // Assert
            Assert.Equal(expectedResponse, result);
        }

        [Fact]
        public void WebhooksServiceTryValidateModelForUpdateReturnsFailure()
        {
            // Arrange
            const bool expectedResponse = false;
            var expectedInvalidContentPageModel = new ContentPageModel();
            var service = BuildWebhooksService();

            // Act
            var result = service.TryValidateModel(expectedInvalidContentPageModel);

            // Assert
            Assert.Equal(expectedResponse, result);
        }

        [Fact]
        public void WebhooksServiceTryValidateModelRaisesExceptionForNullContentPageModel()
        {
            // Arrange
            ContentPageModel? nullContentPageModel = null;
            var service = BuildWebhooksService();

            // Act
            var exceptionResult = Assert.Throws<ArgumentNullException>(() => service.TryValidateModel(nullContentPageModel));

            // Assert
            Assert.Equal("Value cannot be null. (Parameter 'contentPageModel')", exceptionResult.Message);
        }
    }
}

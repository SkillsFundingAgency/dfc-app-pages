using FakeItEasy;
using Microsoft.AspNetCore.Mvc;
using System.Net;
using Xunit;

namespace DFC.App.Pages.UnitTests.ControllerTests.PagesControllerTests
{
    [Trait("Category", "Pages Controller Unit Tests")]
    public class PagesControllerBodyFooterTests : BasePagesControllerTests
    {
        [Theory]
        [MemberData(nameof(JsonMediaTypes))]
        public void PagesControllerBodyFooterJsonReturnsSuccess(string mediaTypeName)
        {
            // Arrange
            const string article = "an-article-name";
            var controller = BuildPagesController(mediaTypeName);

            // Act
            var result = controller.BodyFooter(article);

            // Assert
            var statusResult = Assert.IsType<NoContentResult>(result);

            A.Equals((int)HttpStatusCode.NoContent, statusResult.StatusCode);

            controller.Dispose();
        }

        [Theory]
        [MemberData(nameof(JsonMediaTypes))]
        public void PagesControllerBodyFooterWithNullArticleJsonReturnsSuccess(string mediaTypeName)
        {
            // Arrange
            const string? article = null;
            var controller = BuildPagesController(mediaTypeName);

            // Act
            var result = controller.BodyFooter(article);

            // Assert
            var statusResult = Assert.IsType<NoContentResult>(result);

            A.Equals((int)HttpStatusCode.NoContent, statusResult.StatusCode);

            controller.Dispose();
        }
    }
}

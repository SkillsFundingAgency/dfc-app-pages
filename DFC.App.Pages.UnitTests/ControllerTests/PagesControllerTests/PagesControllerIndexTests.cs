using DFC.App.Pages.ViewModels;
using DFC.Common.SharedContent.Pkg.Netcore.Model.ContentItems;
using DFC.Compui.Cosmos.Models;
using FakeItEasy;
using Microsoft.AspNetCore.Mvc;
using System.Net;
using System.Threading.Tasks;
using Xunit;

namespace DFC.App.Pages.UnitTests.ControllerTests.PagesControllerTests
{
    [Trait("Category", "Pages Controller Unit Tests")]
    public class PagesControllerIndexTests : BasePagesControllerTests
    {
        [Theory]
        [MemberData(nameof(JsonMediaTypes))]
        [MemberData(nameof(HtmlMediaTypes))]
        public async Task PagesControllerIndexHtmlReturnsNoContent(string mediaTypeName)
        {
            // Arrange
            var expected = new Page()
            {
                Herobanner = new()
                {
                    Html = "This is a hero banner",
                },

            };
            var controller = BuildPagesController(mediaTypeName);

            A.CallTo(() => FakeSharedContentRedisInterface.GetDataAsyncWithExpiry<Page>("PageTest", "PUBLISHED", 4)).Returns(expected);
            A.CallTo(() => FakeMapper.Map<IndexDocumentViewModel>(A<ContentPageModel>.Ignored)).Returns(A.Fake<IndexDocumentViewModel>());

            // Act
            var result = await controller.Index().ConfigureAwait(false);
            var statusResult = Assert.IsType<NoContentResult>(result);

            A.Equals((int)HttpStatusCode.NoContent, statusResult.StatusCode);

            controller.Dispose();
        }
    }
}
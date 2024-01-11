using DFC.App.Pages.Data.Models;
using DFC.App.Pages.Models;
using DFC.App.Pages.ViewModels;
using FakeItEasy;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using System.Net;
using System.Threading.Tasks;
using Xunit;

namespace DFC.App.Pages.UnitTests.ControllerTests.PagesControllerTests
{
    [Trait("Category", "Pages Controller Unit Tests")]
    public class PagesControllerBodyTests : BasePagesControllerTests
    {
        [Theory]
        [MemberData(nameof(HtmlMediaTypes))]
        public async Task PagesControllerBodyHtmlReturnsNotFound(string mediaTypeName)
        {
            // Arrange
            var pageRequestModel = new PageRequestModel
            {
                Location1 = "a-location-name",
                Location2 = "an-article-name",
            };
            var expectedResult = new ContentPageModel() { Content = "<h1>A document</h1>", PageLocation = "/" + pageRequestModel.Location1, CanonicalName = pageRequestModel.Location2 };
            var controller = BuildPagesController(mediaTypeName);

            A.CallTo(() => FakePagesControlerHelpers.GetContentPageFromPageServiceAsync(A<string>.Ignored, A<string>.Ignored)).Returns(expectedResult);
            A.CallTo(() => FakeMapper.Map(A<ContentPageModel>.Ignored, A<BodyViewModel>.Ignored)).Returns(A.Fake<BodyViewModel>());

            // Act
            var result = await controller.Body(pageRequestModel).ConfigureAwait(false);

            var statusResult = Assert.IsType<NotFoundResult>(result);

            A.Equals((int)HttpStatusCode.NotFound, statusResult.StatusCode);

            controller.Dispose();
        }
    }
}

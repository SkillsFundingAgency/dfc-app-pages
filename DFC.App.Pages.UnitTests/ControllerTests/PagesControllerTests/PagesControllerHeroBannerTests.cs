using DFC.App.Pages.Data.Models;
using DFC.App.Pages.Models;
using DFC.App.Pages.ViewModels;
using FakeItEasy;
using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Mvc;
using System.Net;
using System.Threading.Tasks;
using Xunit;

namespace DFC.App.Pages.UnitTests.ControllerTests.PagesControllerTests
{
    [Trait("Category", "Pages Controller Unit Tests")]
    public class PagesControllerHeroBannerTests : BasePagesControllerTests
    {
        [Theory]
        [MemberData(nameof(HtmlMediaTypes))]
        public async Task PagesControllerHeroBannerHtmlReturnsSuccess(string mediaTypeName)
        {
            // Arrange
            var pageRequestModel = new PageRequestModel
            {
                Location1 = "a-location-name",
                Location2 = "an-article-name",
            };
            var expectedResult = new ContentPageModel() { HeroBanner = "This is a hero banner" };
            var expectedHeroBanner = new HeroBannerViewModel { Content = new HtmlString(expectedResult.HeroBanner), };
            var controller = BuildPagesController(mediaTypeName);

            A.CallTo(() => FakePagesControllerHelper.GetContentPageFromSharedAsync(A<string>.Ignored, A<string>.Ignored)).Returns(expectedResult);
            A.CallTo(() => FakeMapper.Map<HeroBannerViewModel>(A<ContentPageModel>.Ignored)).Returns(expectedHeroBanner);

            // Act
            var result = await controller.HeroBanner(pageRequestModel).ConfigureAwait(false);

            var viewResult = Assert.IsType<ViewResult>(result);
            var model = Assert.IsAssignableFrom<HeroBannerViewModel>(viewResult.ViewData.Model);

            controller.Dispose();
        }

        [Theory]
        [MemberData(nameof(JsonMediaTypes))]
        public async Task PagesControllerHeroBannerJsonReturnsSuccess(string mediaTypeName)
        {
            // Arrange
            var pageRequestModel = new PageRequestModel
            {
                Location1 = "a-location-name",
                Location2 = "an-article-name",
            };
            var expectedResult = new ContentPageModel() { HeroBanner = "This is a hero banner" };
            var expectedHeroBanner = new HeroBannerViewModel { Content = new HtmlString(expectedResult.HeroBanner), };
            var controller = BuildPagesController(mediaTypeName);

            A.CallTo(() => FakePagesControllerHelper.GetContentPageFromSharedAsync(A<string>.Ignored, A<string>.Ignored)).Returns(expectedResult);
            A.CallTo(() => FakeMapper.Map<HeroBannerViewModel>(A<ContentPageModel>.Ignored)).Returns(expectedHeroBanner);

            // Act
            var result = await controller.HeroBanner(pageRequestModel).ConfigureAwait(false);

            var jsonResult = Assert.IsType<OkObjectResult>(result);
            var model = Assert.IsAssignableFrom<HeroBannerViewModel>(jsonResult.Value);

            controller.Dispose();
        }

        [Theory]
        [MemberData(nameof(HtmlMediaTypes))]
        public async Task PagesControllerHeroBannerHtmlReturnsSuccessWhenNoData(string mediaTypeName)
        {
            // Arrange
            var pageRequestModel = new PageRequestModel
            {
                Location1 = "a-location-name",
                Location2 = "an-article-name",
            };
            ContentPageModel? expectedResult = null;
            var controller = BuildPagesController(mediaTypeName);

            A.CallTo(() => FakePagesControllerHelper.GetContentPageFromSharedAsync(A<string>.Ignored, A<string>.Ignored)).Returns(expectedResult);

            // Act
            var result = await controller.HeroBanner(pageRequestModel).ConfigureAwait(false);

            var viewResult = Assert.IsType<ViewResult>(result);
            var model = Assert.IsAssignableFrom<HeroBannerViewModel>(viewResult.ViewData.Model);

            controller.Dispose();
        }

        //TODO: 
        [Theory]
        [MemberData(nameof(JsonMediaTypes))]
        public async Task PagesControllerHeroBannerJsonReturnsSuccessWhenNoData(string mediaTypeName)
        {
            // Arrange
            var pageRequestModel = new PageRequestModel
            {
                Location1 = "a-location-name",
                Location2 = "an-article-name",
            };
            ContentPageModel? expectedResult = null;
            var controller = BuildPagesController(mediaTypeName);

            A.CallTo(() => FakePagesControllerHelper.GetContentPageFromSharedAsync(A<string>.Ignored, A<string>.Ignored)).Returns(expectedResult);

            // Act
            var result = await controller.HeroBanner(pageRequestModel).ConfigureAwait(false);

            var jsonResult = Assert.IsType<OkObjectResult>(result);
            var model = Assert.IsAssignableFrom<HeroBannerViewModel>(jsonResult.Value);

            controller.Dispose();
        }

        [Theory]
        [MemberData(nameof(InvalidMediaTypes))]
        public async Task PagesControllerHeroBannerReturnsNotAcceptable(string mediaTypeName)
        {
            // Arrange
            var pageRequestModel = new PageRequestModel
            {
                Location1 = "a-location-name",
                Location2 = "an-article-name",
            };
            var expectedResult = new ContentPageModel() { PageLocation = "/" + pageRequestModel.Location1, CanonicalName = pageRequestModel.Location2 };
            var controller = BuildPagesController(mediaTypeName);

            A.CallTo(() => FakePagesControllerHelper.GetContentPageFromSharedAsync(A<string>.Ignored, A<string>.Ignored)).Returns(expectedResult);

            // Act
            var result = await controller.HeroBanner(pageRequestModel).ConfigureAwait(false);

            var statusResult = Assert.IsType<StatusCodeResult>(result);

            A.Equals((int)HttpStatusCode.NotAcceptable, statusResult.StatusCode);

            controller.Dispose();
        }
    }
}
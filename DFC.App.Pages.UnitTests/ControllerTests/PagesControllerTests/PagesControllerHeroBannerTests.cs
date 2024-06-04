using DFC.App.Pages.Data.Models;
using DFC.App.Pages.Models;
using DFC.App.Pages.ViewModels;
using DFC.Common.SharedContent.Pkg.Netcore.Model.ContentItems;
using FakeItEasy;
using FluentAssertions;
using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
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
            var expected = new Page()
            {
                Herobanner = new()
                {
                    Html = "This is a hero banner",
                },

            };
            var expectedHeroBanner = new HeroBannerViewModel { Content = new HtmlString(expected.Herobanner.Html), };
            var controller = BuildPagesController(mediaTypeName);

            A.CallTo(() => FakeSharedContentRedisInterface.GetDataAsync<Page>("PageTest", "PUBLISHED",4)).Returns(expected);
            A.CallTo(() => FakeMapper.Map<HeroBannerViewModel>(A<Page>.Ignored)).Returns(expectedHeroBanner);

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
            var expected = new Page()
            {
                Herobanner = new()
                {
                    Html = "This is a hero banner",
                },

            };
            var expectedHeroBanner = new HeroBannerViewModel { Content = new HtmlString(expected.Herobanner.Html), };
            var controller = BuildPagesController(mediaTypeName);

            A.CallTo(() => FakeSharedContentRedisInterface.GetDataAsync<Page>("PageTest", "PUBLISHED", 4)).Returns(expected);
            A.CallTo(() => FakeMapper.Map<HeroBannerViewModel>(A<Page>.Ignored)).Returns(expectedHeroBanner);

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
            Page? expectedResult = null;
            var controller = BuildPagesController(mediaTypeName);

            A.CallTo(() => FakeSharedContentRedisInterface.GetDataAsync<Page>("PageTest", "PUBLISHED", 4)).Returns(expectedResult);

            // Act
            var result = await controller.HeroBanner(pageRequestModel).ConfigureAwait(false);

            var viewResult = Assert.IsType<ViewResult>(result);
            var model = Assert.IsAssignableFrom<HeroBannerViewModel>(viewResult.ViewData.Model);

            controller.Dispose();
        }

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
            Page? expectedResult = null;
            var controller = BuildPagesController(mediaTypeName);

            A.CallTo(() => FakeSharedContentRedisInterface.GetDataAsync<Page>("PageTest", "PUBLISHED", 4)).Returns(expectedResult);

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
            var expected = new Page()
            {
                PageLocation = new()
                {
                    FullUrl = "/" + pageRequestModel.Location1,
                    UrlName = "location1",
                },
                DisplayText = pageRequestModel.Location2,

            }; var controller = BuildPagesController(mediaTypeName);

            A.CallTo(() => FakeSharedContentRedisInterface.GetDataAsync<Page>("PageTest", "PUBLISHED", 4)).Returns(expected);

            // Act
            var result = await controller.HeroBanner(pageRequestModel).ConfigureAwait(false);

            var statusResult = Assert.IsType<StatusCodeResult>(result);

            A.Equals((int)HttpStatusCode.NotAcceptable, statusResult.StatusCode);

            controller.Dispose();
        }
    }
}
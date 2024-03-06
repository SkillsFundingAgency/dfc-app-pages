using DFC.App.Pages.Data.Models;
using DFC.App.Pages.Models;
using DFC.App.Pages.ViewModels;
using DFC.Common.SharedContent.Pkg.Netcore.Model.ContentItems;
using FakeItEasy;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using System.Net;
using System.Threading.Tasks;
using Xunit;

namespace DFC.App.Pages.UnitTests.ControllerTests.PagesControllerTests
{
    [Trait("Category", "Pages Controller Unit Tests")]
    public class PagesControllerHeadTests : BasePagesControllerTests
    {
        [Theory]
        [MemberData(nameof(HtmlMediaTypes))]
        public async Task PagesControllerHeadHtmlReturnsSuccess(string mediaTypeName)
        {
            // Arrange
            var pageRequestModel = new PageRequestModel
            {
                Location1 = "a-location-name",
                Location2 = "an-article-name",
            };
            var controller = BuildPagesController(mediaTypeName);

            A.CallTo(() => FakeMapper.Map(A<ContentPageModel>.Ignored, A<HeadViewModel>.Ignored)).Returns(A.Fake<HeadViewModel>());

            // Act
            var result = await controller.Head(pageRequestModel).ConfigureAwait(false);

            var viewResult = Assert.IsType<ViewResult>(result);
            var model = Assert.IsAssignableFrom<HeadViewModel>(viewResult.ViewData.Model);

            controller.Dispose();
        }

        [Theory]
        [MemberData(nameof(JsonMediaTypes))]
        public async Task PagesControllerHeadJsonReturnsSuccess(string mediaTypeName)
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

            };

            var controller = BuildPagesController(mediaTypeName);

            A.CallTo(() => FakeSharedContentRedisInterface.GetDataAsync<Page>("PageTest", "PUBLISHED")).Returns(expected);
            A.CallTo(() => FakeMapper.Map(A<ContentPageModel>.Ignored, A<HeadViewModel>.Ignored)).Returns(A.Fake<HeadViewModel>());

            // Act
            var result = await controller.Head(pageRequestModel).ConfigureAwait(false);

            var jsonResult = Assert.IsType<OkObjectResult>(result);
            var model = Assert.IsAssignableFrom<HeadViewModel>(jsonResult.Value);

            controller.Dispose();
        }

        [Theory]
        [MemberData(nameof(HtmlMediaTypes))]
        public async Task PagesControllerHeadHtmlWithNullArticleReturnsSuccess(string mediaTypeName)
        {
            // Arrange
            var pageRequestModel = new PageRequestModel
            {
                Location1 = "a-location-name",
                Location2 = "an-article-name",
            };
            Page? expectedResult = null;
            var controller = BuildPagesController(mediaTypeName);

            A.CallTo(() => FakeSharedContentRedisInterface.GetDataAsync<Page>("PageTest", "PUBLISHED")).Returns(expectedResult);
            A.CallTo(() => FakeMapper.Map(A<ContentPageModel>.Ignored, A<HeadViewModel>.Ignored)).Returns(A.Fake<HeadViewModel>());

            // Act
            var result = await controller.Head(pageRequestModel).ConfigureAwait(false);

            var viewResult = Assert.IsType<ViewResult>(result);
            _ = Assert.IsAssignableFrom<HeadViewModel>(viewResult.ViewData.Model);

            controller.Dispose();
        }

        [Theory]
        [MemberData(nameof(JsonMediaTypes))]
        public async Task PagesControllerHeadJsonWithNullArticleReturnsSuccess(string mediaTypeName)
        {
            // Arrange
            var pageRequestModel = new PageRequestModel
            {
                Location1 = "a-location-name",
                Location2 = "an-article-name",
            };
            Page? expectedResult = null;
            var controller = BuildPagesController(mediaTypeName);

            A.CallTo(() => FakeSharedContentRedisInterface.GetDataAsync<Page>("PageTest", "PUBLISHED")).Returns(expectedResult);
            A.CallTo(() => FakeMapper.Map(A<ContentPageModel>.Ignored, A<HeadViewModel>.Ignored)).Returns(A.Fake<HeadViewModel>());

            // Act
            var result = await controller.Head(pageRequestModel).ConfigureAwait(false);

            var jsonResult = Assert.IsType<OkObjectResult>(result);
            _ = Assert.IsAssignableFrom<HeadViewModel>(jsonResult.Value);

            controller.Dispose();
        }

        [Theory]
        [MemberData(nameof(HtmlMediaTypes))]
        public async Task PagesControllerHeadHtmlReturnsSuccessWhenNoData(string mediaTypeName)
        {
            // Arrange
            var pageRequestModel = new PageRequestModel
            {
                Location1 = "a-location-name",
                Location2 = "an-article-name",
            };
            Page? expectedResult = null;
            var controller = BuildPagesController(mediaTypeName);

            A.CallTo(() => FakeSharedContentRedisInterface.GetDataAsync<Page>("PageTest", "PUBLISHED")).Returns(expectedResult);

            // Act
            var result = await controller.Head(pageRequestModel).ConfigureAwait(false);

            var viewResult = Assert.IsType<ViewResult>(result);
            var model = Assert.IsAssignableFrom<HeadViewModel>(viewResult.ViewData.Model);

            model.CanonicalUrl.Should().BeNull();

            controller.Dispose();
        }

        [Theory]
        [MemberData(nameof(JsonMediaTypes))]
        public async Task PagesControllerHeadJsonReturnsSuccessWhenNoData(string mediaTypeName)
        {
            // Arrange
            var pageRequestModel = new PageRequestModel
            {
                Location1 = "a-location-name",
                Location2 = "an-article-name",
            };
            Page? expectedResult = null;
            var controller = BuildPagesController(mediaTypeName);

            A.CallTo(() => FakeSharedContentRedisInterface.GetDataAsync<Page>("PageTest", "PUBLISHED")).Returns(expectedResult);

            // Act
            var result = await controller.Head(pageRequestModel).ConfigureAwait(false);

            var jsonResult = Assert.IsType<OkObjectResult>(result);
            var model = Assert.IsAssignableFrom<HeadViewModel>(jsonResult.Value);

            model.CanonicalUrl.Should().BeNull();

            controller.Dispose();
        }

        [Theory]
        [MemberData(nameof(InvalidMediaTypes))]
        public async Task PagesControllerHeadReturnsNotAcceptable(string mediaTypeName)
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

            A.CallTo(() => FakeSharedContentRedisInterface.GetDataAsync<Page>("PageTest", "PUBLISHED")).Returns(expected);
            A.CallTo(() => FakeMapper.Map(A<ContentPageModel>.Ignored, A<HeadViewModel>.Ignored)).Returns(A.Fake<HeadViewModel>());

            // Act
            var result = await controller.Head(pageRequestModel).ConfigureAwait(false);

            var statusResult = Assert.IsType<StatusCodeResult>(result);

            A.Equals((int)HttpStatusCode.NotAcceptable, statusResult.StatusCode);

            controller.Dispose();
        }
    }
}
using DFC.App.Pages.Data.Models;
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
    public class PagesControllerHtmlHeadTests : BasePagesControllerTests
    {
        [Theory]
        [MemberData(nameof(HtmlMediaTypes))]
        public async Task PagesControllerHtmlHeadHtmlReturnsSuccess(string mediaTypeName)
        {
            // Arrange
            const string article = "an-article-name";
            var expectedResult = A.Fake<ContentPageModel>();
            var controller = BuildPagesController(mediaTypeName);

            expectedResult.CanonicalName = article;

            A.CallTo(() => FakeContentPageService.GetByNameAsync(A<string>.Ignored)).Returns(expectedResult);
            A.CallTo(() => FakeMapper.Map(A<ContentPageModel>.Ignored, A<HtmlHeadViewModel>.Ignored)).Returns(A.Fake<HtmlHeadViewModel>());

            // Act
            var result = await controller.HtmlHead(article).ConfigureAwait(false);

            // Assert
            A.CallTo(() => FakeContentPageService.GetByNameAsync(A<string>.Ignored)).MustHaveHappenedOnceExactly();
            A.CallTo(() => FakeMapper.Map(A<ContentPageModel>.Ignored, A<HtmlHeadViewModel>.Ignored)).MustHaveHappenedOnceExactly();

            var viewResult = Assert.IsType<ViewResult>(result);
            var model = Assert.IsAssignableFrom<HtmlHeadViewModel>(viewResult.ViewData.Model);

            model.CanonicalUrl.Should().NotBeNull();

            controller.Dispose();
        }

        [Theory]
        [MemberData(nameof(JsonMediaTypes))]
        public async Task PagesControllerHtmlHeadJsonReturnsSuccess(string mediaTypeName)
        {
            // Arrange
            const string article = "an-article-name";
            var expectedResult = A.Fake<ContentPageModel>();
            var controller = BuildPagesController(mediaTypeName);

            expectedResult.CanonicalName = article;

            A.CallTo(() => FakeContentPageService.GetByNameAsync(A<string>.Ignored)).Returns(expectedResult);
            A.CallTo(() => FakeMapper.Map(A<ContentPageModel>.Ignored, A<HtmlHeadViewModel>.Ignored)).Returns(A.Fake<HtmlHeadViewModel>());

            // Act
            var result = await controller.HtmlHead(article).ConfigureAwait(false);

            // Assert
            A.CallTo(() => FakeContentPageService.GetByNameAsync(A<string>.Ignored)).MustHaveHappenedOnceExactly();
            A.CallTo(() => FakeMapper.Map(A<ContentPageModel>.Ignored, A<HtmlHeadViewModel>.Ignored)).MustHaveHappenedOnceExactly();

            var jsonResult = Assert.IsType<OkObjectResult>(result);
            var model = Assert.IsAssignableFrom<HtmlHeadViewModel>(jsonResult.Value);

            model.CanonicalUrl.Should().NotBeNull();

            controller.Dispose();
        }

        [Theory]
        [MemberData(nameof(HtmlMediaTypes))]
        public async Task PagesControllerHtmlHeadHtmlWithNullArticleReturnsSuccess(string mediaTypeName)
        {
            // Arrange
            const string? article = null;
            ContentPageModel? expectedResult = null;
            var controller = BuildPagesController(mediaTypeName);

            A.CallTo(() => FakeContentPageService.GetByNameAsync(A<string>.Ignored)).Returns(expectedResult);
            A.CallTo(() => FakeMapper.Map(A<ContentPageModel>.Ignored, A<HtmlHeadViewModel>.Ignored)).Returns(A.Fake<HtmlHeadViewModel>());

            // Act
            var result = await controller.HtmlHead(article).ConfigureAwait(false);

            // Assert
            A.CallTo(() => FakeContentPageService.GetByNameAsync(A<string>.Ignored)).MustHaveHappenedOnceExactly();

            var viewResult = Assert.IsType<ViewResult>(result);
            _ = Assert.IsAssignableFrom<HtmlHeadViewModel>(viewResult.ViewData.Model);

            controller.Dispose();
        }

        [Theory]
        [MemberData(nameof(JsonMediaTypes))]
        public async Task PagesControllerHtmlHeadJsonWithNullArticleReturnsSuccess(string mediaTypeName)
        {
            // Arrange
            const string? article = null;
            ContentPageModel? expectedResult = null;
            var controller = BuildPagesController(mediaTypeName);

            A.CallTo(() => FakeContentPageService.GetByNameAsync(A<string>.Ignored)).Returns(expectedResult);
            A.CallTo(() => FakeMapper.Map(A<ContentPageModel>.Ignored, A<HtmlHeadViewModel>.Ignored)).Returns(A.Fake<HtmlHeadViewModel>());

            // Act
            var result = await controller.HtmlHead(article).ConfigureAwait(false);

            // Assert
            A.CallTo(() => FakeContentPageService.GetByNameAsync(A<string>.Ignored)).MustHaveHappenedOnceExactly();

            var jsonResult = Assert.IsType<OkObjectResult>(result);
            _ = Assert.IsAssignableFrom<HtmlHeadViewModel>(jsonResult.Value);

            controller.Dispose();
        }

        [Theory]
        [MemberData(nameof(HtmlMediaTypes))]
        public async Task PagesControllerHtmlHeadHtmlReturnsSuccessWhenNoData(string mediaTypeName)
        {
            // Arrange
            const string article = "an-article-name";
            ContentPageModel? expectedResult = null;
            var controller = BuildPagesController(mediaTypeName);

            A.CallTo(() => FakeContentPageService.GetByNameAsync(A<string>.Ignored)).Returns(expectedResult);

            // Act
            var result = await controller.HtmlHead(article).ConfigureAwait(false);

            // Assert
            A.CallTo(() => FakeContentPageService.GetByNameAsync(A<string>.Ignored)).MustHaveHappenedOnceExactly();

            var viewResult = Assert.IsType<ViewResult>(result);
            var model = Assert.IsAssignableFrom<HtmlHeadViewModel>(viewResult.ViewData.Model);

            model.CanonicalUrl.Should().BeNull();

            controller.Dispose();
        }

        [Theory]
        [MemberData(nameof(JsonMediaTypes))]
        public async Task PagesControllerHtmlHeadJsonReturnsSuccessWhenNoData(string mediaTypeName)
        {
            // Arrange
            const string article = "an-article-name";
            ContentPageModel? expectedResult = null;
            var controller = BuildPagesController(mediaTypeName);

            A.CallTo(() => FakeContentPageService.GetByNameAsync(A<string>.Ignored)).Returns(expectedResult);

            // Act
            var result = await controller.HtmlHead(article).ConfigureAwait(false);

            // Assert
            A.CallTo(() => FakeContentPageService.GetByNameAsync(A<string>.Ignored)).MustHaveHappenedOnceExactly();

            var jsonResult = Assert.IsType<OkObjectResult>(result);
            var model = Assert.IsAssignableFrom<HtmlHeadViewModel>(jsonResult.Value);

            model.CanonicalUrl.Should().BeNull();

            controller.Dispose();
        }

        [Theory]
        [MemberData(nameof(InvalidMediaTypes))]
        public async Task PagesControllerHtmlHeadReturnsNotAcceptable(string mediaTypeName)
        {
            // Arrange
            const string article = "an-article-name";
            var expectedResult = A.Fake<ContentPageModel>();
            var controller = BuildPagesController(mediaTypeName);

            A.CallTo(() => FakeContentPageService.GetByNameAsync(A<string>.Ignored)).Returns(expectedResult);
            A.CallTo(() => FakeMapper.Map(A<ContentPageModel>.Ignored, A<HtmlHeadViewModel>.Ignored)).Returns(A.Fake<HtmlHeadViewModel>());

            // Act
            var result = await controller.HtmlHead(article).ConfigureAwait(false);

            // Assert
            A.CallTo(() => FakeContentPageService.GetByNameAsync(A<string>.Ignored)).MustHaveHappenedOnceExactly();
            A.CallTo(() => FakeMapper.Map(A<ContentPageModel>.Ignored, A<HtmlHeadViewModel>.Ignored)).MustHaveHappenedOnceExactly();

            var statusResult = Assert.IsType<StatusCodeResult>(result);

            A.Equals((int)HttpStatusCode.NotAcceptable, statusResult.StatusCode);

            controller.Dispose();
        }
    }
}

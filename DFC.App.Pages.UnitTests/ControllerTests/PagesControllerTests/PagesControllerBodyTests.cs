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
    public class PagesControllerBodyTests : BasePagesControllerTests
    {
        [Theory]
        [MemberData(nameof(HtmlMediaTypes))]
        public async Task PagesControllerBodyHtmlReturnsSuccess(string mediaTypeName)
        {
            // Arrange
            const string article = "an-article-name";
            var expectedResult = new ContentPageModel() { Content = "<h1>A document</h1>" };
            var controller = BuildPagesController(mediaTypeName);

            expectedResult.CanonicalName = article;

            A.CallTo(() => FakeContentPageService.GetByNameAsync(A<string>.Ignored)).Returns(expectedResult);
            A.CallTo(() => FakeMapper.Map(A<ContentPageModel>.Ignored, A<BodyViewModel>.Ignored)).Returns(A.Fake<BodyViewModel>());

            // Act
            var result = await controller.Body(article).ConfigureAwait(false);

            // Assert
            A.CallTo(() => FakeContentPageService.GetByNameAsync(A<string>.Ignored)).MustHaveHappenedOnceExactly();
            A.CallTo(() => FakeMapper.Map(A<ContentPageModel>.Ignored, A<BodyViewModel>.Ignored)).MustHaveHappenedOnceExactly();

            var viewResult = Assert.IsType<ViewResult>(result);
            _ = Assert.IsAssignableFrom<BodyViewModel>(viewResult.ViewData.Model);

            controller.Dispose();
        }

        [Theory]
        [MemberData(nameof(JsonMediaTypes))]
        public async Task PagesControllerBodyJsonReturnsSuccess(string mediaTypeName)
        {
            // Arrange
            const string article = "an-article-name";
            var expectedResult = new ContentPageModel() { Content = "<h1>A document</h1>" };
            var controller = BuildPagesController(mediaTypeName);

            expectedResult.CanonicalName = article;

            A.CallTo(() => FakeContentPageService.GetByNameAsync(A<string>.Ignored)).Returns(expectedResult);
            A.CallTo(() => FakeMapper.Map(A<ContentPageModel>.Ignored, A<BodyViewModel>.Ignored)).Returns(A.Fake<BodyViewModel>());

            // Act
            var result = await controller.Body(article).ConfigureAwait(false);

            // Assert
            A.CallTo(() => FakeContentPageService.GetByNameAsync(A<string>.Ignored)).MustHaveHappenedOnceExactly();
            A.CallTo(() => FakeMapper.Map(A<ContentPageModel>.Ignored, A<BodyViewModel>.Ignored)).MustHaveHappenedOnceExactly();

            var jsonResult = Assert.IsType<OkObjectResult>(result);
            _ = Assert.IsAssignableFrom<ContentPageModel>(jsonResult.Value);

            controller.Dispose();
        }

        [Theory]
        [MemberData(nameof(HtmlMediaTypes))]
        public async Task PagesControllerBodyWithNullArticleHtmlReturnsSuccess(string mediaTypeName)
        {
            // Arrange
            const string? article = null;
            var expectedResult = new ContentPageModel() { Content = "<h1>A document</h1>" };
            var controller = BuildPagesController(mediaTypeName);

            expectedResult.CanonicalName = article;

            A.CallTo(() => FakeContentPageService.GetByNameAsync(A<string>.Ignored)).Returns(expectedResult);
            A.CallTo(() => FakeMapper.Map(A<ContentPageModel>.Ignored, A<BodyViewModel>.Ignored)).Returns(A.Fake<BodyViewModel>());

            // Act
            var result = await controller.Body(article).ConfigureAwait(false);

            // Assert
            A.CallTo(() => FakeContentPageService.GetByNameAsync(A<string>.Ignored)).MustHaveHappenedOnceExactly();
            A.CallTo(() => FakeMapper.Map(A<ContentPageModel>.Ignored, A<BodyViewModel>.Ignored)).MustHaveHappenedOnceExactly();

            var viewResult = Assert.IsType<ViewResult>(result);
            _ = Assert.IsAssignableFrom<BodyViewModel>(viewResult.ViewData.Model);

            controller.Dispose();
        }

        [Theory]
        [MemberData(nameof(JsonMediaTypes))]
        public async Task PagesControllerBodyWithNullArticleJsonReturnsSuccess(string mediaTypeName)
        {
            // Arrange
            const string? article = null;
            var expectedResult = new ContentPageModel() { Content = "<h1>A document</h1>" };
            var controller = BuildPagesController(mediaTypeName);

            expectedResult.CanonicalName = article;

            A.CallTo(() => FakeContentPageService.GetByNameAsync(A<string>.Ignored)).Returns(expectedResult);
            A.CallTo(() => FakeMapper.Map(A<ContentPageModel>.Ignored, A<BodyViewModel>.Ignored)).Returns(A.Fake<BodyViewModel>());

            // Act
            var result = await controller.Body(article).ConfigureAwait(false);

            // Assert
            A.CallTo(() => FakeContentPageService.GetByNameAsync(A<string>.Ignored)).MustHaveHappenedOnceExactly();
            A.CallTo(() => FakeMapper.Map(A<ContentPageModel>.Ignored, A<BodyViewModel>.Ignored)).MustHaveHappenedOnceExactly();

            var jsonResult = Assert.IsType<OkObjectResult>(result);
            _ = Assert.IsAssignableFrom<ContentPageModel>(jsonResult.Value);

            controller.Dispose();
        }

        [Theory]
        [MemberData(nameof(HtmlMediaTypes))]
        [MemberData(nameof(JsonMediaTypes))]
        public async Task PagesControllerBodyReturnsRedirectWhenAlternateArticleExists(string mediaTypeName)
        {
            // Arrange
            const string article = "an-article-name";
            ContentPageModel? expectedResult = null;
            var expectedAlternativeResult = A.Fake<ContentPageModel>();
            var controller = BuildPagesController(mediaTypeName);

            expectedAlternativeResult.CanonicalName = article;

            A.CallTo(() => FakeContentPageService.GetByNameAsync(A<string>.Ignored)).Returns(expectedResult);
            A.CallTo(() => FakeContentPageService.GetByAlternativeNameAsync(A<string>.Ignored)).Returns(expectedAlternativeResult);

            // Act
            var result = await controller.Body(article).ConfigureAwait(false);

            // Assert
            A.CallTo(() => FakeContentPageService.GetByNameAsync(A<string>.Ignored)).MustHaveHappenedOnceExactly();
            A.CallTo(() => FakeContentPageService.GetByAlternativeNameAsync(A<string>.Ignored)).MustHaveHappenedOnceExactly();

            var statusResult = Assert.IsType<RedirectResult>(result);

            statusResult.Url.Should().NotBeNullOrWhiteSpace();
            A.Equals(true, statusResult.Permanent);

            controller.Dispose();
        }

        [Theory]
        [MemberData(nameof(JsonMediaTypes))]
        [MemberData(nameof(HtmlMediaTypes))]
        public async Task PagesControllerBodyReturnsNotFoundWhenNoAlternateArticle(string mediaTypeName)
        {
            // Arrange
            const string article = "an-article-name";
            ContentPageModel? expectedResult = null;
            ContentPageModel? expectedAlternativeResult = null;
            var controller = BuildPagesController(mediaTypeName);

            A.CallTo(() => FakeContentPageService.GetByNameAsync(A<string>.Ignored)).Returns(expectedResult);
            A.CallTo(() => FakeContentPageService.GetByAlternativeNameAsync(A<string>.Ignored)).Returns(expectedAlternativeResult);

            // Act
            var result = await controller.Body(article).ConfigureAwait(false);

            // Assert
            A.CallTo(() => FakeContentPageService.GetByNameAsync(A<string>.Ignored)).MustHaveHappenedOnceExactly();
            A.CallTo(() => FakeContentPageService.GetByAlternativeNameAsync(A<string>.Ignored)).MustHaveHappenedOnceExactly();

            var statusResult = Assert.IsType<NotFoundResult>(result);

            A.Equals((int)HttpStatusCode.NotFound, statusResult.StatusCode);

            controller.Dispose();
        }

        [Theory]
        [MemberData(nameof(InvalidMediaTypes))]
        public async Task PagesControllerBodyReturnsNotAcceptable(string mediaTypeName)
        {
            // Arrange
            const string article = "an-article-name";
            var expectedResult = new ContentPageModel() { Content = "<h1>A document</h1>" };
            var controller = BuildPagesController(mediaTypeName);

            expectedResult.CanonicalName = article;

            A.CallTo(() => FakeContentPageService.GetByNameAsync(A<string>.Ignored)).Returns(expectedResult);
            A.CallTo(() => FakeMapper.Map(A<ContentPageModel>.Ignored, A<BodyViewModel>.Ignored)).Returns(A.Fake<BodyViewModel>());

            // Act
            var result = await controller.Body(article).ConfigureAwait(false);

            // Assert
            A.CallTo(() => FakeContentPageService.GetByNameAsync(A<string>.Ignored)).MustHaveHappenedOnceExactly();
            A.CallTo(() => FakeMapper.Map(A<ContentPageModel>.Ignored, A<BodyViewModel>.Ignored)).MustHaveHappenedOnceExactly();

            var statusResult = Assert.IsType<StatusCodeResult>(result);

            A.Equals((int)HttpStatusCode.NotAcceptable, statusResult.StatusCode);

            controller.Dispose();
        }
    }
}

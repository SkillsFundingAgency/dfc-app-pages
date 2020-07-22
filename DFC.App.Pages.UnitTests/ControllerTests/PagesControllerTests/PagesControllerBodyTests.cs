using DFC.App.Pages.Data.Models;
using DFC.App.Pages.ViewModels;
using FakeItEasy;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
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
            const string location = "a-location-name";
            const string article = "an-article-name";
            var expectedResults = new List<ContentPageModel> { new ContentPageModel() { Content = "<h1>A document</h1>" } };
            var controller = BuildPagesController(mediaTypeName);

            expectedResults.First().CanonicalName = article;

            A.CallTo(() => FakeContentPageService.GetAsync(A<Expression<Func<ContentPageModel, bool>>>.Ignored)).Returns(expectedResults);
            A.CallTo(() => FakeMapper.Map(A<ContentPageModel>.Ignored, A<BodyViewModel>.Ignored)).Returns(A.Fake<BodyViewModel>());

            // Act
            var result = await controller.Body(location, article).ConfigureAwait(false);

            // Assert
            A.CallTo(() => FakeContentPageService.GetAsync(A<Expression<Func<ContentPageModel, bool>>>.Ignored)).MustHaveHappenedOnceExactly();
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
            const string location = "a-location-name";
            const string article = "an-article-name";
            var expectedResults = new List<ContentPageModel> { new ContentPageModel() { Content = "<h1>A document</h1>" } };
            var controller = BuildPagesController(mediaTypeName);

            expectedResults.First().CanonicalName = article;

            A.CallTo(() => FakeContentPageService.GetAsync(A<Expression<Func<ContentPageModel, bool>>>.Ignored)).Returns(expectedResults);
            A.CallTo(() => FakeMapper.Map(A<ContentPageModel>.Ignored, A<BodyViewModel>.Ignored)).Returns(A.Fake<BodyViewModel>());

            // Act
            var result = await controller.Body(location, article).ConfigureAwait(false);

            // Assert
            A.CallTo(() => FakeContentPageService.GetAsync(A<Expression<Func<ContentPageModel, bool>>>.Ignored)).MustHaveHappenedOnceExactly();
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
            const string location = "a-location-name";
            const string? article = null;
            var expectedResults = new List<ContentPageModel> { new ContentPageModel() { Content = "<h1>A document</h1>" } };
            var controller = BuildPagesController(mediaTypeName);

            expectedResults.First().CanonicalName = article;

            A.CallTo(() => FakeContentPageService.GetAsync(A<Expression<Func<ContentPageModel, bool>>>.Ignored)).Returns(expectedResults);
            A.CallTo(() => FakeMapper.Map(A<ContentPageModel>.Ignored, A<BodyViewModel>.Ignored)).Returns(A.Fake<BodyViewModel>());

            // Act
            var result = await controller.Body(location, article).ConfigureAwait(false);

            // Assert
            A.CallTo(() => FakeContentPageService.GetAsync(A<Expression<Func<ContentPageModel, bool>>>.Ignored)).MustHaveHappenedOnceExactly();
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
            const string location = "a-location-name";
            const string? article = null;
            var expectedResults = new List<ContentPageModel> { new ContentPageModel() { Content = "<h1>A document</h1>" } };
            var controller = BuildPagesController(mediaTypeName);

            expectedResults.First().CanonicalName = article;

            A.CallTo(() => FakeContentPageService.GetAsync(A<Expression<Func<ContentPageModel, bool>>>.Ignored)).Returns(expectedResults);
            A.CallTo(() => FakeMapper.Map(A<ContentPageModel>.Ignored, A<BodyViewModel>.Ignored)).Returns(A.Fake<BodyViewModel>());

            // Act
            var result = await controller.Body(location, article).ConfigureAwait(false);

            // Assert
            A.CallTo(() => FakeContentPageService.GetAsync(A<Expression<Func<ContentPageModel, bool>>>.Ignored)).MustHaveHappenedOnceExactly();
            A.CallTo(() => FakeMapper.Map(A<ContentPageModel>.Ignored, A<BodyViewModel>.Ignored)).MustHaveHappenedOnceExactly();

            var jsonResult = Assert.IsType<OkObjectResult>(result);
            _ = Assert.IsAssignableFrom<ContentPageModel>(jsonResult.Value);

            controller.Dispose();
        }

        [Theory]
        [MemberData(nameof(HtmlMediaTypes))]
        [MemberData(nameof(JsonMediaTypes))]
        public async Task PagesControllerBodyReturnsRedirectWhenRedirectLocationExists(string mediaTypeName)
        {
            // Arrange
            const string location = "a-location-name";
            const string article = "an-article-name";
            List<ContentPageModel>? expectedResults = null;
            var expectedRedirectResult = A.Fake<ContentPageModel>();
            var controller = BuildPagesController(mediaTypeName);

            expectedRedirectResult.CanonicalName = article;

            A.CallTo(() => FakeContentPageService.GetAsync(A<Expression<Func<ContentPageModel, bool>>>.Ignored)).Returns(expectedResults);
            A.CallTo(() => FakeContentPageService.GetByRedirectLocationAsync(A<string>.Ignored)).Returns(expectedRedirectResult);

            // Act
            var result = await controller.Body(location, article).ConfigureAwait(false);

            // Assert
            A.CallTo(() => FakeContentPageService.GetAsync(A<Expression<Func<ContentPageModel, bool>>>.Ignored)).MustHaveHappenedOnceExactly();
            A.CallTo(() => FakeContentPageService.GetByRedirectLocationAsync(A<string>.Ignored)).MustHaveHappenedOnceExactly();

            var statusResult = Assert.IsType<RedirectResult>(result);

            statusResult.Url.Should().NotBeNullOrWhiteSpace();
            A.Equals(true, statusResult.Permanent);

            controller.Dispose();
        }

        [Theory]
        [MemberData(nameof(JsonMediaTypes))]
        [MemberData(nameof(HtmlMediaTypes))]
        public async Task PagesControllerBodyReturnsNotFoundWhenNoRedirectLocation(string mediaTypeName)
        {
            // Arrange
            const string location = "a-location-name";
            const string article = "an-article-name";
            List<ContentPageModel>? expectedResults = null;
            ContentPageModel? expectedRedirectResult = null;
            var controller = BuildPagesController(mediaTypeName);

            A.CallTo(() => FakeContentPageService.GetAsync(A<Expression<Func<ContentPageModel, bool>>>.Ignored)).Returns(expectedResults);
            A.CallTo(() => FakeContentPageService.GetByRedirectLocationAsync(A<string>.Ignored)).Returns(expectedRedirectResult);

            // Act
            var result = await controller.Body(location, article).ConfigureAwait(false);

            // Assert
            A.CallTo(() => FakeContentPageService.GetAsync(A<Expression<Func<ContentPageModel, bool>>>.Ignored)).MustHaveHappenedOnceExactly();
            A.CallTo(() => FakeContentPageService.GetByRedirectLocationAsync(A<string>.Ignored)).MustHaveHappenedOnceExactly();

            var statusResult = Assert.IsType<NotFoundResult>(result);

            A.Equals((int)HttpStatusCode.NotFound, statusResult.StatusCode);

            controller.Dispose();
        }

        [Theory]
        [MemberData(nameof(InvalidMediaTypes))]
        public async Task PagesControllerBodyReturnsNotAcceptable(string mediaTypeName)
        {
            // Arrange
            const string location = "a-location-name";
            const string article = "an-article-name";
            var expectedResults = new List<ContentPageModel> { new ContentPageModel() { Content = "<h1>A document</h1>" } };
            var controller = BuildPagesController(mediaTypeName);

            expectedResults.First().CanonicalName = article;

            A.CallTo(() => FakeContentPageService.GetAsync(A<Expression<Func<ContentPageModel, bool>>>.Ignored)).Returns(expectedResults);
            A.CallTo(() => FakeMapper.Map(A<ContentPageModel>.Ignored, A<BodyViewModel>.Ignored)).Returns(A.Fake<BodyViewModel>());

            // Act
            var result = await controller.Body(location, article).ConfigureAwait(false);

            // Assert
            A.CallTo(() => FakeContentPageService.GetAsync(A<Expression<Func<ContentPageModel, bool>>>.Ignored)).MustHaveHappenedOnceExactly();
            A.CallTo(() => FakeMapper.Map(A<ContentPageModel>.Ignored, A<BodyViewModel>.Ignored)).MustHaveHappenedOnceExactly();

            var statusResult = Assert.IsType<StatusCodeResult>(result);

            A.Equals((int)HttpStatusCode.NotAcceptable, statusResult.StatusCode);

            controller.Dispose();
        }
    }
}

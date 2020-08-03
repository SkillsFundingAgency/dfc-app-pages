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
    public class PagesControllerBreadcrumbTests : BasePagesControllerTests
    {
        [Theory]
        [MemberData(nameof(HtmlMediaTypes))]
        public async Task PagesControllerBreadcrumbHtmlReturnsSuccess(string mediaTypeName)
        {
            // Arrange
            const string location = "a-location-name";
            const string article = "an-article-name";
            var expectedResults = A.CollectionOfFake<ContentPageModel>(1);
            var expectedBreadcrumb = new BreadcrumbViewModel { Breadcrumbs = new List<BreadcrumbItemViewModel> { new BreadcrumbItemViewModel { Route = "a-route", Title = "A title", }, }, };
            var controller = BuildPagesController(mediaTypeName);

            expectedResults.First().CanonicalName = article;

            A.CallTo(() => FakeContentPageService.GetAsync(A<Expression<Func<ContentPageModel, bool>>>.Ignored)).Returns(expectedResults);
            A.CallTo(() => FakeMapper.Map<BreadcrumbViewModel>(A<ContentPageModel>.Ignored)).Returns(expectedBreadcrumb);

            // Act
            var result = await controller.Breadcrumb(location, article).ConfigureAwait(false);

            // Assert
            A.CallTo(() => FakeContentPageService.GetAsync(A<Expression<Func<ContentPageModel, bool>>>.Ignored)).MustHaveHappenedOnceExactly();
            A.CallTo(() => FakeMapper.Map<BreadcrumbViewModel>(A<ContentPageModel>.Ignored)).MustHaveHappenedOnceExactly();

            var viewResult = Assert.IsType<ViewResult>(result);
            var model = Assert.IsAssignableFrom<BreadcrumbViewModel>(viewResult.ViewData.Model);

            model?.Breadcrumbs!.Count.Should().BeGreaterThan(0);

            controller.Dispose();
        }

        [Theory]
        [MemberData(nameof(JsonMediaTypes))]
        public async Task PagesControllerBreadcrumbJsonReturnsSuccess(string mediaTypeName)
        {
            // Arrange
            const string location = "a-location-name";
            const string article = "an-article-name";
            var expectedResults = A.CollectionOfFake<ContentPageModel>(1);
            var expectedBreadcrumb = new BreadcrumbViewModel { Breadcrumbs = new List<BreadcrumbItemViewModel> { new BreadcrumbItemViewModel { Route = "a-route", Title = "A title", }, }, };
            var controller = BuildPagesController(mediaTypeName);

            expectedResults.First().CanonicalName = article;
            expectedResults.First().MetaTags.Title = article.ToUpperInvariant();

            A.CallTo(() => FakeContentPageService.GetAsync(A<Expression<Func<ContentPageModel, bool>>>.Ignored)).Returns(expectedResults);
            A.CallTo(() => FakeMapper.Map<BreadcrumbViewModel>(A<ContentPageModel>.Ignored)).Returns(expectedBreadcrumb);

            // Act
            var result = await controller.Breadcrumb(location, article).ConfigureAwait(false);

            // Assert
            A.CallTo(() => FakeContentPageService.GetAsync(A<Expression<Func<ContentPageModel, bool>>>.Ignored)).MustHaveHappenedOnceExactly();
            A.CallTo(() => FakeMapper.Map<BreadcrumbViewModel>(A<ContentPageModel>.Ignored)).MustHaveHappenedOnceExactly();

            var jsonResult = Assert.IsType<OkObjectResult>(result);
            var model = Assert.IsAssignableFrom<BreadcrumbViewModel>(jsonResult.Value);

            model?.Breadcrumbs!.Count.Should().BeGreaterThan(0);

            controller.Dispose();
        }

        [Theory]
        [MemberData(nameof(HtmlMediaTypes))]
        public async Task PagesControllerBreadcrumbHtmlReturnsSuccessWhenNoData(string mediaTypeName)
        {
            // Arrange
            const string location = "a-location-name";
            const string article = "an-article-name";
            List<ContentPageModel>? expectedResults = null;
            var controller = BuildPagesController(mediaTypeName);

            A.CallTo(() => FakeContentPageService.GetAsync(A<Expression<Func<ContentPageModel, bool>>>.Ignored)).Returns(expectedResults);

            // Act
            var result = await controller.Breadcrumb(location, article).ConfigureAwait(false);

            // Assert
            A.CallTo(() => FakeContentPageService.GetAsync(A<Expression<Func<ContentPageModel, bool>>>.Ignored)).MustHaveHappenedOnceExactly();

            var viewResult = Assert.IsType<ViewResult>(result);
            var model = Assert.IsAssignableFrom<BreadcrumbViewModel>(viewResult.ViewData.Model);

            Assert.Null(model?.Breadcrumbs);

            controller.Dispose();
        }

        [Theory]
        [MemberData(nameof(JsonMediaTypes))]
        public async Task PagesControllerBreadcrumbJsonReturnsSuccessWhenNoData(string mediaTypeName)
        {
            // Arrange
            const string location = "a-location-name";
            const string article = "an-article-name";
            List<ContentPageModel>? expectedResults = null;
            var controller = BuildPagesController(mediaTypeName);

            A.CallTo(() => FakeContentPageService.GetAsync(A<Expression<Func<ContentPageModel, bool>>>.Ignored)).Returns(expectedResults);

            // Act
            var result = await controller.Breadcrumb(location, article).ConfigureAwait(false);

            // Assert
            A.CallTo(() => FakeContentPageService.GetAsync(A<Expression<Func<ContentPageModel, bool>>>.Ignored)).MustHaveHappenedOnceExactly();

            var jsonResult = Assert.IsType<OkObjectResult>(result);
            var model = Assert.IsAssignableFrom<BreadcrumbViewModel>(jsonResult.Value);

            Assert.Null(model?.Breadcrumbs);

            controller.Dispose();
        }

        [Theory]
        [MemberData(nameof(InvalidMediaTypes))]
        public async Task PagesControllerBreadcrumbReturnsNotAcceptable(string mediaTypeName)
        {
            // Arrange
            const string location = "a-location-name";
            const string article = "an-article-name";
            var expectedResults = A.CollectionOfFake<ContentPageModel>(1);
            var controller = BuildPagesController(mediaTypeName);

            expectedResults.First().CanonicalName = article;

            A.CallTo(() => FakeContentPageService.GetAsync(A<Expression<Func<ContentPageModel, bool>>>.Ignored)).Returns(expectedResults);

            // Act
            var result = await controller.Breadcrumb(location, article).ConfigureAwait(false);

            // Assert
            A.CallTo(() => FakeContentPageService.GetAsync(A<Expression<Func<ContentPageModel, bool>>>.Ignored)).MustHaveHappenedOnceExactly();

            var statusResult = Assert.IsType<StatusCodeResult>(result);

            A.Equals((int)HttpStatusCode.NotAcceptable, statusResult.StatusCode);

            controller.Dispose();
        }
    }
}

using DFC.App.Pages.Data.Models;
using DFC.App.Pages.Models;
using DFC.App.Pages.ViewModels;
using DFC.Common.SharedContent.Pkg.Netcore.Model.ContentItems;
using FakeItEasy;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Net;
using System.Net.Mime;
using System.Threading.Tasks;
using Xunit;

namespace DFC.App.Pages.UnitTests.ControllerTests.PagesControllerTests
{
    [Trait("Category", "Pages Controller Unit Tests")]
    public class PagesControllerBreadcrumbTests : BasePagesControllerTests
    {
        [Theory]
        [MemberData(nameof(HtmlMediaTypes))]
        public async Task PagesControllerBreadcrumbHtmlReturnsNoContentResult(string mediaTypeName)
        {
            // Arrange
            var pageRequestModel = new PageRequestModel
            {
                Location1 = "a-location-name",
                Location2 = "an-article-name",
            };
            var expectedBreadcrumb = new BreadcrumbViewModel { Breadcrumbs = new List<BreadcrumbItemViewModel> { new BreadcrumbItemViewModel { Route = "a-route", Title = "A title", }, }, };
            var controller = BuildPagesController(mediaTypeName);

            A.CallTo(() => FakeMapper.Map<BreadcrumbViewModel>(A<Page>.Ignored)).Returns(expectedBreadcrumb);

            // Act
            var result = await controller.Breadcrumb(pageRequestModel).ConfigureAwait(false);

            var statusResult = Assert.IsType<NoContentResult>(result);

            A.Equals((int)HttpStatusCode.NoContent, statusResult.StatusCode);

            controller.Dispose();
        }

        [Theory]
        [MemberData(nameof(JsonMediaTypes))]
        public async Task PagesControllerBreadcrumbJsonReturnsNoContent(string mediaTypeName)
        {
            // Arrange
            var pageRequestModel = new PageRequestModel
            {
                Location1 = "a-location-name",
                Location2 = "an-article-name",
            };
            var expectedBreadcrumb = new BreadcrumbViewModel { Breadcrumbs = new List<BreadcrumbItemViewModel> { new BreadcrumbItemViewModel { Route = "a-route", Title = "A title", }, }, };
            var controller = BuildPagesController(mediaTypeName);

            A.CallTo(() => FakeMapper.Map<BreadcrumbViewModel>(A<Page>.Ignored)).Returns(expectedBreadcrumb);

            // Act
            var result = await controller.Breadcrumb(pageRequestModel).ConfigureAwait(false);

            var statusResult = Assert.IsType<NoContentResult>(result);

            A.Equals((int)HttpStatusCode.NoContent, statusResult.StatusCode);

            controller.Dispose();
        }

        [Theory]
        [MemberData(nameof(HtmlMediaTypes))]
        public async Task PagesControllerBreadcrumbHtmlReturnsSuccessWhenNoData(string mediaTypeName)
        {
            // Arrange
            var pageRequestModel = new PageRequestModel
            {
                Location1 = "a-location-name",
                Location2 = "an-article-name",
            };
            var controller = BuildPagesController(mediaTypeName);

            // Act
            var result = await controller.Breadcrumb(pageRequestModel).ConfigureAwait(false);

            var statusResult = Assert.IsType<NoContentResult>(result);

            A.Equals((int)HttpStatusCode.NoContent, statusResult.StatusCode);

            controller.Dispose();
        }

        [Theory]
        [MemberData(nameof(JsonMediaTypes))]
        public async Task PagesControllerBreadcrumbJsonReturnsSuccessWhenNoData(string mediaTypeName)
        {
            // Arrange
            var pageRequestModel = new PageRequestModel
            {
                Location1 = "a-location-name",
                Location2 = "an-article-name",
            };
            var controller = BuildPagesController(mediaTypeName);

            // Act
            var result = await controller.Breadcrumb(pageRequestModel).ConfigureAwait(false);

            var statusResult = Assert.IsType<NoContentResult>(result);

            A.Equals((int)HttpStatusCode.NoContent, statusResult.StatusCode);

            controller.Dispose();
        }

        [Theory]
        [MemberData(nameof(InvalidMediaTypes))]
        public async Task PagesControllerBreadcrumbReturnsNoContent(string mediaTypeName)
        {
            // Arrange
            var pageRequestModel = new PageRequestModel
            {
                Location1 = "a-location-name",
                Location2 = "an-article-name",
            };
            var controller = BuildPagesController(mediaTypeName);

            // Act
            var result = await controller.Breadcrumb(pageRequestModel).ConfigureAwait(false);

            var statusResult = Assert.IsType<NoContentResult>(result);

            A.Equals((int)HttpStatusCode.NoContent, statusResult.StatusCode);

            controller.Dispose();
        }

        [Fact]
        public async Task PagesControllerBreadcrumbHtmlReturnsNoContentForDoNotShowBreadcrumb()
        {
            // Arrange
            var pageRequestModel = new PageRequestModel
            {
                Location1 = "a-location-name",
                Location2 = "an-article-name",
            };
            var controller = BuildPagesController(MediaTypeNames.Text.Html);

            // Act
            var result = await controller.Breadcrumb(pageRequestModel).ConfigureAwait(false);

            var statusResult = Assert.IsType<NoContentResult>(result);

            A.Equals((int)HttpStatusCode.NoContent, statusResult.StatusCode);

            controller.Dispose();
        }
    }
}
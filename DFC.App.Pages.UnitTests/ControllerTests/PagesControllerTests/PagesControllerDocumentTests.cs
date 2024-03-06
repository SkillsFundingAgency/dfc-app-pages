using DFC.App.Pages.Data.Models;
using DFC.App.Pages.Models;
using DFC.App.Pages.ViewModels;
using DFC.Common.SharedContent.Pkg.Netcore.Model.ContentItems;
using FakeItEasy;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Mime;
using System.Threading.Tasks;
using Xunit;

namespace DFC.App.Pages.UnitTests.ControllerTests.PagesControllerTests
{
    [Trait("Category", "Pages Controller Unit Tests")]
    public class PagesControllerDocumentTests : BasePagesControllerTests
    {
        [Theory]
        [MemberData(nameof(HtmlMediaTypes))]
        public async Task PagesControllerDocumentHtmlReturnsSuccess(string mediaTypeName)
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
            var expectedModel = new DocumentViewModel
            {
                DocumentId = Guid.NewGuid(),
                CanonicalName = "a-canonical-name",
                PartitionKey = "partition-key",
                Version = Guid.NewGuid(),
                IncludeInSitemap = true,
                Url = new Uri("https://somewhere.com", UriKind.Absolute),
                Content = new Microsoft.AspNetCore.Html.HtmlString("some content"),
                LastReviewed = DateTime.Now,
            };
            var expectedBreadcrumbModel = new BreadcrumbViewModel
            {
                Breadcrumbs = new List<BreadcrumbItemViewModel>
                {
                    new BreadcrumbItemViewModel
                    {
                        Route = "/",
                        Title = "Root",
                    },
                    new BreadcrumbItemViewModel
                    {
                        Route = "/a-route",
                        Title = "A title",
                    },
                },
            };

            A.CallTo(() => FakeSharedContentRedisInterface.GetDataAsync<Page>("PageTest", "PUBLISHED")).Returns(expected);
            A.CallTo(() => FakeMapper.Map<DocumentViewModel>(A<ContentPageModel>.Ignored)).Returns(expectedModel);
            A.CallTo(() => FakeMapper.Map<BreadcrumbViewModel>(A<ContentPageModel>.Ignored)).Returns(expectedBreadcrumbModel);

            // Act
            var result = await controller.Document(pageRequestModel).ConfigureAwait(false);

            var viewResult = Assert.IsType<ViewResult>(result);
            _ = Assert.IsAssignableFrom<DocumentViewModel>(viewResult.ViewData.Model);
            controller.Dispose();
        }

        [Theory]
        [MemberData(nameof(JsonMediaTypes))]
        public async Task PagesControllerDocumentJsonReturnsSuccess(string mediaTypeName)
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

            var expectedBreadcrumbModel = new BreadcrumbViewModel
            {
                Breadcrumbs = new List<BreadcrumbItemViewModel>
                {
                    new BreadcrumbItemViewModel
                    {
                        Route = "/",
                        Title = "Root",
                    },
                    new BreadcrumbItemViewModel
                    {
                        Route = "/a-route",
                        Title = "A title",
                    },
                },
            };
            var controller = BuildPagesController(mediaTypeName);

            A.CallTo(() => FakeSharedContentRedisInterface.GetDataAsync<Page>("PageTest", "PUBLISHED")).Returns(expected);
            A.CallTo(() => FakeMapper.Map<DocumentViewModel>(A<ContentPageModel>.Ignored)).Returns(A.Fake<DocumentViewModel>());
            A.CallTo(() => FakeMapper.Map<BreadcrumbViewModel?>(A<ContentPageModel>.Ignored)).Returns(expectedBreadcrumbModel);

            // Act
            var result = await controller.Document(pageRequestModel).ConfigureAwait(false);

            var jsonResult = Assert.IsType<OkObjectResult>(result);
            _ = Assert.IsAssignableFrom<DocumentViewModel>(jsonResult.Value);

            controller.Dispose();
        }

        [Theory]
        [MemberData(nameof(HtmlMediaTypes))]
        public async Task PagesControllerDocumentReturnsViewResult(string mediaTypeName)
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
            var result = await controller.Document(pageRequestModel).ConfigureAwait(false);

            var viewResult = Assert.IsType<ViewResult>(result);
            _ = Assert.IsAssignableFrom<DocumentViewModel>(viewResult.ViewData.Model);

            controller.Dispose();
        }

        [Theory]
        [MemberData(nameof(JsonMediaTypes))]
        public async Task PagesControllerDocumentJsonReturnsOkResult(string mediaTypeName)
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
            var result = await controller.Document(pageRequestModel).ConfigureAwait(false);

            var jsonResult = Assert.IsType<OkObjectResult>(result);
            _ = Assert.IsAssignableFrom<DocumentViewModel>(jsonResult.Value);

            controller.Dispose();
        }

        [Theory]
        [MemberData(nameof(InvalidMediaTypes))]
        public async Task PagesControllerDocumentReturnsNotAcceptable(string mediaTypeName)
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

            var expectedBreadcrumbModel = new BreadcrumbViewModel
            {
                Breadcrumbs = new List<BreadcrumbItemViewModel>
                {
                    new BreadcrumbItemViewModel
                    {
                        Route = "/",
                        Title = "Root",
                    },
                    new BreadcrumbItemViewModel
                    {
                        Route = "/a-route",
                        Title = "A title",
                    },
                },
            };
            var controller = BuildPagesController(mediaTypeName);

            A.CallTo(() => FakeSharedContentRedisInterface.GetDataAsync<Page>("PageTest", "PUBLISHED")).Returns(expected);
            A.CallTo(() => FakeMapper.Map<DocumentViewModel>(A<ContentPageModel>.Ignored)).Returns(A.Fake<DocumentViewModel>());
            A.CallTo(() => FakeMapper.Map<BreadcrumbViewModel?>(A<ContentPageModel>.Ignored)).Returns(expectedBreadcrumbModel);

            // Act
            var result = await controller.Document(pageRequestModel).ConfigureAwait(false);

            var statusResult = Assert.IsType<StatusCodeResult>(result);

            A.Equals((int)HttpStatusCode.NotAcceptable, statusResult.StatusCode);

            controller.Dispose();
        }

        [Fact]
        public async Task PagesControllerDocumentHtmlReturnsSuccessForDoNotShowBreadcrumb()
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
            var controller = BuildPagesController(MediaTypeNames.Text.Html);
            var expectedModel = new DocumentViewModel
            {
                DocumentId = Guid.NewGuid(),
                CanonicalName = "a-canonical-name",
                PartitionKey = "partition-key",
                Version = Guid.NewGuid(),
                IncludeInSitemap = true,
                Url = new Uri("https://somewhere.com", UriKind.Absolute),
                Content = new Microsoft.AspNetCore.Html.HtmlString("some content"),
                LastReviewed = DateTime.Now,
            };

            A.CallTo(() => FakeSharedContentRedisInterface.GetDataAsync<Page>("PageTest", "PUBLISHED")).Returns(expected);
            A.CallTo(() => FakeMapper.Map<DocumentViewModel>(A<ContentPageModel>.Ignored)).Returns(expectedModel);

            // Act
            var result = await controller.Document(pageRequestModel).ConfigureAwait(false);

            var viewResult = Assert.IsType<ViewResult>(result);
            _ = Assert.IsAssignableFrom<DocumentViewModel>(viewResult.ViewData.Model);

            controller.Dispose();
        }
    }
}
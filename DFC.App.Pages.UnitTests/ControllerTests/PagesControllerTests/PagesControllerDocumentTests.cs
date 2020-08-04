using DFC.App.Pages.Data.Models;
using DFC.App.Pages.Models;
using DFC.App.Pages.ViewModels;
using FakeItEasy;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Net;
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
            var expectedResult = new ContentPageModel()
            {
                PageLocation = "/" + pageRequestModel.Location1,
                CanonicalName = pageRequestModel.Location2,
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

            A.CallTo(() => FakePagesControlerHelpers.GetContentPageAsync(A<string>.Ignored, A<string>.Ignored)).Returns(expectedResult);
            A.CallTo(() => FakeMapper.Map<DocumentViewModel>(A<ContentPageModel>.Ignored)).Returns(expectedModel);
            A.CallTo(() => FakeMapper.Map<BreadcrumbViewModel>(A<ContentPageModel>.Ignored)).Returns(expectedBreadcrumbModel);

            // Act
            var result = await controller.Document(pageRequestModel).ConfigureAwait(false);

            // Assert
            A.CallTo(() => FakePagesControlerHelpers.GetContentPageAsync(A<string>.Ignored, A<string>.Ignored)).MustHaveHappenedOnceExactly();
            A.CallTo(() => FakeMapper.Map<DocumentViewModel>(A<ContentPageModel>.Ignored)).MustHaveHappenedOnceExactly();
            A.CallTo(() => FakeMapper.Map<BreadcrumbViewModel>(A<ContentPageModel>.Ignored)).MustHaveHappenedOnceExactly();

            var viewResult = Assert.IsType<ViewResult>(result);
            _ = Assert.IsAssignableFrom<DocumentViewModel>(viewResult.ViewData.Model);
            var model = viewResult.ViewData.Model as DocumentViewModel;
            Assert.Equal(expectedModel, model);

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
            var expectedResult = new ContentPageModel() { PageLocation = "/" + pageRequestModel.Location1, CanonicalName = pageRequestModel.Location2 };
            BreadcrumbViewModel? expectedBreadcrumbModel = null;
            var controller = BuildPagesController(mediaTypeName);

            A.CallTo(() => FakePagesControlerHelpers.GetContentPageAsync(A<string>.Ignored, A<string>.Ignored)).Returns(expectedResult);
            A.CallTo(() => FakeMapper.Map<DocumentViewModel>(A<ContentPageModel>.Ignored)).Returns(A.Fake<DocumentViewModel>());
            A.CallTo(() => FakeMapper.Map<BreadcrumbViewModel?>(A<ContentPageModel>.Ignored)).Returns(expectedBreadcrumbModel);

            // Act
            var result = await controller.Document(pageRequestModel).ConfigureAwait(false);

            // Assert
            A.CallTo(() => FakePagesControlerHelpers.GetContentPageAsync(A<string>.Ignored, A<string>.Ignored)).MustHaveHappenedOnceExactly();
            A.CallTo(() => FakeMapper.Map<DocumentViewModel>(A<ContentPageModel>.Ignored)).MustHaveHappenedOnceExactly();
            A.CallTo(() => FakeMapper.Map<BreadcrumbViewModel>(A<ContentPageModel>.Ignored)).MustHaveHappenedOnceExactly();

            var jsonResult = Assert.IsType<OkObjectResult>(result);
            _ = Assert.IsAssignableFrom<DocumentViewModel>(jsonResult.Value);

            controller.Dispose();
        }

        [Theory]
        [MemberData(nameof(HtmlMediaTypes))]
        [MemberData(nameof(JsonMediaTypes))]
        public async Task PagesControllerDocumentReturnsRedirectWhenRedirectLocationExists(string mediaTypeName)
        {
            // Arrange
            var pageRequestModel = new PageRequestModel
            {
                Location1 = "a-location-name",
                Location2 = "an-article-name",
            };
            ContentPageModel? expectedResult = null;
            var expectedRedirectResult = A.Fake<ContentPageModel>();
            var controller = BuildPagesController(mediaTypeName);

            expectedRedirectResult.PageLocation = pageRequestModel.Location1;
            expectedRedirectResult.CanonicalName = pageRequestModel.Location2;

            A.CallTo(() => FakePagesControlerHelpers.GetContentPageAsync(A<string>.Ignored, A<string>.Ignored)).Returns(expectedResult);
            A.CallTo(() => FakePagesControlerHelpers.GetRedirectedContentPageAsync(A<string>.Ignored, A<string>.Ignored)).Returns(expectedRedirectResult);

            // Act
            var result = await controller.Document(pageRequestModel).ConfigureAwait(false);

            // Assert
            A.CallTo(() => FakePagesControlerHelpers.GetContentPageAsync(A<string>.Ignored, A<string>.Ignored)).MustHaveHappenedOnceExactly();
            A.CallTo(() => FakePagesControlerHelpers.GetRedirectedContentPageAsync(A<string>.Ignored, A<string>.Ignored)).MustHaveHappenedOnceExactly();
            A.CallTo(() => FakeMapper.Map<DocumentViewModel>(A<ContentPageModel>.Ignored)).MustNotHaveHappened();
            A.CallTo(() => FakeMapper.Map<BreadcrumbViewModel>(A<ContentPageModel>.Ignored)).MustNotHaveHappened();

            var statusResult = Assert.IsType<RedirectResult>(result);

            statusResult.Url.Should().NotBeNullOrWhiteSpace();
            A.Equals(true, statusResult.Permanent);

            controller.Dispose();
        }

        [Theory]
        [MemberData(nameof(JsonMediaTypes))]
        [MemberData(nameof(HtmlMediaTypes))]
        public async Task PagesControllerDocumentReturnsNoContentWhenNoData(string mediaTypeName)
        {
            // Arrange
            var pageRequestModel = new PageRequestModel
            {
                Location1 = "a-location-name",
                Location2 = "an-article-name",
            };
            ContentPageModel? expectedResult = null;
            var controller = BuildPagesController(mediaTypeName);

            A.CallTo(() => FakePagesControlerHelpers.GetContentPageAsync(A<string>.Ignored, A<string>.Ignored)).Returns(expectedResult);
            A.CallTo(() => FakePagesControlerHelpers.GetRedirectedContentPageAsync(A<string>.Ignored, A<string>.Ignored)).Returns(expectedResult);

            // Act
            var result = await controller.Document(pageRequestModel).ConfigureAwait(false);

            // Assert
            A.CallTo(() => FakePagesControlerHelpers.GetContentPageAsync(A<string>.Ignored, A<string>.Ignored)).MustHaveHappenedOnceExactly();
            A.CallTo(() => FakePagesControlerHelpers.GetRedirectedContentPageAsync(A<string>.Ignored, A<string>.Ignored)).MustHaveHappenedOnceExactly();
            A.CallTo(() => FakeMapper.Map<DocumentViewModel>(A<ContentPageModel>.Ignored)).MustNotHaveHappened();
            A.CallTo(() => FakeMapper.Map<BreadcrumbViewModel>(A<ContentPageModel>.Ignored)).MustNotHaveHappened();

            var statusResult = Assert.IsType<NoContentResult>(result);

            A.Equals((int)HttpStatusCode.NoContent, statusResult.StatusCode);

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
            var expectedResult = new ContentPageModel() { PageLocation = "/" + pageRequestModel.Location1, CanonicalName = pageRequestModel.Location2 };
            BreadcrumbViewModel? expectedBreadcrumbModel = null;
            var controller = BuildPagesController(mediaTypeName);

            A.CallTo(() => FakePagesControlerHelpers.GetContentPageAsync(A<string>.Ignored, A<string>.Ignored)).Returns(expectedResult);
            A.CallTo(() => FakeMapper.Map<DocumentViewModel>(A<ContentPageModel>.Ignored)).Returns(A.Fake<DocumentViewModel>());
            A.CallTo(() => FakeMapper.Map<BreadcrumbViewModel?>(A<ContentPageModel>.Ignored)).Returns(expectedBreadcrumbModel);

            // Act
            var result = await controller.Document(pageRequestModel).ConfigureAwait(false);

            // Assert
            A.CallTo(() => FakePagesControlerHelpers.GetContentPageAsync(A<string>.Ignored, A<string>.Ignored)).MustHaveHappenedOnceExactly();
            A.CallTo(() => FakeMapper.Map<DocumentViewModel>(A<ContentPageModel>.Ignored)).MustHaveHappenedOnceExactly();
            A.CallTo(() => FakeMapper.Map<BreadcrumbViewModel>(A<ContentPageModel>.Ignored)).MustHaveHappenedOnceExactly();

            var statusResult = Assert.IsType<StatusCodeResult>(result);

            A.Equals((int)HttpStatusCode.NotAcceptable, statusResult.StatusCode);

            controller.Dispose();
        }
    }
}

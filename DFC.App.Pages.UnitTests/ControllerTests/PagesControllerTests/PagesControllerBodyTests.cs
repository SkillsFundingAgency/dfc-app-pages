using DFC.App.Pages.Data.Models;
using DFC.App.Pages.Models;
using DFC.App.Pages.Controllers;
using DFC.App.Pages.Cms.Data.Interface;
using DFC.App.Pages.Cms.Data.RequestHandler;
using DFC.App.Pages.Cms.Data.Constant;
using DFC.App.Pages.ViewModels;
using FakeItEasy;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using System.Net;
using System.Threading.Tasks;
using Xunit;
using DFC.App.Pages.Helpers;
using GraphQL.AspNet.Tests.Framework;
using FluentAssertions.Common;
using GraphQL.Client.Abstractions;
using Microsoft.Extensions.DependencyInjection;
using GraphQL.Client.Http;
using System;
using GraphQL.Client.Serializer.Newtonsoft;
using System.Net.Http;
using Moq;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using NSubstitute;
using DFC.App.Pages.Cms.Data.Model;

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
            var pageRequestModel = new PageRequestModel
            {
                Location1 = "find-a-course",
                Location2 = "find-a-course",
            };
           /* var (location, article) = PagesControlerHelpers.ExtractPageLocation(pageRequestModel);
            string pageUrl = $"/{location}/{(string.IsNullOrWhiteSpace(article) ? location : article)}";*/
            var expectedResult = new ContentPageModel() { Content = "<h1>A document</h1>", PageLocation = "/" + pageRequestModel.Location1, CanonicalName = pageRequestModel.Location2 };
            var controller = BuildPagesController(mediaTypeName);

            A.CallTo(() => FakePagesControlerHelpers.GetContentPageAsync(A<string>.Ignored, A<string>.Ignored)).Returns(expectedResult);
            A.CallTo(() => FakeMapper.Map(A<ContentPageModel>.Ignored, A<BodyViewModel>.Ignored)).Returns(A.Fake<BodyViewModel>());

            // Act
            var result = await controller.Body(pageRequestModel).ConfigureAwait(false);
           

            // Assert
            A.CallTo(() => FakePagesControlerHelpers.GetContentPageAsync(A<string>.Ignored, A<string>.Ignored)).MustHaveHappenedOnceExactly();
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
            var pageRequestModel = new PageRequestModel
            {
                Location1 = "a-location-name",
                Location2 = "a-article-name",
            };

            //var expectedResult = new ContentPageModel() { Content = "<h1>A document</h1>", PageLocation = "/" + pageRequestModel.Location1, CanonicalName = pageRequestModel.Location2 };
            var controller = BuildPagesController(mediaTypeName);
            //var pageResponse = FakePageService.GetPage(mediaTypeName);
            

            A.CallTo(() => FakePageService.GetPage(A<string>.Ignored));
            //A.CallTo(() => FakePagesControlerHelpers.GetContentPageAsync(A<string>.Ignored, A<string>.Ignored)).Returns(expectedResult);
            A.CallTo(() => FakeMapper.Map(A<PageResponse>.Ignored, A<BodyViewModel>.Ignored)).Returns(A.Fake<BodyViewModel>());

            // Act
           
            var result = await controller.Body(pageRequestModel).ConfigureAwait(false);
            


            // Assert
            A.CallTo(() => FakePageService.GetPage(A<string>.Ignored)).MustHaveHappenedOnceExactly();
            //A.CallTo(() => FakePagesControlerHelpers.GetContentPageAsync(A<string>.Ignored, A<string>.Ignored)).MustHaveHappenedOnceExactly();
            A.CallTo(() => FakeMapper.Map(A<PageResponse>.Ignored, A<BodyViewModel>.Ignored)).MustHaveHappenedOnceExactly();

            var jsonResult = Assert.IsType<OkObjectResult>(result);
            _ = Assert.IsAssignableFrom<PageResponse>(jsonResult.Value);

            controller.Dispose();
        }

        [Theory]
        [MemberData(nameof(HtmlMediaTypes))]
        public async Task PagesControllerBodyWithNullArticleHtmlReturnsSuccess(string mediaTypeName)
        {
            // Arrange
            var pageRequestModel = new PageRequestModel
            {
                Location1 = "a-location-name",
            };
            var expectedResult = new ContentPageModel() { Content = "<h1>A document</h1>", PageLocation = "/" + pageRequestModel.Location1, CanonicalName = pageRequestModel.Location2 };
            var controller = BuildPagesController(mediaTypeName);

            A.CallTo(() => FakePagesControlerHelpers.GetContentPageAsync(A<string>.Ignored, A<string>.Ignored)).Returns(expectedResult);
            A.CallTo(() => FakeMapper.Map(A<ContentPageModel>.Ignored, A<BodyViewModel>.Ignored)).Returns(A.Fake<BodyViewModel>());

            // Act
            var result = await controller.Body(pageRequestModel).ConfigureAwait(false);

            // Assert
            A.CallTo(() => FakePagesControlerHelpers.GetContentPageAsync(A<string>.Ignored, A<string>.Ignored)).MustHaveHappenedOnceExactly();
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
            var pageRequestModel = new PageRequestModel
            {
                Location1 = "a-location-name",
            };
            var expectedResult = new ContentPageModel() { Content = "<h1>A document</h1>", PageLocation = "/" + pageRequestModel.Location1, CanonicalName = pageRequestModel.Location2 };
            var controller = BuildPagesController(mediaTypeName);

            A.CallTo(() => FakePagesControlerHelpers.GetContentPageAsync(A<string>.Ignored, A<string>.Ignored)).Returns(expectedResult);
            A.CallTo(() => FakeMapper.Map(A<ContentPageModel>.Ignored, A<BodyViewModel>.Ignored)).Returns(A.Fake<BodyViewModel>());

            // Act
            var result = await controller.Body(pageRequestModel).ConfigureAwait(false);

            // Assert
            A.CallTo(() => FakePagesControlerHelpers.GetContentPageAsync(A<string>.Ignored, A<string>.Ignored)).MustHaveHappenedOnceExactly();
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
            var pageRequestModel = new PageRequestModel
            {
                Location1 = "a-location-name",
                Location2 = "an-article-name",
            };
            ContentPageModel? expectedResult = null;
            var expectedRedirectResult = A.Fake<ContentPageModel>();
            var controller = BuildPagesController(mediaTypeName);

            expectedRedirectResult.PageLocation = "/" + pageRequestModel.Location1;
            expectedRedirectResult.CanonicalName = pageRequestModel.Location2;

            A.CallTo(() => FakePagesControlerHelpers.GetContentPageAsync(A<string>.Ignored, A<string>.Ignored)).Returns(expectedResult);
            A.CallTo(() => FakePagesControlerHelpers.GetRedirectedContentPageAsync(A<string>.Ignored, A<string>.Ignored)).Returns(expectedRedirectResult);

            // Act
            var result = await controller.Body(pageRequestModel).ConfigureAwait(false);

            // Assert
            A.CallTo(() => FakePagesControlerHelpers.GetContentPageAsync(A<string>.Ignored, A<string>.Ignored)).MustHaveHappenedOnceExactly();
            A.CallTo(() => FakePagesControlerHelpers.GetRedirectedContentPageAsync(A<string>.Ignored, A<string>.Ignored)).MustHaveHappenedOnceExactly();

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
            var pageRequestModel = new PageRequestModel
            {
                Location1 = "a-location-name",
                Location2 = "an-article-name",
            };
            ContentPageModel? expectedResult = null;
            ContentPageModel? expectedRedirectResult = null;
            var controller = BuildPagesController(mediaTypeName);

            A.CallTo(() => FakePagesControlerHelpers.GetContentPageAsync(A<string>.Ignored, A<string>.Ignored)).Returns(expectedResult);
            A.CallTo(() => FakePagesControlerHelpers.GetRedirectedContentPageAsync(A<string>.Ignored, A<string>.Ignored)).Returns(expectedRedirectResult);

            // Act
            var result = await controller.Body(pageRequestModel).ConfigureAwait(false);

            // Assert
            A.CallTo(() => FakePagesControlerHelpers.GetContentPageAsync(A<string>.Ignored, A<string>.Ignored)).MustHaveHappenedOnceExactly();
            A.CallTo(() => FakePagesControlerHelpers.GetRedirectedContentPageAsync(A<string>.Ignored, A<string>.Ignored)).MustHaveHappenedOnceExactly();

            var statusResult = Assert.IsType<NotFoundResult>(result);

            A.Equals((int)HttpStatusCode.NotFound, statusResult.StatusCode);

            controller.Dispose();
        }

        [Theory]
        [MemberData(nameof(InvalidMediaTypes))]
        public async Task PagesControllerBodyReturnsNotAcceptable(string mediaTypeName)
        {
            // Arrange
            var pageRequestModel = new PageRequestModel
            {
                Location1 = "a-location-name",
                Location2 = "an-article-name",
            };
            var expectedResult = new ContentPageModel() { Content = "<h1>A document</h1>", PageLocation = "/" + pageRequestModel.Location1, CanonicalName = pageRequestModel.Location2 };
            var controller = BuildPagesController(mediaTypeName);

            A.CallTo(() => FakePagesControlerHelpers.GetContentPageAsync(A<string>.Ignored, A<string>.Ignored)).Returns(expectedResult);
            A.CallTo(() => FakeMapper.Map(A<ContentPageModel>.Ignored, A<BodyViewModel>.Ignored)).Returns(A.Fake<BodyViewModel>());

            // Act
            var result = await controller.Body(pageRequestModel).ConfigureAwait(false);

            // Assert
            A.CallTo(() => FakePagesControlerHelpers.GetContentPageAsync(A<string>.Ignored, A<string>.Ignored)).MustHaveHappenedOnceExactly();
            A.CallTo(() => FakeMapper.Map(A<ContentPageModel>.Ignored, A<BodyViewModel>.Ignored)).MustHaveHappenedOnceExactly();

            var statusResult = Assert.IsType<StatusCodeResult>(result);

            A.Equals((int)HttpStatusCode.NotAcceptable, statusResult.StatusCode);

            controller.Dispose();
        }
    }
}

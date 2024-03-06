using DFC.App.Pages.Data.Models;
using DFC.App.Pages.Models;
using DFC.App.Pages.ViewModels;
using DFC.Common.SharedContent.Pkg.Netcore.Interfaces;
using DFC.Common.SharedContent.Pkg.Netcore.Model.ContentItems;
using DFC.Common.SharedContent.Pkg.Netcore.Model.Response;
using FakeItEasy;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Moq;
using System;
using System.Collections.Generic;
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
            var pageRequestModel = new PageRequestModel
            {
                Location1 = "a-location-name",
                Location2 = "an-article-name",
            };

            var controller = BuildPagesController(mediaTypeName);

            A.CallTo(() => FakeMapper.Map(A<ContentPageModel>.Ignored, A<BodyViewModel>.Ignored)).Returns(A.Fake<BodyViewModel>());

            // Act
            var result = await controller.Body(pageRequestModel).ConfigureAwait(false);

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
                Location2 = "an-article-name",
            };
            var controller = BuildPagesController(mediaTypeName);

            A.CallTo(() => FakeMapper.Map(A<ContentPageModel>.Ignored, A<BodyViewModel>.Ignored)).Returns(A.Fake<BodyViewModel>());

            // Act
            var result = await controller.Body(pageRequestModel).ConfigureAwait(false);

            var jsonResult = Assert.IsType<OkObjectResult>(result);
            _ = Assert.IsAssignableFrom<BodyViewModel>(jsonResult.Value);

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
            var controller = BuildPagesController(mediaTypeName);

            A.CallTo(() => FakeMapper.Map(A<ContentPageModel>.Ignored, A<BodyViewModel>.Ignored)).Returns(A.Fake<BodyViewModel>());

            // Act
            var result = await controller.Body(pageRequestModel).ConfigureAwait(false);

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
            var controller = BuildPagesController(mediaTypeName);

            A.CallTo(() => FakeMapper.Map(A<ContentPageModel>.Ignored, A<BodyViewModel>.Ignored)).Returns(A.Fake<BodyViewModel>());

            // Act
            var result = await controller.Body(pageRequestModel).ConfigureAwait(false);

            var jsonResult = Assert.IsType<OkObjectResult>(result);
            _ = Assert.IsAssignableFrom<BodyViewModel>(jsonResult.Value);

            controller.Dispose();
        }

        [Theory]
        [MemberData(nameof(HtmlMediaTypes))]
        public async Task PagesControllerBodyReturnsViewModel(string mediaTypeName)
        {
            // Arrange
            var pageRequestModel = new PageRequestModel
            {
                Location1 = "a-location-name",
                Location2 = "an-article-name",
            };
            var controller = BuildPagesController(mediaTypeName);

            A.CallTo(() => FakeMapper.Map(A<ContentPageModel>.Ignored, A<BodyViewModel>.Ignored)).Returns(A.Fake<BodyViewModel>());

            // Act
            var result = await controller.Body(pageRequestModel).ConfigureAwait(false);

            var viewResult = Assert.IsType<ViewResult>(result);

            _ = Assert.IsAssignableFrom<BodyViewModel>(viewResult.ViewData.Model);

            controller.Dispose();
        }

        [Theory]
        [MemberData(nameof(JsonMediaTypes))]
        public async Task PagesControllerBodyJsonReturnsBodyViewModel(string mediaTypeName)
        {
            // Arrange
            var pageRequestModel = new PageRequestModel
            {
                Location1 = "a-location-name",
                Location2 = "an-article-name",
            };
            var controller = BuildPagesController(mediaTypeName);

            A.CallTo(() => FakeMapper.Map(A<ContentPageModel>.Ignored, A<BodyViewModel>.Ignored)).Returns(A.Fake<BodyViewModel>());

            // Act
            var result = await controller.Body(pageRequestModel).ConfigureAwait(false);

            var jsonResult = Assert.IsType<OkObjectResult>(result);
            _ = Assert.IsAssignableFrom<BodyViewModel>(jsonResult.Value);

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
            var controller = BuildPagesController(mediaTypeName);

            A.CallTo(() => FakeMapper.Map(A<ContentPageModel>.Ignored, A<BodyViewModel>.Ignored)).Returns(A.Fake<BodyViewModel>());

            // Act
            var result = await controller.Body(pageRequestModel).ConfigureAwait(false);

            var statusResult = Assert.IsType<StatusCodeResult>(result);

            A.Equals((int)HttpStatusCode.NotAcceptable, statusResult.StatusCode);

            controller.Dispose();
        }
    }
}
using DFC.App.Pages.Controllers;
using DFC.App.Pages.Data.Models;
using FakeItEasy;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Net.Http.Headers;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Net;
using System.Net.Mime;
using System.Threading.Tasks;
using Xunit;

namespace DFC.App.Pages.UnitTests.ControllerTests.PagesControllerTests
{
    [Trait("Category", "Pages Controller Unit Tests")]
    public class PagesControllerRouteTests : BasePagesControllerTests
    {
        public static IEnumerable<object[]> PagesRouteDataOk => new List<object[]>
        {
            new object[] { "/", string.Empty, string.Empty, nameof(PagesController.Index) },
            new object[] { "/pages", string.Empty, string.Empty, nameof(PagesController.Index) },
            new object[] { "/pages/{article}", "SomeLocation", "SomeArticle", nameof(PagesController.Document) },
            new object[] { "/pages/{article}/htmlhead", "SomeLocation", "SomeArticle", nameof(PagesController.HtmlHead) },
            new object[] { "/pages/htmlhead", string.Empty, string.Empty, nameof(PagesController.HtmlHead) },
            new object[] { "/pages/{article}/breadcrumb", "SomeLocation", "SomeArticle", nameof(PagesController.Breadcrumb) },
            new object[] { "/pages/breadcrumb", string.Empty, string.Empty, nameof(PagesController.Breadcrumb) },
            new object[] { "/pages/{article}/body", "SomeLocation", "SomeArticle", nameof(PagesController.Body) },
            new object[] { "/pages/body", string.Empty, string.Empty, nameof(PagesController.Body) },
        };

        public static IEnumerable<object[]> PagesRouteDataNoContent => new List<object[]>
        {
            new object[] { "/pages/{article}/bodytop", "SomeLocation", "SomeArticle", nameof(PagesController.BodyTop) },
            new object[] { "/pages/bodytop", string.Empty, string.Empty, nameof(PagesController.BodyTop) },
            new object[] { "/pages/{article}/herobanner", "SomeLocation", "SomeArticle", nameof(PagesController.HeroBanner) },
            new object[] { "/pages/herobanner", string.Empty, string.Empty, nameof(PagesController.HeroBanner) },
            new object[] { "/pages/{article}/sidebarright", "SomeLocation", "SomeArticle", nameof(PagesController.SidebarRight) },
            new object[] { "/pages/sidebarright", string.Empty, string.Empty, nameof(PagesController.SidebarRight) },
            new object[] { "/pages/{article}/sidebarleft", "SomeLocation", "SomeArticle", nameof(PagesController.SidebarLeft) },
            new object[] { "/pages/sidebarleft", string.Empty, string.Empty, nameof(PagesController.SidebarLeft) },
            new object[] { "/pages/{article}/bodyfooter", "SomeLocation", "SomeArticle", nameof(PagesController.BodyFooter) },
            new object[] { "/pages/bodyfooter", string.Empty, string.Empty, nameof(PagesController.BodyFooter) },
        };

        [Theory]
        [MemberData(nameof(PagesRouteDataOk))]
        public async Task PagesControllerCallsContentPageServiceUsingPagesRouteForOkResult(string route, string location, string article, string actionMethod)
        {
            // Arrange
            var controller = BuildController(route);
            var expectedResults = new List<ContentPageModel> { new ContentPageModel() { Content = "<h1>A document</h1>" } };

            A.CallTo(() => FakeContentPageService.GetAsync(A<Expression<Func<ContentPageModel, bool>>>.Ignored)).Returns(expectedResults);

            // Act
            var result = await RunControllerAction(controller, location, article, actionMethod).ConfigureAwait(false);

            // Assert
            Assert.IsType<OkObjectResult>(result);
            A.CallTo(() => FakeContentPageService.GetAsync(A<Expression<Func<ContentPageModel, bool>>>.Ignored)).MustHaveHappenedOnceExactly();

            controller.Dispose();
        }

        [Theory]
        [MemberData(nameof(PagesRouteDataNoContent))]
        public async Task PagesControllerCallsContentPageServiceUsingPagesRouteFornoContentResult(string route, string location, string article, string actionMethod)
        {
            // Arrange
            var controller = BuildController(route);

            // Act
            var result = await RunControllerAction(controller, location, article, actionMethod).ConfigureAwait(false);

            // Assert
            var statusResult = Assert.IsType<NoContentResult>(result);

            A.Equals((int)HttpStatusCode.NoContent, statusResult.StatusCode);

            controller.Dispose();
        }

        private static async Task<IActionResult> RunControllerAction(PagesController controller, string location, string article, string actionName)
        {
            return actionName switch
            {
                nameof(PagesController.HtmlHead) => await controller.HtmlHead(location, article).ConfigureAwait(false),
                nameof(PagesController.Breadcrumb) => await controller.Breadcrumb(location, article).ConfigureAwait(false),
                nameof(PagesController.BodyTop) => controller.BodyTop(location, article),
                nameof(PagesController.HeroBanner) => controller.HeroBanner(location, article),
                nameof(PagesController.SidebarRight) => controller.SidebarRight(location, article),
                nameof(PagesController.SidebarLeft) => controller.SidebarLeft(location, article),
                nameof(PagesController.BodyFooter) => controller.BodyFooter(location, article),
                _ => await controller.Body(location, article).ConfigureAwait(false),
            };
        }

        private PagesController BuildController(string route)
        {
            var httpContext = new DefaultHttpContext();
            httpContext.Request.Path = route;
            httpContext.Request.Headers[HeaderNames.Accept] = MediaTypeNames.Application.Json;

            return new PagesController(Logger, FakeContentPageService, FakeMapper)
            {
                ControllerContext = new ControllerContext
                {
                    HttpContext = httpContext,
                },
            };
        }
    }
}
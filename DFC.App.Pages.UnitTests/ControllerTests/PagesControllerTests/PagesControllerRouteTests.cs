using DFC.App.Pages.Controllers;
using DFC.App.Pages.Models;
using DFC.Common.SharedContent.Pkg.Netcore.Model.ContentItems;
using FakeItEasy;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Net.Http.Headers;
using System.Collections.Generic;
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
            new object[] { "/", string.Empty, string.Empty, string.Empty, string.Empty, string.Empty, nameof(PagesController.Index) },
            new object[] { "/pages", string.Empty, string.Empty, string.Empty, string.Empty, string.Empty, nameof(PagesController.Index) },

            new object[] { "/pages/document", string.Empty, string.Empty, string.Empty, string.Empty, string.Empty, nameof(PagesController.Document) },
            new object[] { "/pages/{location1}/document", "SomeLocation1", string.Empty, string.Empty, string.Empty, string.Empty, nameof(PagesController.Document) },
            new object[] { "/pages/{location1}/{location2}/document", "SomeLocation1", "SomeLocation2", string.Empty, string.Empty, string.Empty, nameof(PagesController.Document) },
            new object[] { "/pages/{location1}/{location2}/{location3}/document", "SomeLocation1", "SomeLocation2", "SomeLocation3", string.Empty, string.Empty, nameof(PagesController.Document) },
            new object[] { "/pages/{location1}/{location2}/{location3}/{location4}/document", "SomeLocation1", "SomeLocation2", "SomeLocation3", "SomeLocation4", string.Empty, nameof(PagesController.Document) },
            new object[] { "/pages/{location1}/{location2}/{location3}/{location4}/{location5}/document", "SomeLocation1", "SomeLocation2", "SomeLocation3", "SomeLocation4", "SomeLocation5", nameof(PagesController.Document) },

            new object[] { "/pages/head", string.Empty, string.Empty, string.Empty, string.Empty, string.Empty, nameof(PagesController.Head) },
            new object[] { "/pages/{location1}/head", "SomeLocation1", string.Empty, string.Empty, string.Empty, string.Empty, nameof(PagesController.Head) },
            new object[] { "/pages/{location1}/{location2}/head", "SomeLocation1", "SomeLocation2", string.Empty, string.Empty, string.Empty, nameof(PagesController.Head) },
            new object[] { "/pages/{location1}/{location2}/{location3}/head", "SomeLocation1", "SomeLocation2", "SomeLocation3", string.Empty, string.Empty, nameof(PagesController.Head) },
            new object[] { "/pages/{location1}/{location2}/{location3}/{location4}/head", "SomeLocation1", "SomeLocation2", "SomeLocation3", "SomeLocation4", string.Empty, nameof(PagesController.Head) },
            new object[] { "/pages/{location1}/{location2}/{location3}/{location4}/{location5}/head", "SomeLocation1", "SomeLocation2", "SomeLocation3", "SomeLocation4", "SomeLocation5", nameof(PagesController.Head) },

            new object[] { "/pages/herobanner", string.Empty, string.Empty, string.Empty, string.Empty, string.Empty, nameof(PagesController.HeroBanner) },
            new object[] { "/pages/{location1}/herobanner", "SomeLocation1", string.Empty, string.Empty, string.Empty, string.Empty, nameof(PagesController.HeroBanner) },
            new object[] { "/pages/{location1}{location2}/herobanner", "SomeLocation1", "SomeLocation2", string.Empty, string.Empty, string.Empty, nameof(PagesController.HeroBanner) },
            new object[] { "/pages/{location1}/{location2}/{location3}/herobanner", "SomeLocation1", "SomeLocation2", "SomeLocation3", string.Empty, string.Empty, nameof(PagesController.HeroBanner) },
            new object[] { "/pages/{location1}/{location2}/{location3}/{location4}/herobanner", "SomeLocation1", "SomeLocation2", "SomeLocation3", "SomeLocation4", string.Empty, nameof(PagesController.HeroBanner) },
            new object[] { "/pages/{location1}/{location2}/{location3}/{location4}/{location5}/herobanner", "SomeLocation1", "SomeLocation2", "SomeLocation3", "SomeLocation4", "SomeLocation5", nameof(PagesController.HeroBanner) },

            new object[] { "/pages/body", string.Empty, string.Empty, string.Empty, string.Empty, string.Empty, nameof(PagesController.Body) },
            new object[] { "/pages/{location1}/body", "SomeLocation1", string.Empty, string.Empty, string.Empty, string.Empty, nameof(PagesController.Body) },
            new object[] { "/pages/{location1}/{location2}/body", "SomeLocation1", "SomeLocation2", string.Empty, string.Empty, string.Empty, nameof(PagesController.Body) },
            new object[] { "/pages/{location1}/{location2}/{location3}/body", "SomeLocation1", "SomeLocation2", "SomeLocation3", string.Empty, string.Empty, nameof(PagesController.Body) },
            new object[] { "/pages/{location1}/{location2}/{location3}/{location4}/body", "SomeLocation1", "SomeLocation2", "SomeLocation3", "SomeLocation4", string.Empty, nameof(PagesController.Body) },
            new object[] { "/pages/{location1}/{location2}/{location3}/{location4}/{location5}/body", "SomeLocation1", "SomeLocation2", "SomeLocation3", "SomeLocation4", "SomeLocation5", nameof(PagesController.Body) },
        };

        public static IEnumerable<object[]> PagesRouteDataNoContent => new List<object[]>
        {
            new object[] { "/pages/bodytop", string.Empty, string.Empty, string.Empty, string.Empty, string.Empty, nameof(PagesController.BodyTop) },
            new object[] { "/pages/{location1}/bodytop", "SomeLocation1", string.Empty, string.Empty, string.Empty, string.Empty, nameof(PagesController.BodyTop) },
            new object[] { "/pages/{location1}{location2}/bodytop", "SomeLocation1", "SomeLocation2", string.Empty, string.Empty, string.Empty, nameof(PagesController.BodyTop) },
            new object[] { "/pages/{location1}/{location2}/{location3}/bodytop", "SomeLocation1", "SomeLocation2", "SomeLocation3", string.Empty, string.Empty, nameof(PagesController.BodyTop) },
            new object[] { "/pages/{location1}/{location2}/{location3}/{location4}/bodytop", "SomeLocation1", "SomeLocation2", "SomeLocation3", "SomeLocation4", string.Empty, nameof(PagesController.BodyTop) },
            new object[] { "/pages/{location1}/{location2}/{location3}/{location4}/{location5}/bodytop", "SomeLocation1", "SomeLocation2", "SomeLocation3", "SomeLocation4", "SomeLocation5", nameof(PagesController.BodyTop) },

            new object[] { "/pages/sidebarright", string.Empty, string.Empty, string.Empty, string.Empty, string.Empty, nameof(PagesController.SidebarRight) },
            new object[] { "/pages/{location1}/sidebarright", "SomeLocation1", string.Empty, string.Empty, string.Empty, string.Empty, nameof(PagesController.SidebarRight) },
            new object[] { "/pages/{location1}{location2}/sidebarright", "SomeLocation1", "SomeLocation2", string.Empty, string.Empty, string.Empty, nameof(PagesController.SidebarRight) },
            new object[] { "/pages/{location1}/{location2}/{location3}/sidebarright", "SomeLocation1", "SomeLocation2", "SomeLocation3", string.Empty, string.Empty, nameof(PagesController.SidebarRight) },
            new object[] { "/pages/{location1}/{location2}/{location3}/{location4}/sidebarright", "SomeLocation1", "SomeLocation2", "SomeLocation3", "SomeLocation4", string.Empty, nameof(PagesController.SidebarRight) },
            new object[] { "/pages/{location1}/{location2}/{location3}/{location4}/{location5}/sidebarright", "SomeLocation1", "SomeLocation2", "SomeLocation3", "SomeLocation4", "SomeLocation5", nameof(PagesController.SidebarRight) },

            new object[] { "/pages/sidebarleft", string.Empty, string.Empty, string.Empty, string.Empty, string.Empty, nameof(PagesController.SidebarLeft) },
            new object[] { "/pages/{location1}/sidebarleft", "SomeLocation1", string.Empty, string.Empty, string.Empty, string.Empty, nameof(PagesController.SidebarLeft) },
            new object[] { "/pages/{location1}{location2}/sidebarleft", "SomeLocation1", "SomeLocation2", string.Empty, string.Empty, string.Empty, nameof(PagesController.SidebarLeft) },
            new object[] { "/pages/{location1}/{location2}/{location3}/sidebarleft", "SomeLocation1", "SomeLocation2", "SomeLocation3", string.Empty, string.Empty, nameof(PagesController.SidebarLeft) },
            new object[] { "/pages/{location1}/{location2}/{location3}/{location4}/sidebarleft", "SomeLocation1", "SomeLocation2", "SomeLocation3", "SomeLocation4", string.Empty, nameof(PagesController.SidebarLeft) },
            new object[] { "/pages/{location1}/{location2}/{location3}/{location4}/{location5}/sidebarleft", "SomeLocation1", "SomeLocation2", "SomeLocation3", "SomeLocation4", "SomeLocation5", nameof(PagesController.SidebarLeft) },

            new object[] { "/pages/bodyfooter", string.Empty, string.Empty, string.Empty, string.Empty, string.Empty, nameof(PagesController.BodyFooter) },
            new object[] { "/pages/{location1}/bodyfooter", "SomeLocation1", string.Empty, string.Empty, string.Empty, string.Empty, nameof(PagesController.BodyFooter) },
            new object[] { "/pages/{location1}{location2}/bodyfooter", "SomeLocation1", "SomeLocation2", string.Empty, string.Empty, string.Empty, nameof(PagesController.BodyFooter) },
            new object[] { "/pages/{location1}/{location2}/{location3}/bodyfooter", "SomeLocation1", "SomeLocation2", "SomeLocation3", string.Empty, string.Empty, nameof(PagesController.BodyFooter) },
            new object[] { "/pages/{location1}/{location2}/{location3}/{location4}/bodyfooter", "SomeLocation1", "SomeLocation2", "SomeLocation3", "SomeLocation4", string.Empty, nameof(PagesController.BodyFooter) },
            new object[] { "/pages/{location1}/{location2}/{location3}/{location4}/{location5}/bodyfooter", "SomeLocation1", "SomeLocation2", "SomeLocation3", "SomeLocation4", "SomeLocation5", nameof(PagesController.BodyFooter) },

            new object[] { "/pages/breadcrumb", string.Empty, string.Empty, string.Empty, string.Empty, string.Empty, nameof(PagesController.Breadcrumb) },
            new object[] { "/pages/{location1}/breadcrumb", "SomeLocation1", string.Empty, string.Empty, string.Empty, string.Empty, nameof(PagesController.Breadcrumb) },
            new object[] { "/pages/{location1}/{location2}/breadcrumb", "SomeLocation1", "SomeLocation2", string.Empty, string.Empty, string.Empty, nameof(PagesController.Breadcrumb) },
            new object[] { "/pages/{location1}/{location2}/{location3}/breadcrumb", "SomeLocation1", "SomeLocation2", "SomeLocation3", string.Empty, string.Empty, nameof(PagesController.Breadcrumb) },
            new object[] { "/pages/{location1}/{location2}/{location3}/{location4}/breadcrumb", "SomeLocation1", "SomeLocation2", "SomeLocation3", "SomeLocation4", string.Empty, nameof(PagesController.Breadcrumb) },
            new object[] { "/pages/{location1}/{location2}/{location3}/{location4}/{location5}/breadcrumb", "SomeLocation1", "SomeLocation2", "SomeLocation3", "SomeLocation4", "SomeLocation5", nameof(PagesController.Breadcrumb) },
        };

        [Theory]
        [MemberData(nameof(PagesRouteDataOk))]
        public async Task PagesControllerCallsContentPageServiceUsingPagesRouteForOkResult(string route, string? location1, string? location2, string? location3, string? location4, string? location5, string actionMethod)
        {
            // Arrange
            var pageRequestModel = new PageRequestModel
            {
                Location1 = location1,
                Location2 = location2,
                Location3 = location3,
                Location4 = location4,
                Location5 = location5,
            };
            var controller = BuildController(route);
            var expected = new Page()
            {
                Herobanner = new()
                {
                    Html = "This is a hero banner",
                },

            };

            A.CallTo(() => FakeSharedContentRedisInterface.GetDataAsyncWithExpiry<Page>("PageTest", "PUBLISHED", 4)).Returns(expected);

            // Act
            var result = await RunControllerAction(controller, pageRequestModel, actionMethod).ConfigureAwait(false);

            // Assert
            Assert.IsType<OkObjectResult>(result);

            controller.Dispose();
        }

        [Theory]
        [MemberData(nameof(PagesRouteDataNoContent))]
        public async Task PagesControllerCallsContentPageServiceUsingPagesRouteForNoContentResult(string route, string? location1, string? location2, string? location3, string? location4, string? location5, string actionMethod)
        {
            // Arrange
            var pageRequestModel = new PageRequestModel
            {
                Location1 = location1,
                Location2 = location2,
                Location3 = location3,
                Location4 = location4,
                Location5 = location5,
            };
            var controller = BuildController(route);

            // Act
            var result = await RunControllerAction(controller, pageRequestModel, actionMethod).ConfigureAwait(false);

            // Assert
            var statusResult = Assert.IsType<NoContentResult>(result);

            A.Equals((int)HttpStatusCode.NoContent, statusResult.StatusCode);

            controller.Dispose();
        }

        private static async Task<IActionResult> RunControllerAction(PagesController controller, PageRequestModel pageRequestModel, string actionName)
        {
            return actionName switch
            {
                nameof(PagesController.Head) => await controller.Head(pageRequestModel).ConfigureAwait(false),
                nameof(PagesController.Breadcrumb) => await controller.Breadcrumb(pageRequestModel).ConfigureAwait(false),
                nameof(PagesController.BodyTop) => controller.BodyTop(pageRequestModel),
                nameof(PagesController.HeroBanner) => await controller.HeroBanner(pageRequestModel).ConfigureAwait(false),
                nameof(PagesController.SidebarRight) => controller.SidebarRight(pageRequestModel),
                nameof(PagesController.SidebarLeft) => controller.SidebarLeft(pageRequestModel),
                nameof(PagesController.BodyFooter) => controller.BodyFooter(pageRequestModel),
                _ => await controller.Body(pageRequestModel).ConfigureAwait(false),
            };
        }

        private PagesController BuildController(string route)
        {
            var httpContext = new DefaultHttpContext();
            httpContext.Request.Path = route;
            httpContext.Request.Headers[HeaderNames.Accept] = MediaTypeNames.Application.Json;

            return new PagesController(FakeConfiguration, Logger, FakeMapper, FakeSharedContentRedisInterface, FakeContentOptions)
            {
                ControllerContext = new ControllerContext
                {
                    HttpContext = httpContext,
                },
            };
        }
    }
}
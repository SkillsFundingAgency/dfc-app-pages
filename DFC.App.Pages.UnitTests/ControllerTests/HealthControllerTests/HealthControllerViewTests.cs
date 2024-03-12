using DFC.App.Pages.ViewModels;
using FakeItEasy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using Xunit;

namespace DFC.App.Pages.UnitTests.ControllerTests.HealthControllerTests
{
    [Trait("Category", "Health Controller Unit Tests")]
    public class HealthControllerViewTests : BaseHealthControllerTests
    {
        [Theory]
        [MemberData(nameof(HtmlMediaTypes))]
        public async Task HealthControllerViewHtmlReturnsSuccess(string mediaTypeName)
        {
            // Arrange
            var service = CreateHealthChecksService(b =>
            {
                b.AddAsyncCheck("HealthyCheck", _ => Task.FromResult(HealthCheckResult.Healthy()));
            });
            var controller = BuildHealthController(mediaTypeName, service);

            // Act
            var healthCheckResult = await service.CheckHealthAsync();
            var controllerResult = await controller.Health().ConfigureAwait(false);

            // Assert
            var viewResult = Assert.IsType<ViewResult>(controllerResult);
            _ = Assert.IsAssignableFrom<HealthViewModel>(viewResult.ViewData.Model);

            controller.Dispose();
        }

        [Theory]
        [MemberData(nameof(JsonMediaTypes))]
        public async Task HealthControllerViewJsonReturnsSuccess(string mediaTypeName)
        {
            // Arrange
            var service = CreateHealthChecksService(b =>
            {
                b.AddAsyncCheck("HealthyCheck", _ => Task.FromResult(HealthCheckResult.Healthy()));
            });
            var controller = BuildHealthController(mediaTypeName, service);

            // Act
            var healthCheckResult = await service.CheckHealthAsync();
            var controllerResult = await controller.Health().ConfigureAwait(false);

            // Assert
            var jsonResult = Assert.IsType<OkObjectResult>(controllerResult);
            _ = Assert.IsAssignableFrom<IList<HealthItemViewModel>>(jsonResult.Value);

            controller.Dispose();
        }

        [Theory]
        [MemberData(nameof(InvalidMediaTypes))]
        public async Task HealthControllerHealthViewReturnsNotAcceptable(string mediaTypeName)
        {
            // Arrange
            var service = CreateHealthChecksService(b =>
            {
                b.AddAsyncCheck("HealthyCheck", _ => Task.FromResult(HealthCheckResult.Healthy()));
            });
            var controller = BuildHealthController(mediaTypeName, service);

            // Act
            var healthCheckResult = await service.CheckHealthAsync();
            var controllerResult = await controller.Health().ConfigureAwait(false);

            // Assert
            var statusResult = Assert.IsType<StatusCodeResult>(controllerResult);

            A.Equals((int)HttpStatusCode.NotAcceptable, statusResult.StatusCode);

            controller.Dispose();
        }
    }
}

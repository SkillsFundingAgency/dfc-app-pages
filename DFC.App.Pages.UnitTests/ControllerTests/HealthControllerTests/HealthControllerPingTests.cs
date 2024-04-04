using FakeItEasy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using System.Collections.Generic;
using System.Net;
using System.Net.Mime;
using System.Threading.Tasks;
using Xunit;

namespace DFC.App.Pages.UnitTests.ControllerTests.HealthControllerTests
{
    [Trait("Category", "Health Controller Unit Tests")]
    public class HealthControllerPingTests : BaseHealthControllerTests
    {
        [Fact]
        public void HealthControllerPingReturnsSuccess()
        {
            // Arrange
            var service = CreateHealthChecksService(b =>
            {
                b.AddAsyncCheck("HealthyCheck", _ => Task.FromResult(HealthCheckResult.Healthy()));
            });
            var controller = BuildHealthController(MediaTypeNames.Application.Json, service);

            // Act
            var result = controller.Ping();

            // Assert
            var statusResult = Assert.IsType<OkResult>(result);

            A.Equals((int)HttpStatusCode.OK, statusResult.StatusCode);

            controller.Dispose();
        }
    }
}

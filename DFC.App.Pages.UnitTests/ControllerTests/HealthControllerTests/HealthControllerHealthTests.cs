using DFC.App.Pages.ViewModels;
using FakeItEasy;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Net;
using System.Net.Mime;
using System.Threading.Tasks;
using Xunit;

namespace DFC.App.Pages.UnitTests.ControllerTests.HealthControllerTests
{
    [Trait("Category", "Health Controller Unit Tests")]
    public class HealthControllerHealthTests : BaseHealthControllerTests
    {
        [Fact]
        public async Task HealthControllerHealthReturnsSuccessWhenHealthy()
        {
            // Arrange
            var service = CreateHealthChecksService(b =>
            {
                b.AddAsyncCheck("HealthyCheck", _ => Task.FromResult(HealthCheckResult.Healthy()));
            });
            var controller = BuildHealthController(MediaTypeNames.Application.Json, service);

            // Act
            var healthCheckResult = await service.CheckHealthAsync();
            var controllerResult = await controller.Health().ConfigureAwait(false);

            // Assert
            Assert.Collection(
                healthCheckResult.Entries,
                actual =>
                {
                    var jsonResult = Assert.IsType<OkObjectResult>(controllerResult);
                    var models = Assert.IsAssignableFrom<List<HealthItemViewModel>>(jsonResult.Value);
                    models.Count.Should().BeGreaterThan(0);
                    models.First().Service.Should().NotBeNullOrWhiteSpace();
                    models.First().Message.Should().NotBeNullOrWhiteSpace();
                    Assert.Equal("HealthyCheck", actual.Key);
                    Assert.Equal(HealthStatus.Healthy, actual.Value.Status);
                    Assert.Null(actual.Value.Exception);
                });

            controller.Dispose();
        }

        [Fact]
        public async Task HealthControllerHealthReturnsServiceUnavailableWhenUnhealthy()
        {
            // Arrange
            var service = CreateHealthChecksService(b =>
            {
                b.AddAsyncCheck("timeout", async (ct) =>
                {
                    await Task.Delay(2000, ct);
                    return HealthCheckResult.Healthy();
                }, timeout: TimeSpan.FromMilliseconds(100));
            });
            var controller = BuildHealthController(MediaTypeNames.Application.Json, service);

            // Act
            var healthCheckResult = await service.CheckHealthAsync();
            var controllerResult = await controller.Health().ConfigureAwait(false);

            // Assert
            Assert.Collection(
                healthCheckResult.Entries,
                actual =>
                {
                    Assert.Equal("timeout", actual.Key);
                    Assert.Equal(HealthStatus.Unhealthy, actual.Value.Status);
                    var statusResult = Assert.IsType<StatusCodeResult>(controllerResult);
                    A.Equals((int)HttpStatusCode.ServiceUnavailable, statusResult.StatusCode);
                });

            controller.Dispose();
        }

        [Fact]
        public async Task HealthControllerHealthReturnsServiceUnavailableWhenException()
        {
            // Arrange
            const string ExceptionMessage = "exception-message";
            var exception = new Exception();

            var service = CreateHealthChecksService(b =>
            {
                b.AddAsyncCheck("UnhealthyCheck", _ => Task.FromResult(HealthCheckResult.Unhealthy(null, exception)));
                b.AddAsyncCheck("ExceptionCheck", _ => throw new Exception(ExceptionMessage));
            });
            var controller = BuildHealthController(MediaTypeNames.Application.Json, service);

            // Act
            var results = await service.CheckHealthAsync();
            var result = await controller.Health().ConfigureAwait(false);

            // Assert
            var statusResult = Assert.IsType<StatusCodeResult>(result);

            Assert.Collection(
                results.Entries.OrderBy(kvp => kvp.Key),
                actual =>
                {
                    Assert.Equal("ExceptionCheck", actual.Key);
                    Assert.Equal(ExceptionMessage, actual.Value.Description);
                    Assert.Equal(HealthStatus.Unhealthy, actual.Value.Status);
                    Assert.Equal(ExceptionMessage, actual.Value.Exception!.Message);
                    Assert.Empty(actual.Value.Data);
                    A.Equals((int)HttpStatusCode.ServiceUnavailable, statusResult.StatusCode);
                },
                actual =>
                {
                    Assert.Equal("UnhealthyCheck", actual.Key);
                    Assert.Equal(HealthStatus.Unhealthy, actual.Value.Status);
                    Assert.Same(exception, actual.Value.Exception);
                    Assert.Empty(actual.Value.Data);
                    A.Equals((int)HttpStatusCode.ServiceUnavailable, statusResult.StatusCode);
                });

            controller.Dispose();
        }
    }
}

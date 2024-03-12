using DFC.App.Pages.ViewModels;
using FakeItEasy;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
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
        //TODO: To fix once Redis and GraphQl ping has been implemented
        /*[Fact]
        public async Task HealthControllerHealthReturnsSuccessWhenHealthy()
        {
            // Arrange
            bool expectedResult = true;
            var controller = BuildHealthController(MediaTypeNames.Application.Json);

            A.CallTo(() => FakeContentPageService.PingAsync()).Returns(expectedResult);

            // Act
            var result = await controller.Health().ConfigureAwait(false);

            // Assert
            A.CallTo(() => FakeContentPageService.PingAsync()).MustHaveHappenedOnceExactly();

            var jsonResult = Assert.IsType<OkObjectResult>(result);
            var models = Assert.IsAssignableFrom<List<HealthItemViewModel>>(jsonResult.Value);

            models.Count.Should().BeGreaterThan(0);
            models.First().Service.Should().NotBeNullOrWhiteSpace();
            models.First().Message.Should().NotBeNullOrWhiteSpace();

            controller.Dispose();
        }*/

        //TODO: To fix once Redis and GraphQl ping has been implemented
        /*[Fact]
        public async Task HealthControllerHealthReturnsServiceUnavailableWhenUnhealthy()
        {
            // Arrange
            bool expectedResult = false;
            var controller = BuildHealthController(MediaTypeNames.Application.Json);

            A.CallTo(() => FakeContentPageService.PingAsync()).Returns(expectedResult);

            // Act
            var result = await controller.Health().ConfigureAwait(false);

            // Assert
            A.CallTo(() => FakeContentPageService.PingAsync()).MustHaveHappenedOnceExactly();

            var statusResult = Assert.IsType<StatusCodeResult>(result);

            A.Equals((int)HttpStatusCode.ServiceUnavailable, statusResult.StatusCode);

            controller.Dispose();
        }*/

        //TODO: To fix once Redis and GraphQl ping has been implemented
        /*[Fact]
        public async Task HealthControllerHealthReturnsServiceUnavailableWhenException()
        {
            // Arrange
            var controller = BuildHealthController(MediaTypeNames.Application.Json);

            A.CallTo(() => FakeContentPageService.PingAsync()).Throws<Exception>();

            // Act
            var result = await controller.Health().ConfigureAwait(false);

            // Assert
            A.CallTo(() => FakeContentPageService.PingAsync()).MustHaveHappenedOnceExactly();

            var statusResult = Assert.IsType<StatusCodeResult>(result);

            A.Equals((int)HttpStatusCode.ServiceUnavailable, statusResult.StatusCode);

            controller.Dispose();
        }*/
    }
}

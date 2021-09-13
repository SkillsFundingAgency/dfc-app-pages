using FakeItEasy;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Mime;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace DFC.App.Pages.IntegrationTests.ControllerTests.HealthControllerTests
{
    [Trait("Category", "Integration")]
    public class HealthControllerRouteTests : IClassFixture<CustomWebApplicationFactory<Startup>>
    {
        private readonly CustomWebApplicationFactory<Startup> factory;
        private readonly HttpClient httpClient;

        public HealthControllerRouteTests(CustomWebApplicationFactory<Startup> factory)
        {
            this.factory = factory;
            this.httpClient = this.factory.CreateClient();
        }

        public static IEnumerable<object[]> HealthContentRouteData => new List<object[]>
        {
            new object[] { "/health" },
        };

        public static IEnumerable<object[]> HealthOkRouteData => new List<object[]>
        {
            new object[] { "/health/ping" },
        };

        [Theory]
        [MemberData(nameof(HealthContentRouteData))]
        public async Task GetHealthHtmlContentEndpointsReturnSuccessAndCorrectContentType(string url)
        {
            // Arrange
            var uri = new Uri(url, UriKind.Relative);
            httpClient.DefaultRequestHeaders.Accept.Clear();
            httpClient.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue(MediaTypeNames.Text.Html));
            A.CallTo(() => factory.MockContentPageService.PingAsync()).Returns(true);

            // Act
            var response = await httpClient.GetAsync(uri);

            // Assert
            response.EnsureSuccessStatusCode();
            Assert.Equal($"{MediaTypeNames.Text.Html}; charset={Encoding.UTF8.WebName}", response.Content.Headers.ContentType.ToString());
        }

        [Theory]
        [MemberData(nameof(HealthContentRouteData))]
        public async Task GetHealthJsonContentEndpointsReturnSuccessAndCorrectContentType(string url)
        {
            // Arrange
            var uri = new Uri(url, UriKind.Relative);
            httpClient.DefaultRequestHeaders.Accept.Clear();
            httpClient.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue(MediaTypeNames.Application.Json));
            A.CallTo(() => factory.MockContentPageService.PingAsync()).Returns(true);

            // Act
            var response = await httpClient.GetAsync(uri);

            // Assert
            response.EnsureSuccessStatusCode();
            Assert.Equal($"{MediaTypeNames.Application.Json}; charset={Encoding.UTF8.WebName}", response.Content.Headers.ContentType.ToString());
        }

        [Theory]
        [MemberData(nameof(HealthOkRouteData))]
        public async Task GetHealthOkEndpointsReturnSuccess(string url)
        {
            // Arrange
            var uri = new Uri(url, UriKind.Relative);
            httpClient.DefaultRequestHeaders.Accept.Clear();

            // Act
            var response = await httpClient.GetAsync(uri);

            // Assert
            response.EnsureSuccessStatusCode();
            Assert.Equal(System.Net.HttpStatusCode.OK, response.StatusCode);
        }
    }
}
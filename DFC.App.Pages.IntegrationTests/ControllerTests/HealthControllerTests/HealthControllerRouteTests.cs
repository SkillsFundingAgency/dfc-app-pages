using System;
using System.Collections.Generic;
using System.Net.Mime;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace DFC.App.Pages.IntegrationTests.ControllerTests.HealthControllerTests
{
    [Trait("Category", "Integration")]
    public class HealthControllerRouteTests : IClassFixture<CustomWebApplicationFactory<DFC.App.Pages.Startup>>
    {
        private readonly CustomWebApplicationFactory<DFC.App.Pages.Startup> factory;

        public HealthControllerRouteTests(CustomWebApplicationFactory<DFC.App.Pages.Startup> factory)
        {
            this.factory = factory;

            DataSeeding.SeedDefaultArticles(factory);
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
            var client = factory.CreateClient();
            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue(MediaTypeNames.Text.Html));

            // Act
            var response = await client.GetAsync(uri).ConfigureAwait(false);

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
            var client = factory.CreateClient();
            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue(MediaTypeNames.Application.Json));

            // Act
            var response = await client.GetAsync(uri).ConfigureAwait(false);

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
            var client = factory.CreateClient();
            client.DefaultRequestHeaders.Accept.Clear();

            // Act
            var response = await client.GetAsync(uri).ConfigureAwait(false);

            // Assert
            response.EnsureSuccessStatusCode();
            Assert.Equal(System.Net.HttpStatusCode.OK, response.StatusCode);
        }
    }
}
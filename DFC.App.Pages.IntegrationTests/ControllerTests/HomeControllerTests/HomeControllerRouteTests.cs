using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Mime;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace DFC.App.Pages.IntegrationTests.ControllerTests.HomeControllerTests
{
    [Trait("Category", "Integration")]
    public class HomeControllerRouteTests : IClassFixture<CustomWebApplicationFactory<Startup>>
    {
        private readonly CustomWebApplicationFactory<Startup> factory;
        private readonly HttpClient httpClient;

        public HomeControllerRouteTests(CustomWebApplicationFactory<Startup> factory)
        {
            this.factory = factory;
            this.httpClient = this.factory.CreateClient();
        }

        public static IEnumerable<object[]> HomeContentRouteData => new List<object[]>
        {
            new object[] { "/Home/Error" },
        };

        [Theory]
        [MemberData(nameof(HomeContentRouteData))]
        public async Task GetHomeHtmlContentEndpointsReturnSuccessAndCorrectContentType(string url)
        {
            // Arrange
            var uri = new Uri(url, UriKind.Relative);
            httpClient.DefaultRequestHeaders.Accept.Clear();

            // Act
            var response = await httpClient.GetAsync(uri);

            // Assert
            response.EnsureSuccessStatusCode();
            Assert.Equal($"{MediaTypeNames.Text.Html}; charset={Encoding.UTF8.WebName}", response.Content.Headers.ContentType.ToString());
        }
    }
}
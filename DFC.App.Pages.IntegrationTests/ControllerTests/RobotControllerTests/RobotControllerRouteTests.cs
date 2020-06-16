using System;
using System.Collections.Generic;
using System.Net.Mime;
using System.Threading.Tasks;
using Xunit;

namespace DFC.App.Pages.IntegrationTests.ControllerTests.RobotControllerTests
{
    [Trait("Category", "Integration")]
    public class RobotControllerRouteTests : IClassFixture<CustomWebApplicationFactory<DFC.App.Pages.Startup>>
    {
        private readonly CustomWebApplicationFactory<DFC.App.Pages.Startup> factory;

        public RobotControllerRouteTests(CustomWebApplicationFactory<DFC.App.Pages.Startup> factory)
        {
            this.factory = factory;

            DataSeeding.SeedDefaultArticles(factory);
        }

        public static IEnumerable<object[]> RobotRouteData => new List<object[]>
        {
            new object[] { "/robots.txt" },
        };

        [Theory]
        [MemberData(nameof(RobotRouteData))]
        public async Task GetRobotTextContentEndpointsReturnSuccessAndCorrectContentType(string url)
        {
            // Arrange
            var uri = new Uri(url, UriKind.Relative);
            var client = factory.CreateClient();
            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue(MediaTypeNames.Text.Plain));

            // Act
            var response = await client.GetAsync(uri).ConfigureAwait(false);

            // Assert
            response.EnsureSuccessStatusCode();
            Assert.Equal(MediaTypeNames.Text.Plain, response.Content.Headers.ContentType.ToString());
        }
    }
}
using FluentAssertions;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Mime;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace DFC.App.Pages.IntegrationTests.ControllerTests.PagesControllerTests
{
    [Trait("Category", "Integration")]
    public class PagesControllerRouteTests : IClassFixture<CustomWebApplicationFactory<DFC.App.Pages.Startup>>
    {
        private readonly CustomWebApplicationFactory<DFC.App.Pages.Startup> factory;

        public PagesControllerRouteTests(CustomWebApplicationFactory<DFC.App.Pages.Startup> factory)
        {
            this.factory = factory;

            DataSeeding.SeedDefaultArticles(factory);
        }

        public static IEnumerable<object[]> PagesContentRouteData => new List<object[]>
        {
            new object[] { "/" },
            new object[] { "/pages" },
            new object[] { $"/pages/htmlhead" },
            new object[] { $"/pages/breadcrumb" },
            new object[] { $"/pages/body" },
            new object[] { $"/pages/{DataSeeding.DefaultLocationName}/document" },
            new object[] { $"/pages/{DataSeeding.DefaultLocationName}/htmlhead" },
            new object[] { $"/pages/{DataSeeding.DefaultLocationName}/breadcrumb" },
            new object[] { $"/pages/{DataSeeding.DefaultLocationName}/body" },
            new object[] { $"/pages/{DataSeeding.DefaultLocationName}/{DataSeeding.ExampleArticleName}/document" },
            new object[] { $"/pages/{DataSeeding.DefaultLocationName}/{DataSeeding.ExampleArticleName}/htmlhead" },
            new object[] { $"/pages/{DataSeeding.DefaultLocationName}/{DataSeeding.ExampleArticleName}/breadcrumb" },
            new object[] { $"/pages/{DataSeeding.DefaultLocationName}/{DataSeeding.ExampleArticleName}/body" },
            new object[] { "/pages/health" },
        };

        public static IEnumerable<object[]> PagesNoContentRouteData => new List<object[]>
        {
            new object[] { $"/pages/bodytop" },
            new object[] { $"/pages/herobanner" },
            new object[] { $"/pages/sidebarright" },
            new object[] { $"/pages/sidebarleft" },
            new object[] { $"/pages/bodyfooter" },
            new object[] { $"/pages/{DataSeeding.DefaultLocationName}/bodytop" },
            new object[] { $"/pages/{DataSeeding.DefaultLocationName}/herobanner" },
            new object[] { $"/pages/{DataSeeding.DefaultLocationName}/sidebarright" },
            new object[] { $"/pages/{DataSeeding.DefaultLocationName}/sidebarleft" },
            new object[] { $"/pages/{DataSeeding.DefaultLocationName}/bodyfooter" },
            new object[] { $"/pages/{DataSeeding.DefaultLocationName}/{DataSeeding.ExampleArticleName}/bodytop" },
            new object[] { $"/pages/{DataSeeding.DefaultLocationName}/{DataSeeding.ExampleArticleName}/herobanner" },
            new object[] { $"/pages/{DataSeeding.DefaultLocationName}/{DataSeeding.ExampleArticleName}/sidebarright" },
            new object[] { $"/pages/{DataSeeding.DefaultLocationName}/{DataSeeding.ExampleArticleName}/sidebarleft" },
            new object[] { $"/pages/{DataSeeding.DefaultLocationName}/{DataSeeding.ExampleArticleName}/bodyfooter" },
            new object[] { $"/pages/{DataSeeding.DefaultLocationName}/{DataSeeding.AlternativeArticleName}/bodytop" },
            new object[] { $"/pages/{DataSeeding.DefaultLocationName}/{DataSeeding.AlternativeArticleName}/herobanner" },
            new object[] { $"/pages/{DataSeeding.DefaultLocationName}/{DataSeeding.AlternativeArticleName}/sidebarright" },
            new object[] { $"/pages/{DataSeeding.DefaultLocationName}/{DataSeeding.AlternativeArticleName}/sidebarleft" },
            new object[] { $"/pages/{DataSeeding.DefaultLocationName}/{DataSeeding.AlternativeArticleName}/bodyfooter" },
        };

        [Theory]
        [MemberData(nameof(PagesContentRouteData))]
        public async Task GetPagesHtmlContentEndpointsReturnSuccessAndCorrectContentType(string url)
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
        [MemberData(nameof(PagesContentRouteData))]
        public async Task GetPagesJsonContentEndpointsReturnSuccessAndCorrectContentType(string url)
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
        [MemberData(nameof(PagesNoContentRouteData))]
        public async Task GetPagesEndpointsReturnSuccessAndNoContent(string url)
        {
            // Arrange
            var uri = new Uri(url, UriKind.Relative);
            var client = factory.CreateClient();
            client.DefaultRequestHeaders.Accept.Clear();

            // Act
            var response = await client.GetAsync(uri).ConfigureAwait(false);

            // Assert
            response.EnsureSuccessStatusCode();
            response.StatusCode.Should().Be(HttpStatusCode.NoContent);
        }
    }
}
using DFC.Common.SharedContent.Pkg.Netcore.Interfaces;
using DFC.Common.SharedContent.Pkg.Netcore.Model.Common;
using DFC.Common.SharedContent.Pkg.Netcore.Model.ContentItems;
using DFC.Common.SharedContent.Pkg.Netcore.Model.Response;
using FakeItEasy;
using FluentAssertions;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Mime;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace DFC.App.Pages.IntegrationTests.ControllerTests.PagesControllerTests
{
    [Trait("Category", "Integration")]
    public class PagesControllerRouteTests : IClassFixture<CustomWebApplicationFactory<Startup>>
    {
        private readonly CustomWebApplicationFactory<Startup> factory;
        private readonly HttpClient httpClient;

        public PagesControllerRouteTests(CustomWebApplicationFactory<Startup> factory)
        {
            this.factory = factory;
            this.httpClient = this.factory.CreateClient();
        }

        public static IEnumerable<object[]> PagesContentRouteData => new List<object[]>
        {
            new object[] { "/" },
            new object[] { "/pages" },
            new object[] { "/pages/head" },
            new object[] { "/pages/breadcrumb" },
            new object[] { $"/pages/herobanner" },
        };

        public static IEnumerable<object[]> PagesNoContentRouteData => new List<object[]>
        {
            new object[] { $"/pages/bodytop" },
            new object[] { $"/pages/sidebarright" },
            new object[] { $"/pages/sidebarleft" },
            new object[] { $"/pages/bodyfooter" },
        };

        [Theory]
        [MemberData(nameof(PagesContentRouteData))]
        public async Task GetPagesHtmlContentEndpointsReturnSuccessAndCorrectContentType(string url)
        {
            var page = new Page()
            {
                Description = "test",
                DisplayText = "test",
            };
            var pageUrl = new PageUrl()
            {
                DisplayText = "test",
                PageLocation = new PageLocation()
                {
                    FullUrl = "test",
                    UrlName = "test",
                },
            };
            var pageUrlResponse = new PageUrlResponse()
            {
                Page = new List<PageUrl> { pageUrl },
            };
            this.factory.MockSharedContentRedis.Setup(
                x => x.GetDataAsync<PageUrlResponse>(
                    It.IsAny<string>(), "PUBLISHED", 4))
            .ReturnsAsync(pageUrlResponse);

            // Arrange
            var uri = new Uri(url, UriKind.Relative);
            httpClient.DefaultRequestHeaders.Accept.Clear();
            httpClient.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue(MediaTypeNames.Text.Html));

            // Act
            var response = await httpClient.GetAsync(uri);

            // Assert
            response.EnsureSuccessStatusCode();

            if (response.StatusCode != HttpStatusCode.NoContent)
            {
                Assert.Equal($"{MediaTypeNames.Text.Html}; charset={Encoding.UTF8.WebName}", response.Content.Headers.ContentType.ToString());
            }
        }

        [Theory]
        [MemberData(nameof(PagesContentRouteData))]
        public async Task GetPagesJsonContentEndpointsReturnSuccessAndCorrectContentType(string url)
        {
            // Arrange
            var pageUrl = new PageUrl()
            {
                DisplayText = "test",
                PageLocation = new PageLocation()
                {
                    FullUrl = "test",
                    UrlName = "test",
                },
            };
            var pageUrlResponse = new PageUrlResponse()
            {
                Page = new List<PageUrl> { pageUrl },
            };
            this.factory.MockSharedContentRedis.Setup(
                x => x.GetDataAsync<PageUrlResponse>(
                    It.IsAny<string>(), "PUBLISHED", 4))
            .ReturnsAsync(pageUrlResponse);
            var uri = new Uri(url, UriKind.Relative);
            httpClient.DefaultRequestHeaders.Accept.Clear();
            httpClient.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue(MediaTypeNames.Application.Json));

            // Act
            var response = await httpClient.GetAsync(uri);

            // Assert
            response.EnsureSuccessStatusCode();

            if (response.StatusCode != HttpStatusCode.NoContent)
            {
                Assert.Equal($"{MediaTypeNames.Application.Json}; charset={Encoding.UTF8.WebName}", response.Content.Headers.ContentType.ToString());
            }
        }

        [Theory]
        [MemberData(nameof(PagesNoContentRouteData))]
        public async Task GetPagesEndpointsReturnSuccessAndNoContent(string url)
        {
            // Arrange
            var uri = new Uri(url, UriKind.Relative);
            httpClient.DefaultRequestHeaders.Accept.Clear();
            var RedisContentMock = new Mock<ISharedContentRedisInterface>();
            RedisContentMock.Setup(m => m.GetDataAsync<PageUrlResponse>("PagesIntegration", "PUBKUSHED",4)).ReturnsAsync((PageUrlResponse)null);

            // Act
            var response = await httpClient.GetAsync(uri);

            // Assert
            response.EnsureSuccessStatusCode();
            response.StatusCode.Should().Be(HttpStatusCode.NoContent);
        }
    }
}
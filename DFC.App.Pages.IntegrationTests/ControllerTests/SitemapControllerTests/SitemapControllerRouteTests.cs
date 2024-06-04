using DFC.App.Pages.Data.Models;
using DFC.Common.SharedContent.Pkg.Netcore.Model.ContentItems.Sitemap;
using DFC.Common.SharedContent.Pkg.Netcore.Model.Response;
using FakeItEasy;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Net.Http;
using System.Net.Mime;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace DFC.App.Pages.IntegrationTests.ControllerTests.SitemapControllerTests
{
    [Trait("Category", "Integration")]
    public class SitemapControllerRouteTests : IClassFixture<CustomWebApplicationFactory<Startup>>
    {
        private readonly CustomWebApplicationFactory<Startup> factory;
        private readonly HttpClient httpClient;

        public SitemapControllerRouteTests(CustomWebApplicationFactory<Startup> factory)
        {
            this.factory = factory;
            this.httpClient = this.factory.CreateClient();
        }

        public static IEnumerable<object[]> SitemapRouteData => new List<object[]>
        {
            new object[] { $"/sitemap.xml" },
            new object[] { $"/pages/sitemap" },
        };

        [Theory]
        [MemberData(nameof(SitemapRouteData))]
        public async Task GetSitemapXmlContentEndpointsReturnSuccessAndCorrectContentType(string url)
        {
            //Arrange
            var sitemaps = new PageSitemapModel()
            {
                PageLocation = new PageLocation()
                {
                    DefaultPageForLocation = false,
                    FullUrl = "/test/test",
                    urlName = "test",
                },
                Sitemap = new SitemapModel()
                {
                    ChangeFrequency = "DAILY",
                    Priority = 5,
                    Exclude = false,
                },
            };

            var sitemapResponse = new SitemapResponse()
            {
                Page = new List<PageSitemapModel> { sitemaps },
            };

            this.factory.MockSharedContentRedis.Setup(
                x => x.GetDataAsync<SitemapResponse>(
                    It.IsAny<string>(), "PUBLISHED",4))
                .ReturnsAsync(sitemapResponse);

            var uri = new Uri(url, UriKind.Relative);
            httpClient.DefaultRequestHeaders.Accept.Clear();
            httpClient.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue(MediaTypeNames.Text.Html));

            // Act
            var response = await httpClient.GetAsync(uri);

            // Assert
            response.EnsureSuccessStatusCode();

            if (response.StatusCode != System.Net.HttpStatusCode.NoContent)
            {
                Assert.Equal($"{MediaTypeNames.Application.Xml}", response.Content.Headers.ContentType.ToString());
            }
        }
    }
}
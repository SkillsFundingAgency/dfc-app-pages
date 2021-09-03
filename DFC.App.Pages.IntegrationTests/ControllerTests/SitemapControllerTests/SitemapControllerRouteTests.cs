using DFC.App.Pages.Data.Models;
using FakeItEasy;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Net.Http;
using System.Net.Mime;
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
            // Arrange
            var contentPageModel = factory.GetContentPageModels().Where(x => x.CanonicalName == "an-article");
            var uri = new Uri(url, UriKind.Relative);
            httpClient.DefaultRequestHeaders.Accept.Clear();
            httpClient.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue(MediaTypeNames.Application.Xml));
            A.CallTo(() => factory.MockCosmosRepo.GetAllAsync(A<string>.Ignored)).Returns(factory.GetContentPageModels());
            A.CallTo(() => factory.MockCosmosRepo.GetAsync(A<Expression<Func<ContentPageModel, bool>>>.Ignored)).Returns(contentPageModel);

            // Act
            var response = await httpClient.GetAsync(uri).ConfigureAwait(false);

            // Assert
            response.EnsureSuccessStatusCode();
        }
    }
}
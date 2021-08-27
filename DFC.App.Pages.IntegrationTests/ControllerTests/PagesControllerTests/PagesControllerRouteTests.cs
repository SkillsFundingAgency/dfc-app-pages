using DFC.App.Pages.Data.Models;
using FakeItEasy;
using FluentAssertions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Net;
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

        public PagesControllerRouteTests(CustomWebApplicationFactory<Startup> factory)
        {
            this.factory = factory;
        }

        public static IEnumerable<object[]> PagesContentRouteData => new List<object[]>
        {
            new object[] { "/" },
            new object[] { "/pages" },
            new object[] { "/pages/htmlhead" },
            new object[] { "/pages/breadcrumb" },
            new object[] { "/pages/body" },
        };

        public static IEnumerable<object[]> PagesNoContentRouteData => new List<object[]>
        {
            new object[] { $"/pages/bodytop" },
            new object[] { $"/pages/herobanner" },
            new object[] { $"/pages/sidebarright" },
            new object[] { $"/pages/sidebarleft" },
            new object[] { $"/pages/bodyfooter" },
        };

        [Theory]
        [MemberData(nameof(PagesContentRouteData))]
        public async Task GetPagesHtmlContentEndpointsReturnSuccessAndCorrectContentType(string url)
        {
            // Arrange
            var contentPageModel = factory.GetContentPageModels().Where(x => x.CanonicalName == "an-article");
            var uri = new Uri(url, UriKind.Relative);
            var client = factory.CreateClientWithWebHostBuilder();
            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue(MediaTypeNames.Text.Html));
            A.CallTo(() => factory.MockCosmosRepo.GetAllAsync(A<string>.Ignored)).Returns(factory.GetContentPageModels());
            A.CallTo(() => factory.MockCosmosRepo.GetAsync(A<Expression<Func<ContentPageModel, bool>>>.Ignored)).Returns(contentPageModel);

            // Act
            var response = await client.GetAsync(uri).ConfigureAwait(false);

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
            var contentPageModel = factory.GetContentPageModels().Where(x => x.CanonicalName == "an-article");
            var uri = new Uri(url, UriKind.Relative);
            var client = factory.CreateClientWithWebHostBuilder();
            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue(MediaTypeNames.Application.Json));
            A.CallTo(() => factory.MockCosmosRepo.GetAllAsync(A<string>.Ignored)).Returns(factory.GetContentPageModels());
            A.CallTo(() => factory.MockCosmosRepo.GetAsync(A<Expression<Func<ContentPageModel, bool>>>.Ignored)).Returns(contentPageModel);

            // Act
            var response = await client.GetAsync(uri).ConfigureAwait(false);

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
            List<ContentPageModel>? contentPageModel = null;
            var uri = new Uri(url, UriKind.Relative);
            var client = factory.CreateClientWithWebHostBuilder();
            client.DefaultRequestHeaders.Accept.Clear();
            A.CallTo(() => factory.MockCosmosRepo.GetAsync(A<Expression<Func<ContentPageModel, bool>>>.Ignored)).Returns(contentPageModel);

            // Act
            var response = await client.GetAsync(uri).ConfigureAwait(false);

            // Assert
            response.EnsureSuccessStatusCode();
            response.StatusCode.Should().Be(HttpStatusCode.NoContent);
        }
    }
}
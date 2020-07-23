using DFC.App.Pages.Data.Models;
using FakeItEasy;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using System.Net.Mime;
using System.Threading.Tasks;
using Xunit;

namespace DFC.App.Pages.UnitTests.ControllerTests.SitemapControllerTests
{
    [Trait("Category", "Sitemap Controller Unit Tests")]
    public class SitemapControllerSitemapTests : BaseSitemapControllerTests
    {
        [Fact]
        public async Task SitemapControllerSitemapReturnsSuccess()
        {
            // Arrange
            const int resultsCount = 5;
            var expectedResults = A.CollectionOfFake<ContentPageModel>(resultsCount);
            var controller = BuildSitemapController();

            expectedResults[0].IncludeInSitemap = true;
            expectedResults[0].PageLocation = "/default-location";
            expectedResults[0].CanonicalName = "default-article";
            expectedResults[0].IsDefaultForPageLocation = true;
            expectedResults[1].IncludeInSitemap = false;
            expectedResults[1].PageLocation = "/default-location";
            expectedResults[1].CanonicalName = "not-in-sitemap";
            expectedResults[2].IncludeInSitemap = true;
            expectedResults[2].PageLocation = "/default-location";
            expectedResults[2].CanonicalName = "in-sitemap";
            expectedResults[3].IncludeInSitemap = true;
            expectedResults[3].PageLocation = "/";
            expectedResults[3].CanonicalName = "slash-location";
            expectedResults[4].IncludeInSitemap = true;
            expectedResults[4].PageLocation = string.Empty;
            expectedResults[4].CanonicalName = "empty-location";

            A.CallTo(() => FakeContentPageService.GetAllAsync()).Returns(expectedResults);

            // Act
            var result = await controller.Sitemap().ConfigureAwait(false);

            // Assert
            A.CallTo(() => FakeContentPageService.GetAllAsync()).MustHaveHappenedOnceExactly();

            var contentResult = Assert.IsType<ContentResult>(result);

            contentResult.ContentType.Should().Be(MediaTypeNames.Application.Xml);

            controller.Dispose();
        }

        [Fact]
        public async Task SitemapControllerSitemapReturnsSuccessWhenNoData()
        {
            // Arrange
            const int resultsCount = 0;
            var expectedResults = A.CollectionOfFake<ContentPageModel>(resultsCount);
            var controller = BuildSitemapController();

            A.CallTo(() => FakeContentPageService.GetAllAsync()).Returns(expectedResults);

            // Act
            var result = await controller.Sitemap().ConfigureAwait(false);

            // Assert
            A.CallTo(() => FakeContentPageService.GetAllAsync()).MustHaveHappenedOnceExactly();

            _ = Assert.IsType<NoContentResult>(result);

            controller.Dispose();
        }
    }
}

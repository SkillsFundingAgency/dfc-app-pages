using DFC.App.Pages.Cms.Data.Content;
using DFC.App.Pages.Controllers;
using DFC.Common.SharedContent.Pkg.Netcore.Interfaces;
using DFC.Common.SharedContent.Pkg.Netcore.Model.ContentItems.Sitemap;
using DFC.Common.SharedContent.Pkg.Netcore.Model.Response;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;
using System.Collections.Generic;
using System.Net.Mime;
using System.Threading.Tasks;
using Xunit;

namespace DFC.App.Pages.UnitTests.ControllerTests.SitemapControllerTests
{
    [Trait("Category", "Sitemap Controller Unit Tests")]
    public class SitemapControllerViewTests : BaseSitemapControllerTests
    {
        [Fact]
        public async Task SitemapControllerViewReturnsSuccess()
        {
            var loggerMock = new Mock<ILogger<SitemapController>>();

            var configurationMock = new Mock<IConfiguration>();
            var requestMock = new Mock<HttpRequest>();
            var httpContextMock = new Mock<HttpContext>();
            httpContextMock.Setup(c => c.Request).Returns(requestMock.Object);

            var mockIConfigurationSection = new Mock<IConfigurationSection>();
            mockIConfigurationSection.Setup(x => x.Key).Returns("Cms:Expiry");
            mockIConfigurationSection.Setup(x => x.Value).Returns("4");
            configurationMock.Setup(r => r.GetSection("Cms:Expiry")).Returns(mockIConfigurationSection.Object);
            var settings = new contentModeOptions()
            {
                contentMode = "contentMode",
                value = "PUBLISHED",
            };
            var monitor = Mock.Of<IOptionsMonitor<ContentModeOptions>>(x => x.CurrentValue == settings);

            var sharedContentRedisMock = new Mock<ISharedContentRedisInterface>();
            var pageSitemapResponse = new SitemapResponse
            {
                Page = new List<PageSitemapModel>
                {
                    new ()
                    {
                        Sitemap = new ()
                        {
                            ChangeFrequency = "DAILY",
                            Priority = 5,
                            Exclude = false,
                        },
                        PageLocation = new ()
                        {
                            DefaultPageForLocation = false,
                            FullUrl = "/test/test",
                            urlName = "test",
                        },
                    },
                },
            };

            sharedContentRedisMock.Setup(m => m.GetDataAsyncWithExpiry<SitemapResponse>("SitemapPages/ALL", monitor.CurrentValue.contentMode, 4)).ReturnsAsync(pageSitemapResponse);

            var controller = new SitemapController(configurationMock.Object, loggerMock.Object, sharedContentRedisMock.Object, monitor);

            // Act
            var result = await controller.Sitemap();

            // Assert
            Assert.IsType<ContentResult>(result);
            var contentResult = Assert.IsType<ContentResult>(result);
            contentResult.ContentType.Should().Be(MediaTypeNames.Application.Xml);
            Assert.NotNull(contentResult.Content);
        }

        [Fact]
        public async Task SitemapControllerViewReturnsSuccessWhenNoData()
        {
            //Arrange
            var loggerMock = new Mock<ILogger<SitemapController>>();
            var requestMock = new Mock<HttpRequest>();
            var configurationMock = new Mock<IConfiguration>();

            var mockIConfigurationSection = new Mock<IConfigurationSection>();
            mockIConfigurationSection.Setup(x => x.Key).Returns("Cms:Expiry");
            mockIConfigurationSection.Setup(x => x.Value).Returns("4");
            configurationMock.Setup(r => r.GetSection("Cms:Expiry")).Returns(mockIConfigurationSection.Object);
            var settings = new contentModeOptions()
            {
                contentMode = "contentMode",
                value = "PUBLISHED",
            };
            var monitor = Mock.Of<IOptionsMonitor<ContentModeOptions>>(x => x.CurrentValue == settings);

            requestMock.Setup(r => r.Scheme).Returns("https");
            requestMock.Setup(r => r.Host).Returns(new HostString("example.com"));
            var httpContextMock = new Mock<HttpContext>();
            httpContextMock.Setup(c => c.Request).Returns(requestMock.Object);

            var sharedContentRedisMock = new Mock<ISharedContentRedisInterface>();
            sharedContentRedisMock.Setup(m => m.GetDataAsyncWithExpiry<SitemapResponse>("PagesSitemap/All", "PUBLISHED", 4)).ReturnsAsync((SitemapResponse)null);
            var controller = new SitemapController(configurationMock.Object, loggerMock.Object, sharedContentRedisMock.Object, monitor);

            //Act
            var result = await controller.SitemapView().ConfigureAwait(false);

            //Assert
            Assert.IsType<NoContentResult>(result);
        }
    }
}

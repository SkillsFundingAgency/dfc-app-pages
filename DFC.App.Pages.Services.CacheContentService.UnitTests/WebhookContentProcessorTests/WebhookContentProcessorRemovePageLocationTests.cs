
using DFC.App.Pages.Data.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace DFC.App.Pages.Services.CacheContentService.UnitTests.WebhookContentProcessorTests
{
    [Trait("Category", "WebhookContentProcessor - RemovePageLocation Unit Tests")]
    public class WebhookContentProcessorRemovePageLocationTests : BaseWebhookContentProcessor
    {
        [Fact]
        public void WebhooksServiceRemovePageLocationTestsReturnsSuccess()
        {
            // Arrange
            var pageLocationId = Guid.NewGuid();
            var expectedPageLocationModel = new PageLocationModel
            {
                ItemId = pageLocationId,
            };
            var items = BuildPageLocationSet();
            var service = BuildWebhookContentProcessor();

            items.First().PageLocations.First().PageLocations!.Add(expectedPageLocationModel);

            // Act
            var result = service.RemovePageLocation(pageLocationId, items);

            // Assert
            Assert.True(result);
        }

        [Fact]
        public void WebhooksServiceRemovePageLocationTestsReturnsFalseforNotFound()
        {
            // Arrange
            var pageLocationId = Guid.NewGuid();
            var items = BuildPageLocationSet();
            var service = BuildWebhookContentProcessor();

            // Act
            var result = service.RemovePageLocation(pageLocationId, items);

            // Assert
            Assert.False(result);
        }

        [Fact]
        public void WebhooksServiceRemovePageLocationTestsReturnsFalseForNullPageLocations()
        {
            // Arrange
            var pageLocationId = Guid.NewGuid();
            List<PageLocationModel>? items = null;
            var service = BuildWebhookContentProcessor();

            // Act
            var result = service.RemovePageLocation(pageLocationId, items);

            // Assert
            Assert.False(result);
        }

        [Fact]
        public void WebhooksServiceRemovePageLocationTestsReturnsFalseForNoPageLocations()
        {
            // Arrange
            var pageLocationId = Guid.NewGuid();
            var items = new List<PageLocationModel>();
            var service = BuildWebhookContentProcessor();

            // Act
            var result = service.RemovePageLocation(pageLocationId, items);

            // Assert
            Assert.False(result);
        }

        private List<PageLocationModel> BuildPageLocationSet()
        {
            var items = new List<PageLocationModel>
            {
                new PageLocationModel
                {
                    ItemId = Guid.NewGuid(),
                    PageLocations = new List<PageLocationModel>
                    {
                        new PageLocationModel
                        {
                            ItemId = Guid.NewGuid(),
                            PageLocations = new List<PageLocationModel>
                            {
                                new PageLocationModel
                                {
                                    ItemId = Guid.NewGuid(),
                                },
                            },
                        },
                    },
                },
            };

            return items;
        }
    }
}

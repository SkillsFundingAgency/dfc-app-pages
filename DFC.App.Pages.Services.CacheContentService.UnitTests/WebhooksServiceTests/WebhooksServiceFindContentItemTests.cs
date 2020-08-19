using DFC.App.Pages.Data.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace DFC.App.Pages.Services.CacheContentService.UnitTests.WebhooksServiceTests
{
    [Trait("Category", "Webhooks Service FindContentItems Unit Tests")]
    public class WebhooksServiceFindContentItemTests : BaseWebhooksServiceTests
    {
        [Fact]
        public void WebhooksServiceFindContentItemTestsReturnsSuccess()
        {
            // Arrange
            var contentItemId = Guid.NewGuid();
            var expectedContentItemModel = new ContentItemModel
            {
                ItemId = contentItemId,
            };
            var items = BuildContentItemSet();
            var service = BuildWebhooksService();

            items.First().ContentItems.First().ContentItems.Add(expectedContentItemModel);

            // Act
            var result = service.FindContentItem(contentItemId, items);

            // Assert
            Assert.Equal(expectedContentItemModel.ItemId, result.ItemId);
        }

        [Fact]
        public void WebhooksServiceFindContentItemTestsReturnsNullforNotFound()
        {
            // Arrange
            var contentItemId = Guid.NewGuid();
            var items = BuildContentItemSet();
            var service = BuildWebhooksService();

            // Act
            var result = service.FindContentItem(contentItemId, items);

            // Assert
            Assert.Null(result);
        }

        [Fact]
        public void WebhooksServiceFindContentItemTestsReturnsNullForNullContentItems()
        {
            // Arrange
            var contentItemId = Guid.NewGuid();
            List<ContentItemModel>? items = null;
            var service = BuildWebhooksService();

            // Act
            var result = service.FindContentItem(contentItemId, items);

            // Assert
            Assert.Null(result);
        }

        [Fact]
        public void WebhooksServiceFindContentItemTestsReturnsNullForNNoContentItems()
        {
            // Arrange
            var contentItemId = Guid.NewGuid();
            var items = new List<ContentItemModel>();
            var service = BuildWebhooksService();

            // Act
            var result = service.FindContentItem(contentItemId, items);

            // Assert
            Assert.Null(result);
        }

        private List<ContentItemModel> BuildContentItemSet()
        {
            var items = new List<ContentItemModel>
            {
                new ContentItemModel
                {
                    ItemId = Guid.NewGuid(),
                    ContentItems = new List<ContentItemModel>
                    {
                        new ContentItemModel
                        {
                            ItemId = Guid.NewGuid(),
                            ContentItems = new List<ContentItemModel>
                            {
                                new ContentItemModel
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

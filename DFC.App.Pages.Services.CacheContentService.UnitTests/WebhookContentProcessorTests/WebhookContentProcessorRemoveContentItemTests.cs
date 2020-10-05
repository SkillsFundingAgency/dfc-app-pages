using DFC.App.Pages.Data.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace DFC.App.Pages.Services.CacheContentService.UnitTests.WebhookContentProcessorTests
{
    [Trait("Category", "WebhookContentProcessor - RemoveContentItem Unit Tests")]
    public class WebhookContentProcessorRemoveContentItemTests : BaseWebhookContentProcessor
    {
        [Fact]
        public void WebhooksServiceRemoveContentItemTestsReturnsSuccess()
        {
            // Arrange
            var contentItemId = Guid.NewGuid();
            var expectedContentItemModel = new ContentItemModel
            {
                ItemId = contentItemId,
            };
            var items = BuildContentItemSet();
            var service = BuildWebhookContentProcessor();

            items.First().ContentItems.First().ContentItems!.Add(expectedContentItemModel);

            // Act
            var result = service.RemoveContentItem(contentItemId, items);

            // Assert
            Assert.True(result);
        }

        [Fact]
        public void WebhooksServiceRemoveContentItemTestsReturnsFalseforNotFound()
        {
            // Arrange
            var contentItemId = Guid.NewGuid();
            var items = BuildContentItemSet();
            var service = BuildWebhookContentProcessor();

            // Act
            var result = service.RemoveContentItem(contentItemId, items);

            // Assert
            Assert.False(result);
        }

        [Fact]
        public void WebhooksServiceRemoveContentItemTestsReturnsFalseForNullContentItems()
        {
            // Arrange
            var contentItemId = Guid.NewGuid();
            List<ContentItemModel>? items = null;
            var service = BuildWebhookContentProcessor();

            // Act
            var result = service.RemoveContentItem(contentItemId, items);

            // Assert
            Assert.False(result);
        }

        [Fact]
        public void WebhooksServiceRemoveContentItemTestsReturnsFalseForNoContentItems()
        {
            // Arrange
            var contentItemId = Guid.NewGuid();
            var items = new List<ContentItemModel>();
            var service = BuildWebhookContentProcessor();

            // Act
            var result = service.RemoveContentItem(contentItemId, items);

            // Assert
            Assert.False(result);
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

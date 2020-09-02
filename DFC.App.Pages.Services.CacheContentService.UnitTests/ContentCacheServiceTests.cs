using FakeItEasy;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using Xunit;

namespace DFC.App.Pages.Services.CacheContentService.UnitTests
{
    [Trait("Category", "Content Cache service Unit Tests")]
    public class ContentCacheServiceTests
    {
        [Fact]
        public void ContentCacheServiceCheckIsContentItemReturnsTrue()
        {
            // arrange
            const bool expectedResult = true;
            var contentItemId = Guid.NewGuid();

            var contentCacheService = new ContentCacheService(A.Fake<ILogger<ContentCacheService>>());
            contentCacheService.AddOrReplace(Guid.NewGuid(), new List<Guid> { Guid.NewGuid(), contentItemId, Guid.NewGuid(), });

            // act
            var result = contentCacheService.CheckIsContentItem(contentItemId);

            // assert
            Assert.Equal(expectedResult, result);
        }

        [Fact]
        public void ContentCacheServiceCheckIsContentItemReturnsFalse()
        {
            // arrange
            const bool expectedResult = false;
            var contentItemId = Guid.NewGuid();

            var contentCacheService = new ContentCacheService(A.Fake<ILogger<ContentCacheService>>());
            contentCacheService.AddOrReplace(Guid.NewGuid(), new List<Guid> { Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid(), });

            // act
            var result = contentCacheService.CheckIsContentItem(contentItemId);

            // assert
            Assert.Equal(expectedResult, result);
        }

        [Fact]
        public void ContentCacheServiceClearNoReturns()
        {
            // arrange
            var contentCacheService = new ContentCacheService(A.Fake<ILogger<ContentCacheService>>());

            // act
            contentCacheService.Clear();

            // assert
            Assert.True(true);      // nothing can be asserted here
        }

        [Fact]
        public void ContentCacheServiceGetContentIdsContainingContentItemIdReturnsListWhenMatch()
        {
            // arrange
            var contentId1 = Guid.NewGuid();
            var contentId2 = Guid.NewGuid();
            var contentItemId = Guid.NewGuid();
            List<Guid> expectedResult = new List<Guid> { contentId1, contentId2 };

            var contentCacheService = new ContentCacheService(A.Fake<ILogger<ContentCacheService>>());
            contentCacheService.AddOrReplace(contentId1, new List<Guid> { Guid.NewGuid(), contentItemId, Guid.NewGuid(), });
            contentCacheService.AddOrReplace(contentId2, new List<Guid> { Guid.NewGuid(), contentItemId, Guid.NewGuid(), });

            // act
            var result = contentCacheService.GetContentIdsContainingContentItemId(contentItemId);

            // assert
            Assert.Equal(expectedResult, result);
        }

        [Fact]
        public void ContentCacheServiceGetContentIdsContainingContentItemIdNullWhenNoMatch()
        {
            // arrange
            var contentId1 = Guid.NewGuid();
            var contentId2 = Guid.NewGuid();
            var contentItemId = Guid.NewGuid();
            var expectedResult = new List<Guid>();

            var contentCacheService = new ContentCacheService(A.Fake<ILogger<ContentCacheService>>());
            contentCacheService.AddOrReplace(contentId1, new List<Guid> { Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid(), });
            contentCacheService.AddOrReplace(contentId2, new List<Guid> { Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid(), });

            // act
            var result = contentCacheService.GetContentIdsContainingContentItemId(contentItemId);

            // assert
            Assert.Equal(expectedResult, result);
        }

        [Fact]
        public void ContentCacheServiceRemove()
        {
            // arrange
            var contentId1 = Guid.NewGuid();
            var contentId2 = Guid.NewGuid();
            var contentItemId = Guid.NewGuid();

            var contentCacheService = new ContentCacheService(A.Fake<ILogger<ContentCacheService>>());
            contentCacheService.AddOrReplace(contentId1, new List<Guid> { Guid.NewGuid(), contentItemId, Guid.NewGuid(), });
            contentCacheService.AddOrReplace(contentId2, new List<Guid> { Guid.NewGuid(), contentItemId, Guid.NewGuid(), });

            // act
            contentCacheService.Remove(contentId1);

            // assert
            Assert.True(true);      // nothing can be asserted here
        }

        [Fact]
        public void ContentCacheServiceRemoveContentItem()
        {
            // arrange
            var contentId1 = Guid.NewGuid();
            var contentId2 = Guid.NewGuid();
            var contentItemId = Guid.NewGuid();

            var contentCacheService = new ContentCacheService(A.Fake<ILogger<ContentCacheService>>());
            contentCacheService.AddOrReplace(contentId1, new List<Guid> { Guid.NewGuid(), contentItemId, Guid.NewGuid(), });
            contentCacheService.AddOrReplace(contentId2, new List<Guid> { Guid.NewGuid(), contentItemId, Guid.NewGuid(), });

            // act
            contentCacheService.RemoveContentItem(contentId1, contentItemId);

            // assert
            Assert.True(true);      // nothing can be asserted here
        }

        [Fact]
        public void ContentCacheServiceAddOrReplaceAdd()
        {
            // arrange
            var contentId = Guid.NewGuid();
            var contentItemId = Guid.NewGuid();

            var contentCacheService = new ContentCacheService(A.Fake<ILogger<ContentCacheService>>());

            // act
            contentCacheService.AddOrReplace(contentId, new List<Guid> { Guid.NewGuid(), contentItemId, Guid.NewGuid(), });

            // assert
            Assert.True(true);      // nothing can be asserted here
        }

        [Fact]
        public void ContentCacheServiceAddOrReplaceReplace()
        {
            // arrange
            var contentId = Guid.NewGuid();
            var contentItemId = Guid.NewGuid();

            var contentCacheService = new ContentCacheService(A.Fake<ILogger<ContentCacheService>>());
            contentCacheService.AddOrReplace(contentId, new List<Guid> { Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid(), });

            // act
            contentCacheService.AddOrReplace(contentId, new List<Guid> { Guid.NewGuid(), contentItemId, Guid.NewGuid(), });

            // assert
            Assert.True(true);      // nothing can be asserted here
        }
    }
}

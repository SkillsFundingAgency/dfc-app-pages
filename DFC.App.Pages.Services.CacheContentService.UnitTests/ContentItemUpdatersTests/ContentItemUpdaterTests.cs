using DFC.App.Pages.Data.Common;
using DFC.App.Pages.Data.Contracts;
using DFC.App.Pages.Data.Models;
using DFC.App.Pages.Data.Models.CmsApiModels;
using DFC.App.Pages.Services.CacheContentService.ContentItemUpdaters;
using FakeItEasy;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xunit;

namespace DFC.App.Pages.Services.CacheContentService.UnitTests.ContentItemUpdatersTests
{
    [Trait("Category", "ContentItemUpdaters ContentItemUpdaterTests Unit Tests")]
    public class ContentItemUpdaterTests
    {
        private readonly IMarkupContentItemUpdater<CmsApiHtmlModel> fakeHtmlMarkupContentItemUpdater = A.Fake<IMarkupContentItemUpdater<CmsApiHtmlModel>>();
        private readonly IMarkupContentItemUpdater<CmsApiHtmlSharedModel> fakeHtmlSharedMarkupContentItemUpdater = A.Fake<IMarkupContentItemUpdater<CmsApiHtmlSharedModel>>();
        private readonly IMarkupContentItemUpdater<CmsApiSharedContentModel> fakeSharedContentMarkupContentItemUpdater = A.Fake<IMarkupContentItemUpdater<CmsApiSharedContentModel>>();

        [Fact]
        public async Task ContentItemUpdaterFindAndUpdateAsyncHtmlReturnsSuccess()
        {
            // Arrange
            const bool expectedResult = true;
            var url = new Uri("https://www.somewhere.com", UriKind.Absolute);
            var contentItemId = Guid.NewGuid();
            var validContentItems = BuildValidContentItems(contentItemId, Constants.ContentTypeHtml);
            var service = new ContentItemUpdater(fakeHtmlMarkupContentItemUpdater, fakeHtmlSharedMarkupContentItemUpdater, fakeSharedContentMarkupContentItemUpdater);

            A.CallTo(() => fakeHtmlMarkupContentItemUpdater.FindAndUpdateAsync(A<ContentItemModel>.Ignored, A<Uri>.Ignored)).Returns(expectedResult);

            // Act
            var result = await service.FindAndUpdateAsync(url, contentItemId, validContentItems).ConfigureAwait(false);

            // Assert
            A.CallTo(() => fakeHtmlMarkupContentItemUpdater.FindAndUpdateAsync(A<ContentItemModel>.Ignored, A<Uri>.Ignored)).MustHaveHappenedOnceExactly();
            A.CallTo(() => fakeHtmlSharedMarkupContentItemUpdater.FindAndUpdateAsync(A<ContentItemModel>.Ignored, A<Uri>.Ignored)).MustNotHaveHappened();
            A.CallTo(() => fakeSharedContentMarkupContentItemUpdater.FindAndUpdateAsync(A<ContentItemModel>.Ignored, A<Uri>.Ignored)).MustNotHaveHappened();

            Assert.Equal(expectedResult, result);
        }

        [Fact]
        public async Task ContentItemUpdaterFindAndUpdateAsyncHtmlSharedReturnsSuccess()
        {
            // Arrange
            const bool expectedResult = true;
            var url = new Uri("https://www.somewhere.com", UriKind.Absolute);
            var contentItemId = Guid.NewGuid();
            var validContentItems = BuildValidContentItems(contentItemId, Constants.ContentTypeHtmlShared);
            var service = new ContentItemUpdater(fakeHtmlMarkupContentItemUpdater, fakeHtmlSharedMarkupContentItemUpdater, fakeSharedContentMarkupContentItemUpdater);

            A.CallTo(() => fakeHtmlSharedMarkupContentItemUpdater.FindAndUpdateAsync(A<ContentItemModel>.Ignored, A<Uri>.Ignored)).Returns(expectedResult);

            // Act
            var result = await service.FindAndUpdateAsync(url, contentItemId, validContentItems).ConfigureAwait(false);

            // Assert
            A.CallTo(() => fakeHtmlMarkupContentItemUpdater.FindAndUpdateAsync(A<ContentItemModel>.Ignored, A<Uri>.Ignored)).MustNotHaveHappened();
            A.CallTo(() => fakeHtmlSharedMarkupContentItemUpdater.FindAndUpdateAsync(A<ContentItemModel>.Ignored, A<Uri>.Ignored)).MustHaveHappenedOnceExactly();
            A.CallTo(() => fakeSharedContentMarkupContentItemUpdater.FindAndUpdateAsync(A<ContentItemModel>.Ignored, A<Uri>.Ignored)).MustNotHaveHappened();

            Assert.Equal(expectedResult, result);
        }

        [Fact]
        public async Task ContentItemUpdaterFindAndUpdateAsynSharedContentReturnsSuccess()
        {
            // Arrange
            const bool expectedResult = true;
            var url = new Uri("https://www.somewhere.com", UriKind.Absolute);
            var contentItemId = Guid.NewGuid();
            var validContentItems = BuildValidContentItems(contentItemId, Constants.ContentTypeSharedContent);
            var service = new ContentItemUpdater(fakeHtmlMarkupContentItemUpdater, fakeHtmlSharedMarkupContentItemUpdater, fakeSharedContentMarkupContentItemUpdater);

            A.CallTo(() => fakeSharedContentMarkupContentItemUpdater.FindAndUpdateAsync(A<ContentItemModel>.Ignored, A<Uri>.Ignored)).Returns(expectedResult);

            // Act
            var result = await service.FindAndUpdateAsync(url, contentItemId, validContentItems).ConfigureAwait(false);

            // Assert
            A.CallTo(() => fakeHtmlMarkupContentItemUpdater.FindAndUpdateAsync(A<ContentItemModel>.Ignored, A<Uri>.Ignored)).MustNotHaveHappened();
            A.CallTo(() => fakeHtmlSharedMarkupContentItemUpdater.FindAndUpdateAsync(A<ContentItemModel>.Ignored, A<Uri>.Ignored)).MustNotHaveHappened();
            A.CallTo(() => fakeSharedContentMarkupContentItemUpdater.FindAndUpdateAsync(A<ContentItemModel>.Ignored, A<Uri>.Ignored)).MustHaveHappenedOnceExactly();

            Assert.Equal(expectedResult, result);
        }

        [Fact]
        public async Task ContentItemUpdaterFindAndUpdateAsyncDoesNotFindItemReturnsFail()
        {
            // Arrange
            const bool expectedResult = false;
            var url = new Uri("https://www.somewhere.com", UriKind.Absolute);
            var contentItemId = Guid.NewGuid();
            var validContentItems = BuildValidContentItems(Guid.NewGuid(), Constants.ContentTypeHtml);
            var service = new ContentItemUpdater(fakeHtmlMarkupContentItemUpdater, fakeHtmlSharedMarkupContentItemUpdater, fakeSharedContentMarkupContentItemUpdater);

            // Act
            var result = await service.FindAndUpdateAsync(url, contentItemId, validContentItems).ConfigureAwait(false);

            // Assert
            A.CallTo(() => fakeHtmlMarkupContentItemUpdater.FindAndUpdateAsync(A<ContentItemModel>.Ignored, A<Uri>.Ignored)).MustNotHaveHappened();
            A.CallTo(() => fakeHtmlSharedMarkupContentItemUpdater.FindAndUpdateAsync(A<ContentItemModel>.Ignored, A<Uri>.Ignored)).MustNotHaveHappened();
            A.CallTo(() => fakeSharedContentMarkupContentItemUpdater.FindAndUpdateAsync(A<ContentItemModel>.Ignored, A<Uri>.Ignored)).MustNotHaveHappened();

            Assert.Equal(expectedResult, result);
        }

        [Fact]
        public void ContentItemUpdaterFindItemReturnsSuccess()
        {
            // Arrange
            var contentItemId = Guid.NewGuid();
            var expectedResult = BuildValidContentItem(contentItemId, Constants.ContentTypeHtml);
            var validContentItems = BuildValidContentItems(contentItemId, Constants.ContentTypeHtml);
            var service = new ContentItemUpdater(fakeHtmlMarkupContentItemUpdater, fakeHtmlSharedMarkupContentItemUpdater, fakeSharedContentMarkupContentItemUpdater);

            // Act
            var result = service.FindItem(contentItemId, validContentItems);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(expectedResult.ItemId, result!.ItemId);
            Assert.Equal(expectedResult.Title, result.Title);
        }

        [Fact]
        public void ContentItemUpdaterFindItemReturnsNullWhenNotFound()
        {
            // Arrange
            var contentItemId = Guid.NewGuid();
            var validContentItems = BuildValidContentItems(Guid.NewGuid(), Constants.ContentTypeHtml);
            var service = new ContentItemUpdater(fakeHtmlMarkupContentItemUpdater, fakeHtmlSharedMarkupContentItemUpdater, fakeSharedContentMarkupContentItemUpdater);

            // Act
            var result = service.FindItem(contentItemId, validContentItems);

            // Assert
            Assert.Null(result);
        }

        [Fact]
        public void ContentItemUpdaterFindItemReturnsNullWhenNullItems()
        {
            // Arrange
            var contentItemId = Guid.NewGuid();
            List<ContentItemModel>? nullContentItems = null;
            var service = new ContentItemUpdater(fakeHtmlMarkupContentItemUpdater, fakeHtmlSharedMarkupContentItemUpdater, fakeSharedContentMarkupContentItemUpdater);

            // Act
            var result = service.FindItem(contentItemId, nullContentItems);

            // Assert
            Assert.Null(result);
        }

        [Fact]
        public void ContentItemUpdaterFindItemReturnsNullWhenNoItems()
        {
            // Arrange
            var contentItemId = Guid.NewGuid();
            var emptyContentItems = new List<ContentItemModel>();
            var service = new ContentItemUpdater(fakeHtmlMarkupContentItemUpdater, fakeHtmlSharedMarkupContentItemUpdater, fakeSharedContentMarkupContentItemUpdater);

            // Act
            var result = service.FindItem(contentItemId, emptyContentItems);

            // Assert
            Assert.Null(result);
        }

        private List<ContentItemModel> BuildValidContentItems(Guid contentItemId, string contentType)
        {
            return new List<ContentItemModel>
            {
                new ContentItemModel
                {
                    ItemId = Guid.NewGuid(),
                    ContentType = Constants.ContentTypeHtml,
                    Title = "Title-1",
                    Content = "Content #1",
                    HtmlBody = "HtmlBody #1",
                    ContentItems = new List<ContentItemModel>
                    {
                        new ContentItemModel
                        {
                            ItemId = Guid.NewGuid(),
                            ContentType = Constants.ContentTypeHtmlShared,
                            Title = "Title-2",
                            Content = "Content #2",
                            HtmlBody = "HtmlBody #2",
                            ContentItems = new List<ContentItemModel>
                            {
                                new ContentItemModel
                                {
                                    ItemId = Guid.NewGuid(),
                                    ContentType = Constants.ContentTypeSharedContent,
                                    Title = "Title-3",
                                    Content = "Content #3",
                                    HtmlBody = "HtmlBody #3",
                                    ContentItems = new List<ContentItemModel>
                                    {
                                        BuildValidContentItem(contentItemId, contentType),
                                    },
                                },
                            },
                        },
                    },
                },
            };
        }

        private ContentItemModel BuildValidContentItem(Guid contentItemId, string contentType)
        {
            return new ContentItemModel
            {
                ItemId = contentItemId,
                ContentType = contentType,
                Title = "Title-X",
                Content = "Content #X",
                HtmlBody = "HtmlBody #X",
            };
        }
    }
}

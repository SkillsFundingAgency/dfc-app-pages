using DFC.App.Pages.Data.Common;
using DFC.App.Pages.Data.Models;
using DFC.App.Pages.Data.Models.CmsApiModels;
using DFC.App.Pages.Services.CacheContentService.ContentItemUpdaters;
using DFC.Content.Pkg.Netcore.Data.Contracts;
using FakeItEasy;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xunit;

namespace DFC.App.Pages.Services.CacheContentService.UnitTests.ContentItemUpdatersTests
{
    [Trait("Category", "ContentItemUpdaters PageLocatonUpdaterTests Unit Tests")]
    public class PageLocatonUpdaterTests
    {
        private readonly ICmsApiService fakeCmsApiService = A.Fake<ICmsApiService>();

        [Fact]
        public async Task PageLocatonUpdaterFindAndUpdateAsyncReturnsSuccess()
        {
            // Arrange
            const bool expectedResult = true;
            var url = new Uri("https://www.somewhere.com", UriKind.Absolute);
            var contentItemId = Guid.NewGuid();
            var validPageLocatons = BuildValidPageLocations(contentItemId);
            var dummyCmsApiPageLocationModel = A.Dummy<CmsApiPageLocationModel>();
            var service = new PageLocatonUpdater(fakeCmsApiService);

            A.CallTo(() => fakeCmsApiService.GetContentItemAsync<CmsApiPageLocationModel>(A<Uri>.Ignored)).Returns(dummyCmsApiPageLocationModel);

            // Act
            var result = await service.FindAndUpdateAsync(url, contentItemId, validPageLocatons).ConfigureAwait(false);

            // Assert
            A.CallTo(() => fakeCmsApiService.GetContentItemAsync<CmsApiPageLocationModel>(A<Uri>.Ignored)).MustHaveHappenedOnceExactly();

            Assert.Equal(expectedResult, result);
        }

        [Fact]
        public async Task PageLocatonUpdaterFindAndUpdateAsyncDoesNotFindItemReturnsFail()
        {
            // Arrange
            const bool expectedResult = false;
            var url = new Uri("https://www.somewhere.com", UriKind.Absolute);
            var contentItemId = Guid.NewGuid();
            var validPageLocatons = BuildValidPageLocations(Guid.NewGuid());
            var service = new PageLocatonUpdater(fakeCmsApiService);

            // Act
            var result = await service.FindAndUpdateAsync(url, contentItemId, validPageLocatons).ConfigureAwait(false);

            // Assert
            A.CallTo(() => fakeCmsApiService.GetContentItemAsync<CmsApiPageLocationModel>(A<Uri>.Ignored)).MustNotHaveHappened();

            Assert.Equal(expectedResult, result);
        }

        [Fact]
        public async Task PageLocatonUpdaterFindAndUpdateAsyncDoesNotGetItemReturnsFail()
        {
            // Arrange
            const bool expectedResult = false;
            var url = new Uri("https://www.somewhere.com", UriKind.Absolute);
            var contentItemId = Guid.NewGuid();
            var validPageLocatons = BuildValidPageLocations(contentItemId);
            CmsApiPageLocationModel? nullCmsApiPageLocationModel = null;
            var service = new PageLocatonUpdater(fakeCmsApiService);

            A.CallTo(() => fakeCmsApiService.GetContentItemAsync<CmsApiPageLocationModel>(A<Uri>.Ignored)).Returns(nullCmsApiPageLocationModel);

            // Act
            var result = await service.FindAndUpdateAsync(url, contentItemId, validPageLocatons).ConfigureAwait(false);

            // Assert
            A.CallTo(() => fakeCmsApiService.GetContentItemAsync<CmsApiPageLocationModel>(A<Uri>.Ignored)).MustHaveHappenedOnceExactly();

            Assert.Equal(expectedResult, result);
        }

        [Fact]
        public void PageLocatonUpdaterFindItemReturnsSuccess()
        {
            // Arrange
            var contentItemId = Guid.NewGuid();
            var expectedResult = BuildValidPageLocation(contentItemId);
            var validPageLocatons = BuildValidPageLocations(contentItemId);
            var service = new PageLocatonUpdater(fakeCmsApiService);

            // Act
            var result = service.FindItem(contentItemId, validPageLocatons);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(expectedResult.ItemId, result!.ItemId);
            Assert.Equal(expectedResult.BreadcrumbLinkSegment, result.BreadcrumbLinkSegment);
        }

        [Fact]
        public void PageLocatonUpdaterFindItemReturnsNullWhenNotFound()
        {
            // Arrange
            var contentItemId = Guid.NewGuid();
            var validPageLocatons = BuildValidPageLocations(Guid.NewGuid());
            var service = new PageLocatonUpdater(fakeCmsApiService);

            // Act
            var result = service.FindItem(contentItemId, validPageLocatons);

            // Assert
            Assert.Null(result);
        }

        [Fact]
        public void PageLocatonUpdaterFindItemReturnsNullWhenNullItems()
        {
            // Arrange
            var contentItemId = Guid.NewGuid();
            List<PageLocationModel>? nullPageLocations = null;
            var service = new PageLocatonUpdater(fakeCmsApiService);

            // Act
            var result = service.FindItem(contentItemId, nullPageLocations);

            // Assert
            Assert.Null(result);
        }

        [Fact]
        public void PageLocatonUpdaterFindItemReturnsNullWhenNoItems()
        {
            // Arrange
            var contentItemId = Guid.NewGuid();
            var emptyPageLocatons = new List<PageLocationModel>();
            var service = new PageLocatonUpdater(fakeCmsApiService);

            // Act
            var result = service.FindItem(contentItemId, emptyPageLocatons);

            // Assert
            Assert.Null(result);
        }

        private List<PageLocationModel> BuildValidPageLocations(Guid contentItemId)
        {
            return new List<PageLocationModel>
            {
                new PageLocationModel
                {
                    ItemId = Guid.NewGuid(),
                    ContentType = Constants.ContentTypePageLocation,
                    BreadcrumbLinkSegment = "BreadcrumbLinkSegment-1",
                    BreadcrumbText = "BreadcrumbText #1",
                    PageLocations = new List<PageLocationModel>
                    {
                        new PageLocationModel
                        {
                            ItemId = Guid.NewGuid(),
                            ContentType = Constants.ContentTypePageLocation,
                            BreadcrumbLinkSegment = "BreadcrumbLinkSegment-2",
                            BreadcrumbText = "BreadcrumbText #2",
                            PageLocations = new List<PageLocationModel>
                            {
                                new PageLocationModel
                                {
                                    ItemId = Guid.NewGuid(),
                                    ContentType = Constants.ContentTypePageLocation,
                                    BreadcrumbLinkSegment = "BreadcrumbLinkSegment-3",
                                    BreadcrumbText = "BreadcrumbText #3",
                                    PageLocations = new List<PageLocationModel>
                                    {
                                        BuildValidPageLocation(contentItemId),
                                    },
                                },
                            },
                        },
                    },
                },
            };
        }

        private PageLocationModel BuildValidPageLocation(Guid contentItemId)
        {
            return new PageLocationModel
            {
                ItemId = contentItemId,
                ContentType = Constants.ContentTypePageLocation,
                BreadcrumbLinkSegment = "BreadcrumbLinkSegment-X",
                BreadcrumbText = "BreadcrumbText #X",
            };
        }
    }
}

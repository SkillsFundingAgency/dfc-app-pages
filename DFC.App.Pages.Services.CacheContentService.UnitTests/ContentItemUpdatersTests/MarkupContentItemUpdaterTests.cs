using DFC.App.Pages.Data.Contracts;
using DFC.App.Pages.Data.Models;
using DFC.App.Pages.Data.Models.CmsApiModels;
using DFC.Content.Pkg.Netcore.Data.Contracts;
using FakeItEasy;
using System;
using System.Threading.Tasks;
using Xunit;

namespace DFC.App.Pages.Services.CacheContentService.UnitTests.ContentItemUpdatersTests
{
    [Trait("Category", "ContentItemUpdaters MarkupContentItemUpdater Unit Tests")]
    public class MarkupContentItemUpdaterTests
    {
        private readonly ICmsApiService fakeCmsApiService = A.Fake<ICmsApiService>();

        [Fact]
        public async Task ContentItemUpdaterFindAndUpdateAsyncReturnsSuccess()
        {
            // Arrange
            const bool expectedResult = true;
            var dummyCmsApiHtmlModel = A.Dummy<CmsApiHtmlModel>();
            var dummyContentItemModel = A.Dummy<ContentItemModel>();
            var url = new Uri("https://www.somewhere.com", UriKind.Absolute);
            var service = new MarkupContentItemUpdater<CmsApiHtmlModel>(fakeCmsApiService);

            A.CallTo(() => fakeCmsApiService.GetContentItemAsync<CmsApiHtmlModel>(A<Uri>.Ignored)).Returns(dummyCmsApiHtmlModel);

            // Act
            var result = await service.FindAndUpdateAsync(dummyContentItemModel, url).ConfigureAwait(false);

            // Assert
            A.CallTo(() => fakeCmsApiService.GetContentItemAsync<CmsApiHtmlModel>(A<Uri>.Ignored)).MustHaveHappenedOnceExactly();

            Assert.Equal(expectedResult, result!);
        }

        [Fact]
        public async Task ContentItemUpdaterFindAndUpdateAsyncThrowsExceptionFailWhenNullContentitemmodel()
        {
            // Arrange
            var dummyCmsApiHtmlModel = A.Dummy<CmsApiHtmlModel>();
            ContentItemModel? nullContentItemModel = null;
            var url = new Uri("https://www.somewhere.com", UriKind.Absolute);
            var service = new MarkupContentItemUpdater<CmsApiHtmlModel>(fakeCmsApiService);

            A.CallTo(() => fakeCmsApiService.GetContentItemAsync<CmsApiHtmlModel>(A<Uri>.Ignored)).Returns(dummyCmsApiHtmlModel);

            // Act & Assert
            await Assert.ThrowsAsync<ArgumentNullException>(async () => await service.FindAndUpdateAsync(nullContentItemModel, url).ConfigureAwait(false)).ConfigureAwait(false);
        }

        [Fact]
        public async Task ContentItemUpdaterFindAndUpdateAsyncReturnsFailWhenNoApiResponse()
        {
            // Arrange
            const bool expectedResult = false;
            CmsApiHtmlModel? nullCmsApiHtmlModel = null;
            var dummyContentItemModel = A.Dummy<ContentItemModel>();
            var url = new Uri("https://www.somewhere.com", UriKind.Absolute);
            var service = new MarkupContentItemUpdater<CmsApiHtmlModel>(fakeCmsApiService);

            A.CallTo(() => fakeCmsApiService.GetContentItemAsync<CmsApiHtmlModel>(A<Uri>.Ignored)).Returns(nullCmsApiHtmlModel);

            // Act
            var result = await service.FindAndUpdateAsync(dummyContentItemModel, url).ConfigureAwait(false);

            // Assert
            A.CallTo(() => fakeCmsApiService.GetContentItemAsync<CmsApiHtmlModel>(A<Uri>.Ignored)).MustHaveHappenedOnceExactly();

            Assert.Equal(expectedResult, result!);
        }
    }
}
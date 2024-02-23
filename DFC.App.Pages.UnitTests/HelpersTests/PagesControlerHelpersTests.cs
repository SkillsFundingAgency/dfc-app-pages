using DFC.App.Pages.Data.Models;
using DFC.App.Pages.Helpers;
using DFC.App.Pages.Models;
using DFC.Compui.Cosmos.Contracts;

using FakeItEasy;

using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

using Xunit;

namespace DFC.App.Pages.UnitTests.HelpersTests
{
    [Trait("Category", "Pages Controler Helpers")]
    public class PagesControlerHelpersTests
    {
        private readonly IContentPageService<ContentPageModel> fakeContentPageService = A.Fake<IContentPageService<ContentPageModel>>();
        private readonly IMemoryCache memoryCache = new MemoryCache(Options.Create(new MemoryCacheOptions()));

        [Fact]
        public void PagesControlerHelpersTestsExtractPageLocationReturnsSuccessForFiveLocations()
        {
            // Arrange
            const string expectedLocation = "one/two/three/four";
            const string expectedArticle = "five";
            var pageRequestModel = new PageRequestModel
            {
                Location1 = "one",
                Location2 = "two",
                Location3 = "three",
                Location4 = "four",
                Location5 = "five",
            };

            // Act
            var (location, article) = PagesControlerHelpers.ExtractPageLocation(pageRequestModel);

            // Assert
            Assert.Equal(expectedLocation, location);
            Assert.Equal(expectedArticle, article);
        }

        [Fact]
        public void PagesControlerHelpersTestsExtractPageLocationReturnsSuccessForZeroLocations()
        {
            // Arrange
            string expectedLocation = string.Empty;
            string expectedArticle = string.Empty;
            var pageRequestModel = new PageRequestModel
            {
                Location1 = null,
                Location2 = null,
                Location3 = string.Empty,
                Location4 = null,
                Location5 = string.Empty,
            };

            // Act
            var (location, article) = PagesControlerHelpers.ExtractPageLocation(pageRequestModel);

            // Assert
            Assert.Equal(expectedLocation, location);
            Assert.Equal(expectedArticle, article);
        }

        [Fact]
        public void PagesControlerHelpersTestsExtractPageLocationReturnsSuccessForOneLocation()
        {
            // Arrange
            const string expectedLocation = "one";
            string expectedArticle = string.Empty;
            var pageRequestModel = new PageRequestModel
            {
                Location1 = "one",
                Location2 = null,
                Location3 = string.Empty,
                Location4 = null,
                Location5 = string.Empty,
            };

            // Act
            var (location, article) = PagesControlerHelpers.ExtractPageLocation(pageRequestModel);

            // Assert
            Assert.Equal(expectedLocation, location);
            Assert.Equal(expectedArticle, article);
        }

        [Fact]
        public async Task PagesControlerHelpersTestsGetContentPageAsyncReturnsSuccess()
        {
            // Arrange
            var expectedResults = A.CollectionOfDummy<ContentPageModel>(1);
            const string location = "a-location";
            const string article = "an-article";
            var helper = new PagesControlerHelpers(fakeContentPageService, memoryCache);

            A.CallTo(() => fakeContentPageService.GetAsync(A<Expression<Func<ContentPageModel, bool>>>.Ignored)).Returns(expectedResults);

            // Act
            var result = await helper.GetContentPageAsync(location, article).ConfigureAwait(false);

            // Assert
            A.CallTo(() => fakeContentPageService.GetAsync(A<Expression<Func<ContentPageModel, bool>>>.Ignored)).MustHaveHappenedOnceExactly();

            Assert.Equal(expectedResults.First(), result);
        }

        [Fact]
        public async Task PagesControlerHelpersTestsGetContentPageAsyncReturnsSuccessEmptyParameters()
        {
            // Arrange
            var expectedResults = A.CollectionOfDummy<ContentPageModel>(1);
            string location = string.Empty;
            const string? article = null;
            var helper = new PagesControlerHelpers(fakeContentPageService, memoryCache);

            A.CallTo(() => fakeContentPageService.GetAsync(A<Expression<Func<ContentPageModel, bool>>>.Ignored)).Returns(expectedResults);

            // Act
            var result = await helper.GetContentPageAsync(location, article).ConfigureAwait(false);

            // Assert
            A.CallTo(() => fakeContentPageService.GetAsync(A<Expression<Func<ContentPageModel, bool>>>.Ignored)).MustHaveHappenedOnceExactly();

            Assert.Equal(expectedResults.First(), result);
        }

        [Fact]
        public async Task PagesControlerHelpersTestsGetContentPageAsyncReturnsSuccessEmptyArticle()
        {
            // Arrange
            var expectedResults = A.CollectionOfDummy<ContentPageModel>(1);
            const string location = "a-location";
            string article = string.Empty;
            var helper = new PagesControlerHelpers(fakeContentPageService, memoryCache);

            A.CallTo(() => fakeContentPageService.GetAsync(A<Expression<Func<ContentPageModel, bool>>>.Ignored)).Returns(expectedResults);

            // Act
            var result = await helper.GetContentPageAsync(location, article).ConfigureAwait(false);

            // Assert
            A.CallTo(() => fakeContentPageService.GetAsync(A<Expression<Func<ContentPageModel, bool>>>.Ignored)).MustHaveHappenedOnceExactly();

            Assert.Equal(expectedResults.First(), result);
        }

        [Fact]
        public async Task PagesControlerHelpersTestsGetContentPageAsyncReturnsNull()
        {
            // Arrange
            List<ContentPageModel>? expectedResults = null;
            const string location = "a-location";
            const string article = "an-article";
            var helper = new PagesControlerHelpers(fakeContentPageService, memoryCache);

            A.CallTo(() => fakeContentPageService.GetAsync(A<Expression<Func<ContentPageModel, bool>>>.Ignored)).Returns(expectedResults);

            // Act
            var result = await helper.GetContentPageAsync(location, article).ConfigureAwait(false);

            // Assert
            A.CallTo(() => fakeContentPageService.GetAsync(A<Expression<Func<ContentPageModel, bool>>>.Ignored)).MustHaveHappenedTwiceExactly();

            Assert.Null(result);
        }

        [Fact]
        public async Task PagesControlerHelpersTestsGetRedirectedContentPageAsyncReturnsSuccess()
        {
            // Arrange
            var expectedResult = A.Dummy<ContentPageModel>();
            const string location = "a-location";
            const string article = "an-article";
            var helper = new PagesControlerHelpers(fakeContentPageService, memoryCache);

            A.CallTo(() => fakeContentPageService.GetByRedirectLocationAsync(A<string>.Ignored)).Returns(expectedResult);

            // Act
            var result = await helper.GetRedirectedContentPageAsync(location, article).ConfigureAwait(false);

            // Assert
            A.CallTo(() => fakeContentPageService.GetByRedirectLocationAsync(A<string>.Ignored)).MustHaveHappenedOnceExactly();

            Assert.Equal(expectedResult, result);
        }

        [Fact]
        public async Task PagesControlerHelpersTestsGetRedirectedContentPageAsyncReturnsSuccessWithMissingArticle()
        {
            // Arrange
            var expectedResult = A.Dummy<ContentPageModel>();
            const string location = "a-location";
            const string? article = null;
            var helper = new PagesControlerHelpers(fakeContentPageService, memoryCache);

            A.CallTo(() => fakeContentPageService.GetByRedirectLocationAsync(A<string>.Ignored)).Returns(expectedResult);

            // Act
            var result = await helper.GetRedirectedContentPageAsync(location, article).ConfigureAwait(false);

            // Assert
            A.CallTo(() => fakeContentPageService.GetByRedirectLocationAsync(A<string>.Ignored)).MustHaveHappenedOnceExactly();

            Assert.Equal(expectedResult, result);
        }
    }
}

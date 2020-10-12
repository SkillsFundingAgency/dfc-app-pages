using DFC.App.Pages.Data.Contracts;
using DFC.App.Pages.Data.Models;
using DFC.App.Pages.Data.Models.CmsApiModels;
using DFC.Content.Pkg.Netcore.Data.Contracts;
using DFC.Content.Pkg.Netcore.Data.Models;
using FakeItEasy;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace DFC.App.Pages.Services.CacheContentService.UnitTests
{
    [Trait("Category", "Cache Reload background service Unit Tests")]
    public class CacheReloadServiceTests
    {
        private readonly ILogger<CacheReloadService> fakeLogger = A.Fake<ILogger<CacheReloadService>>();
        private readonly AutoMapper.IMapper fakeMapper = A.Fake<AutoMapper.IMapper>();
        private readonly IEventMessageService<ContentPageModel> fakeEventMessageService = A.Fake<IEventMessageService<ContentPageModel>>();
        private readonly ICmsApiService fakeCmsApiService = A.Fake<ICmsApiService>();
        private readonly IContentCacheService fakeContentCacheService = A.Fake<IContentCacheService>();
        private readonly IAppRegistryApiService fakeAppRegistryService = A.Fake<IAppRegistryApiService>();
        private readonly IContentTypeMappingService fakeContentTypeMappingService = A.Fake<IContentTypeMappingService>();

        public static IEnumerable<object[]> TestValidationData => new List<object[]>
        {
            new object[] { BuildValidContentPageModel(), true },
            new object[] { A.Fake<ContentPageModel>(), false },
        };

        [Fact]
        public async Task CacheReloadServiceReloadIsSuccessfulForCreate()
        {
            // arrange
            const int NumerOfSummaryItems = 2;
            const int NumberOfDeletions = 3;
            var cancellationToken = new CancellationToken(false);
            var expectedValidContentPageModel = BuildValidContentPageModel();
            var fakePagesSummaryItemModels = BuldFakePagesSummaryItemModel(NumerOfSummaryItems);
            var fakeCachedContentPageModels = BuldFakeContentPageModels(NumberOfDeletions);

            A.CallTo(() => fakeCmsApiService.GetSummaryAsync<CmsApiSummaryItemModel>()).Returns(fakePagesSummaryItemModels);
            A.CallTo(() => fakeCmsApiService.GetItemAsync<CmsApiDataModel>(A<Uri>.Ignored)).Returns(A.Fake<CmsApiDataModel>());
            A.CallTo(() => fakeMapper.Map<ContentPageModel>(A<CmsApiDataModel>.Ignored)).Returns(expectedValidContentPageModel);
            A.CallTo(() => fakeEventMessageService.UpdateAsync(A<ContentPageModel>.Ignored)).Returns(HttpStatusCode.NotFound);
            A.CallTo(() => fakeEventMessageService.CreateAsync(A<ContentPageModel>.Ignored)).Returns(HttpStatusCode.Created);
            A.CallTo(() => fakeEventMessageService.GetAllCachedItemsAsync()).Returns(fakeCachedContentPageModels);
            A.CallTo(() => fakeEventMessageService.DeleteAsync(A<Guid>.Ignored)).Returns(HttpStatusCode.OK);
            A.CallTo(() => fakeContentCacheService.AddOrReplace(A<Guid>.Ignored, A<List<Guid>>.Ignored, A<string>.Ignored));

            var cacheReloadService = new CacheReloadService(fakeLogger, fakeMapper, fakeEventMessageService, fakeCmsApiService, fakeContentCacheService, fakeAppRegistryService, fakeContentTypeMappingService);

            // act
            await cacheReloadService.Reload(cancellationToken).ConfigureAwait(false);

            // assert
            A.CallTo(() => fakeContentTypeMappingService.AddMapping(A<string>.Ignored, A<Type>.Ignored)).MustHaveHappenedOnceOrMore();
            A.CallTo(() => fakeCmsApiService.GetSummaryAsync<CmsApiSummaryItemModel>()).MustHaveHappenedOnceExactly();
            A.CallTo(() => fakeContentCacheService.Clear()).MustHaveHappenedOnceExactly();
            A.CallTo(() => fakeCmsApiService.GetItemAsync<CmsApiDataModel>(A<Uri>.Ignored)).MustHaveHappened(NumerOfSummaryItems, Times.Exactly);
            A.CallTo(() => fakeMapper.Map<ContentPageModel>(A<CmsApiDataModel>.Ignored)).MustHaveHappened(NumerOfSummaryItems, Times.Exactly);
            A.CallTo(() => fakeEventMessageService.UpdateAsync(A<ContentPageModel>.Ignored)).MustHaveHappened(NumerOfSummaryItems, Times.Exactly);
            A.CallTo(() => fakeEventMessageService.CreateAsync(A<ContentPageModel>.Ignored)).MustHaveHappened(NumerOfSummaryItems, Times.Exactly);
            A.CallTo(() => fakeEventMessageService.GetAllCachedItemsAsync()).MustHaveHappenedTwiceExactly();
            A.CallTo(() => fakeEventMessageService.DeleteAsync(A<Guid>.Ignored)).MustHaveHappened(NumberOfDeletions, Times.Exactly);
            A.CallTo(() => fakeContentCacheService.AddOrReplace(A<Guid>.Ignored, A<List<Guid>>.Ignored, A<string>.Ignored)).MustHaveHappened(NumerOfSummaryItems, Times.Exactly);
            A.CallTo(() => fakeAppRegistryService.PagesDataLoadAsync()).MustHaveHappenedOnceExactly();
        }

        [Fact]
        public async Task CacheReloadServiceReloadIsSuccessfulForUpdate()
        {
            // arrange
            const int NumerOfSummaryItems = 2;
            const int NumberOfDeletions = 3;
            var cancellationToken = new CancellationToken(false);
            var expectedValidContentPageModel = BuildValidContentPageModel();
            var fakePagesSummaryItemModels = BuldFakePagesSummaryItemModel(NumerOfSummaryItems);
            var fakeCachedContentPageModels = BuldFakeContentPageModels(NumberOfDeletions);

            A.CallTo(() => fakeCmsApiService.GetSummaryAsync<CmsApiSummaryItemModel>()).Returns(fakePagesSummaryItemModels);
            A.CallTo(() => fakeCmsApiService.GetItemAsync<CmsApiDataModel>(A<Uri>.Ignored)).Returns(A.Fake<CmsApiDataModel>());
            A.CallTo(() => fakeMapper.Map<ContentPageModel>(A<CmsApiDataModel>.Ignored)).Returns(expectedValidContentPageModel);
            A.CallTo(() => fakeEventMessageService.UpdateAsync(A<ContentPageModel>.Ignored)).Returns(HttpStatusCode.OK);
            A.CallTo(() => fakeEventMessageService.GetAllCachedItemsAsync()).Returns(fakeCachedContentPageModels);
            A.CallTo(() => fakeEventMessageService.DeleteAsync(A<Guid>.Ignored)).Returns(HttpStatusCode.OK);
            A.CallTo(() => fakeContentCacheService.AddOrReplace(A<Guid>.Ignored, A<List<Guid>>.Ignored, A<string>.Ignored));

            var cacheReloadService = new CacheReloadService(fakeLogger, fakeMapper, fakeEventMessageService, fakeCmsApiService, fakeContentCacheService, fakeAppRegistryService, fakeContentTypeMappingService);

            // act
            await cacheReloadService.Reload(cancellationToken).ConfigureAwait(false);

            // assert
            A.CallTo(() => fakeContentTypeMappingService.AddMapping(A<string>.Ignored, A<Type>.Ignored)).MustHaveHappenedOnceOrMore();
            A.CallTo(() => fakeCmsApiService.GetSummaryAsync<CmsApiSummaryItemModel>()).MustHaveHappenedOnceExactly();
            A.CallTo(() => fakeContentCacheService.Clear()).MustHaveHappenedOnceExactly();
            A.CallTo(() => fakeCmsApiService.GetItemAsync<CmsApiDataModel>(A<Uri>.Ignored)).MustHaveHappened(NumerOfSummaryItems, Times.Exactly);
            A.CallTo(() => fakeMapper.Map<ContentPageModel>(A<CmsApiDataModel>.Ignored)).MustHaveHappened(NumerOfSummaryItems, Times.Exactly);
            A.CallTo(() => fakeEventMessageService.UpdateAsync(A<ContentPageModel>.Ignored)).MustHaveHappened(NumerOfSummaryItems, Times.Exactly);
            A.CallTo(() => fakeEventMessageService.CreateAsync(A<ContentPageModel>.Ignored)).MustNotHaveHappened();
            A.CallTo(() => fakeEventMessageService.GetAllCachedItemsAsync()).MustHaveHappenedTwiceExactly();
            A.CallTo(() => fakeEventMessageService.DeleteAsync(A<Guid>.Ignored)).MustHaveHappened(NumberOfDeletions, Times.Exactly);
            A.CallTo(() => fakeContentCacheService.AddOrReplace(A<Guid>.Ignored, A<List<Guid>>.Ignored, A<string>.Ignored)).MustHaveHappened(NumerOfSummaryItems, Times.Exactly);
            A.CallTo(() => fakeAppRegistryService.PagesDataLoadAsync()).MustHaveHappenedOnceExactly();
        }

        [Fact]
        public async Task CacheReloadServiceReloadIsCancelled()
        {
            // arrange
            const int NumerOfSummaryItems = 2;
            const int NumberOfDeletions = 1;
            var cancellationToken = new CancellationToken(true);
            var fakePagesSummaryItemModels = BuldFakePagesSummaryItemModel(NumerOfSummaryItems);
            var fakeCachedContentPageModels = BuldFakeContentPageModels(NumberOfDeletions);

            A.CallTo(() => fakeEventMessageService.GetAllCachedItemsAsync()).Returns(fakeCachedContentPageModels);
            A.CallTo(() => fakeCmsApiService.GetSummaryAsync<CmsApiSummaryItemModel>()).Returns(fakePagesSummaryItemModels);

            var cacheReloadService = new CacheReloadService(fakeLogger, fakeMapper, fakeEventMessageService, fakeCmsApiService, fakeContentCacheService, fakeAppRegistryService, fakeContentTypeMappingService);

            // act
            await cacheReloadService.Reload(cancellationToken).ConfigureAwait(false);

            // assert
            A.CallTo(() => fakeContentTypeMappingService.AddMapping(A<string>.Ignored, A<Type>.Ignored)).MustHaveHappenedOnceOrMore();
            A.CallTo(() => fakeCmsApiService.GetSummaryAsync<CmsApiSummaryItemModel>()).MustHaveHappenedOnceExactly();
            A.CallTo(() => fakeContentCacheService.Clear()).MustNotHaveHappened();
            A.CallTo(() => fakeCmsApiService.GetItemAsync<CmsApiDataModel>(A<Uri>.Ignored)).MustNotHaveHappened();
            A.CallTo(() => fakeMapper.Map<ContentPageModel>(A<CmsApiDataModel>.Ignored)).MustNotHaveHappened();
            A.CallTo(() => fakeEventMessageService.UpdateAsync(A<ContentPageModel>.Ignored)).MustNotHaveHappened();
            A.CallTo(() => fakeEventMessageService.CreateAsync(A<ContentPageModel>.Ignored)).MustNotHaveHappened();
            A.CallTo(() => fakeEventMessageService.GetAllCachedItemsAsync()).MustHaveHappenedOnceExactly();
            A.CallTo(() => fakeEventMessageService.DeleteAsync(A<Guid>.Ignored)).MustNotHaveHappened();
            A.CallTo(() => fakeContentCacheService.AddOrReplace(A<Guid>.Ignored, A<List<Guid>>.Ignored, A<string>.Ignored)).MustNotHaveHappened();
            A.CallTo(() => fakeAppRegistryService.PagesDataLoadAsync()).MustNotHaveHappened();
        }

        [Fact]
        public async Task CacheReloadServiceRemoveDuplicateCacheItemsIsSuccessful()
        {
            // arrange
            const int NumberOfItems = 1;
            var fakeCachedContentPageModels = BuldFakeContentPageModels(NumberOfItems);

            fakeCachedContentPageModels.Add(fakeCachedContentPageModels.First());

            A.CallTo(() => fakeEventMessageService.GetAllCachedItemsAsync()).Returns(fakeCachedContentPageModels);
            A.CallTo(() => fakeEventMessageService.DeleteAsync(A<Guid>.Ignored)).Returns(HttpStatusCode.OK);

            var cacheReloadService = new CacheReloadService(fakeLogger, fakeMapper, fakeEventMessageService, fakeCmsApiService, fakeContentCacheService, fakeAppRegistryService, fakeContentTypeMappingService);

            // act
            await cacheReloadService.RemoveDuplicateCacheItems().ConfigureAwait(false);

            // assert
            A.CallTo(() => fakeEventMessageService.GetAllCachedItemsAsync()).MustHaveHappenedOnceExactly();
            A.CallTo(() => fakeEventMessageService.DeleteAsync(A<Guid>.Ignored)).MustHaveHappened(NumberOfItems, Times.Exactly);
        }

        [Fact]
        public async Task CacheReloadServiceRemoveDuplicateCacheItemsNoData()
        {
            // arrange
            const int NumberOfItems = 0;
            var fakeCachedContentPageModels = BuldFakeContentPageModels(NumberOfItems);

            A.CallTo(() => fakeEventMessageService.GetAllCachedItemsAsync()).Returns(fakeCachedContentPageModels);

            var cacheReloadService = new CacheReloadService(fakeLogger, fakeMapper, fakeEventMessageService, fakeCmsApiService, fakeContentCacheService, fakeAppRegistryService, fakeContentTypeMappingService);

            // act
            await cacheReloadService.RemoveDuplicateCacheItems().ConfigureAwait(false);

            // assert
            A.CallTo(() => fakeEventMessageService.GetAllCachedItemsAsync()).MustHaveHappenedOnceExactly();
            A.CallTo(() => fakeEventMessageService.DeleteAsync(A<Guid>.Ignored)).MustNotHaveHappened();
        }

        [Fact]
        public async Task CacheReloadServiceGetAndSaveItemIsSuccessfulForCreate()
        {
            // arrange
            var cancellationToken = new CancellationToken(false);
            var expectedValidPagesSummaryItemModel = BuildValidPagesSummaryItemModel();
            var expectedValidPagesApiDataModel = BuildValidPagesApiDataModel();
            var expectedValidContentPageModel = BuildValidContentPageModel();

            A.CallTo(() => fakeCmsApiService.GetItemAsync<CmsApiDataModel>(A<Uri>.Ignored)).Returns(expectedValidPagesApiDataModel);
            A.CallTo(() => fakeMapper.Map<ContentPageModel>(A<CmsApiDataModel>.Ignored)).Returns(expectedValidContentPageModel);
            A.CallTo(() => fakeEventMessageService.UpdateAsync(A<ContentPageModel>.Ignored)).Returns(HttpStatusCode.NotFound);
            A.CallTo(() => fakeContentCacheService.AddOrReplace(A<Guid>.Ignored, A<List<Guid>>.Ignored, A<string>.Ignored));

            var cacheReloadService = new CacheReloadService(fakeLogger, fakeMapper, fakeEventMessageService, fakeCmsApiService, fakeContentCacheService, fakeAppRegistryService, fakeContentTypeMappingService);

            // act
            await cacheReloadService.GetAndSaveItemAsync(expectedValidPagesSummaryItemModel, cancellationToken).ConfigureAwait(false);

            // assert
            A.CallTo(() => fakeCmsApiService.GetItemAsync<CmsApiDataModel>(A<Uri>.Ignored)).MustHaveHappenedOnceExactly();
            A.CallTo(() => fakeMapper.Map<ContentPageModel>(A<CmsApiDataModel>.Ignored)).MustHaveHappenedOnceExactly();
            A.CallTo(() => fakeEventMessageService.UpdateAsync(A<ContentPageModel>.Ignored)).MustHaveHappenedOnceExactly();
            A.CallTo(() => fakeEventMessageService.CreateAsync(A<ContentPageModel>.Ignored)).MustHaveHappenedOnceExactly();
            A.CallTo(() => fakeContentCacheService.AddOrReplace(A<Guid>.Ignored, A<List<Guid>>.Ignored, A<string>.Ignored)).MustHaveHappenedOnceExactly();
        }

        [Fact]
        public async Task CacheReloadServiceGetAndSaveItemIsSuccessfulForUpdate()
        {
            // arrange
            var cancellationToken = new CancellationToken(false);
            var expectedValidPagesSummaryItemModel = BuildValidPagesSummaryItemModel();
            var expectedValidPagesApiDataModel = BuildValidPagesApiDataModel();
            var expectedValidContentPageModel = BuildValidContentPageModel();

            A.CallTo(() => fakeCmsApiService.GetItemAsync<CmsApiDataModel>(A<Uri>.Ignored)).Returns(expectedValidPagesApiDataModel);
            A.CallTo(() => fakeMapper.Map<ContentPageModel>(A<CmsApiDataModel>.Ignored)).Returns(expectedValidContentPageModel);
            A.CallTo(() => fakeEventMessageService.UpdateAsync(A<ContentPageModel>.Ignored)).Returns(HttpStatusCode.OK);
            A.CallTo(() => fakeContentCacheService.AddOrReplace(A<Guid>.Ignored, A<List<Guid>>.Ignored, A<string>.Ignored));

            var cacheReloadService = new CacheReloadService(fakeLogger, fakeMapper, fakeEventMessageService, fakeCmsApiService, fakeContentCacheService, fakeAppRegistryService, fakeContentTypeMappingService);

            // act
            await cacheReloadService.GetAndSaveItemAsync(expectedValidPagesSummaryItemModel, cancellationToken).ConfigureAwait(false);

            // assert
            A.CallTo(() => fakeCmsApiService.GetItemAsync<CmsApiDataModel>(A<Uri>.Ignored)).MustHaveHappenedOnceExactly();
            A.CallTo(() => fakeMapper.Map<ContentPageModel>(A<CmsApiDataModel>.Ignored)).MustHaveHappenedOnceExactly();
            A.CallTo(() => fakeEventMessageService.UpdateAsync(A<ContentPageModel>.Ignored)).MustHaveHappenedOnceExactly();
            A.CallTo(() => fakeEventMessageService.CreateAsync(A<ContentPageModel>.Ignored)).MustNotHaveHappened();
            A.CallTo(() => fakeContentCacheService.AddOrReplace(A<Guid>.Ignored, A<List<Guid>>.Ignored, A<string>.Ignored)).MustHaveHappenedOnceExactly();
        }

        [Fact]
        public async Task CacheReloadServiceGetAndSaveItemIsCancelled()
        {
            // arrange
            var cancellationToken = new CancellationToken(true);
            var expectedValidPagesSummaryItemModel = BuildValidPagesSummaryItemModel();
            var expectedValidPagesApiDataModel = BuildValidPagesApiDataModel();

            A.CallTo(() => fakeCmsApiService.GetItemAsync<CmsApiDataModel>(A<Uri>.Ignored)).Returns(expectedValidPagesApiDataModel);

            var cacheReloadService = new CacheReloadService(fakeLogger, fakeMapper, fakeEventMessageService, fakeCmsApiService, fakeContentCacheService, fakeAppRegistryService, fakeContentTypeMappingService);

            // act
            await cacheReloadService.GetAndSaveItemAsync(expectedValidPagesSummaryItemModel, cancellationToken).ConfigureAwait(false);

            // assert
            A.CallTo(() => fakeCmsApiService.GetItemAsync<CmsApiDataModel>(A<Uri>.Ignored)).MustHaveHappenedOnceExactly();
            A.CallTo(() => fakeMapper.Map<ContentPageModel>(A<CmsApiDataModel>.Ignored)).MustNotHaveHappened();
            A.CallTo(() => fakeEventMessageService.UpdateAsync(A<ContentPageModel>.Ignored)).MustNotHaveHappened();
            A.CallTo(() => fakeEventMessageService.CreateAsync(A<ContentPageModel>.Ignored)).MustNotHaveHappened();
            A.CallTo(() => fakeContentCacheService.AddOrReplace(A<Guid>.Ignored, A<List<Guid>>.Ignored, A<string>.Ignored)).MustNotHaveHappened();
        }

        [Fact]
        public async Task CacheReloadServiceDeleteStaleCacheEntriesIsSuccessful()
        {
            // arrange
            const int NumerOfSummaryItems = 2;
            const int NumberOfDeletions = 3;
            var cancellationToken = new CancellationToken(false);
            var fakePagesSummaryItemModels = BuldFakePagesSummaryItemModel(NumerOfSummaryItems);
            var fakeCachedContentPageModels = BuldFakeContentPageModels(NumberOfDeletions);

            A.CallTo(() => fakeEventMessageService.GetAllCachedItemsAsync()).Returns(fakeCachedContentPageModels);
            A.CallTo(() => fakeEventMessageService.DeleteAsync(A<Guid>.Ignored)).Returns(HttpStatusCode.OK);

            var cacheReloadService = new CacheReloadService(fakeLogger, fakeMapper, fakeEventMessageService, fakeCmsApiService, fakeContentCacheService, fakeAppRegistryService, fakeContentTypeMappingService);

            // act
            await cacheReloadService.DeleteStaleCacheEntriesAsync(fakePagesSummaryItemModels, cancellationToken).ConfigureAwait(false);

            // assert
            A.CallTo(() => fakeEventMessageService.GetAllCachedItemsAsync()).MustHaveHappenedOnceExactly();
            A.CallTo(() => fakeEventMessageService.DeleteAsync(A<Guid>.Ignored)).MustHaveHappened(NumberOfDeletions, Times.Exactly);
        }

        [Fact]
        public async Task CacheReloadServiceDeleteStaleCacheEntriesIsCancelled()
        {
            // arrange
            const int NumerOfSummaryItems = 2;
            const int NumberOfDeletions = 3;
            var cancellationToken = new CancellationToken(true);
            var fakePagesSummaryItemModels = BuldFakePagesSummaryItemModel(NumerOfSummaryItems);
            var fakeCachedContentPageModels = BuldFakeContentPageModels(NumberOfDeletions);

            A.CallTo(() => fakeEventMessageService.GetAllCachedItemsAsync()).Returns(fakeCachedContentPageModels);

            var cacheReloadService = new CacheReloadService(fakeLogger, fakeMapper, fakeEventMessageService, fakeCmsApiService, fakeContentCacheService, fakeAppRegistryService, fakeContentTypeMappingService);

            // act
            await cacheReloadService.DeleteStaleCacheEntriesAsync(fakePagesSummaryItemModels, cancellationToken).ConfigureAwait(false);

            // assert
            A.CallTo(() => fakeEventMessageService.GetAllCachedItemsAsync()).MustHaveHappenedOnceExactly();
            A.CallTo(() => fakeEventMessageService.DeleteAsync(A<Guid>.Ignored)).MustNotHaveHappened();
        }

        [Fact]
        public async Task CacheReloadServiceDeleteStaleItemsIsSuccessful()
        {
            // arrange
            const int NumberOfDeletions = 2;
            var cancellationToken = new CancellationToken(false);
            var fakeContentPageModels = BuldFakeContentPageModels(NumberOfDeletions);

            A.CallTo(() => fakeEventMessageService.DeleteAsync(A<Guid>.Ignored)).Returns(HttpStatusCode.OK);

            var cacheReloadService = new CacheReloadService(fakeLogger, fakeMapper, fakeEventMessageService, fakeCmsApiService, fakeContentCacheService, fakeAppRegistryService, fakeContentTypeMappingService);

            // act
            await cacheReloadService.DeleteStaleItemsAsync(fakeContentPageModels, cancellationToken).ConfigureAwait(false);

            // assert
            A.CallTo(() => fakeEventMessageService.DeleteAsync(A<Guid>.Ignored)).MustHaveHappened(NumberOfDeletions, Times.Exactly);
        }

        [Fact]
        public async Task CacheReloadServiceDeleteStaleItemsIsUnsuccessful()
        {
            // arrange
            const int NumberOfDeletions = 2;
            var cancellationToken = new CancellationToken(false);
            var fakeContentPageModels = BuldFakeContentPageModels(NumberOfDeletions);

            A.CallTo(() => fakeEventMessageService.DeleteAsync(A<Guid>.Ignored)).Returns(HttpStatusCode.NotFound);

            var cacheReloadService = new CacheReloadService(fakeLogger, fakeMapper, fakeEventMessageService, fakeCmsApiService, fakeContentCacheService, fakeAppRegistryService, fakeContentTypeMappingService);

            // act
            await cacheReloadService.DeleteStaleItemsAsync(fakeContentPageModels, cancellationToken).ConfigureAwait(false);

            // assert
            A.CallTo(() => fakeEventMessageService.DeleteAsync(A<Guid>.Ignored)).MustHaveHappened(NumberOfDeletions, Times.Exactly);
        }

        [Fact]
        public async Task CacheReloadServiceDeleteStaleItemsIsCancelled()
        {
            // arrange
            const int NumberOfDeletions = 2;
            var cancellationToken = new CancellationToken(true);
            var fakeContentPageModels = BuldFakeContentPageModels(NumberOfDeletions);

            var cacheReloadService = new CacheReloadService(fakeLogger, fakeMapper, fakeEventMessageService, fakeCmsApiService, fakeContentCacheService, fakeAppRegistryService, fakeContentTypeMappingService);

            // act
            await cacheReloadService.DeleteStaleItemsAsync(fakeContentPageModels, cancellationToken).ConfigureAwait(false);

            // assert
            A.CallTo(() => fakeEventMessageService.DeleteAsync(A<Guid>.Ignored)).MustNotHaveHappened();
        }

        [Fact]
        public async Task CacheReloadServicePostAppRegistryRefreshIsSuccessful()
        {
            // arrange
            var cacheReloadService = new CacheReloadService(fakeLogger, fakeMapper, fakeEventMessageService, fakeCmsApiService, fakeContentCacheService, fakeAppRegistryService, fakeContentTypeMappingService);

            // act
            await cacheReloadService.PostAppRegistryRefresh().ConfigureAwait(false);

            // assert
            A.CallTo(() => fakeAppRegistryService.PagesDataLoadAsync()).MustHaveHappenedOnceExactly();
        }

        [Theory]
        [MemberData(nameof(TestValidationData))]
        public void CacheReloadServiceTryValidateModelForValidAndInvalid(ContentPageModel contentPageModel, bool expectedResult)
        {
            // arrange
            var cacheReloadService = new CacheReloadService(fakeLogger, fakeMapper, fakeEventMessageService, fakeCmsApiService, fakeContentCacheService, fakeAppRegistryService, fakeContentTypeMappingService);

            // act
            var result = cacheReloadService.TryValidateModel(contentPageModel);

            // assert
            A.Equals(result, expectedResult);
        }

        private static CmsApiSummaryItemModel BuildValidPagesSummaryItemModel()
        {
            var model = new CmsApiSummaryItemModel()
            {
                Title = "an-article",
                Url = new Uri("/aaa/bbb", UriKind.Relative),
                Published = DateTime.Now,
                CreatedDate = DateTime.Now,
            };

            return model;
        }

        private static CmsApiDataModel BuildValidPagesApiDataModel()
        {
            var model = new CmsApiDataModel()
            {
                ItemId = Guid.NewGuid(),
                CanonicalName = "an-article",
                Version = Guid.NewGuid(),
                ExcludeFromSitemap = true,
                Url = new Uri("/aaa/bbb", UriKind.Relative),
                RedirectLocations = new List<string> { "alt-name-1", "alt-name-2" },
                Title = "A title",
                Description = "a description",
                Keywords = "some keywords",
                ContentLinks = new ContentLinksModel(new JObject())
                {
                    ContentLinks = new List<KeyValuePair<string, List<LinkDetails>>>()
                    {
                        new KeyValuePair<string, List<LinkDetails>>(
                            "test",
                            new List<LinkDetails>
                            {
                                new LinkDetails
                                {
                                    Uri = new Uri("http://www.one.com"),
                                },
                                new LinkDetails
                                {
                                    Uri = new Uri("http://www.two.com"),
                                },
                                new LinkDetails
                                {
                                    Uri = new Uri("http://www.three.com"),
                                },
                            }),
                    },
                },
                ContentItems = new List<IBaseContentItemModel>
                {
                    new CmsApiHtmlModel { Alignment = "Left", Ordinal = 1, Size = 50, Content = "<h1>A document</h1>", },
                },
                Published = DateTime.UtcNow,
            };

            return model;
        }

        private static ContentPageModel BuildValidContentPageModel()
        {
            var model = new ContentPageModel()
            {
                Id = Guid.NewGuid(),
                CanonicalName = "an-article",
                IncludeInSitemap = true,
                Version = Guid.NewGuid(),
                Url = new Uri("/aaa/bbb", UriKind.Relative),
                Content = "<h1>A document</h1>",
                ContentItems = new List<ContentItemModel>
                {
                    new ContentItemModel
                    {
                        ItemId = Guid.NewGuid(),
                        Alignment = "Left",
                        Ordinal = 1, Size = 50,
                        Content = "<h1>A document</h1>",
                        CreatedDate = DateTime.Now,
                        Title = "title",
                        ContentType = "content type",
                        HtmlBody = "body",
                        ContentItems = new List<ContentItemModel>
                        {
                            new ContentItemModel
                            {
                                CreatedDate = DateTime.Now,
                                Url = new Uri("http://www.test.com"),
                                Content = "content",
                                ItemId = Guid.NewGuid(),
                                LastReviewed = DateTime.Now,
                            },
                        },
                    },
                },
                LastReviewed = DateTime.UtcNow,
                CreatedDate = DateTime.UtcNow,
            };

            return model;
        }

        private List<CmsApiSummaryItemModel> BuldFakePagesSummaryItemModel(int iemCount)
        {
            var models = A.CollectionOfFake<CmsApiSummaryItemModel>(iemCount);

            foreach (var item in models)
            {
                var id = Guid.NewGuid();

                item.Url = new Uri($"http://somewhere.com/item/{id}", UriKind.Absolute);
            }

            return models.ToList();
        }

        private List<ContentPageModel> BuldFakeContentPageModels(int itemCount)
        {
            var models = A.CollectionOfFake<ContentPageModel>(itemCount);

            foreach (var item in models)
            {
                var id = Guid.NewGuid();

                item.Id = id;
                item.Url = new Uri($"http://somewhere.com/item/{id}", UriKind.Absolute);
                item.CanonicalName = id.ToString();
            }

            return models.ToList();
        }
    }
}

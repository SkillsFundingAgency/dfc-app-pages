using DFC.App.Pages.Data.Common;
using DFC.App.Pages.Data.Models;
using DFC.Compui.Cosmos.Contracts;
using FakeItEasy;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using Xunit;

namespace DFC.App.Pages.Services.EventProcessorService.UnitTests
{
    [Trait("Category", "Event Message Service Unit Tests")]
    public class EventMessageServiceTests
    {
        private readonly ILogger<EventMessageService<ContentPageModel>> fakeLogger = A.Fake<ILogger<EventMessageService<ContentPageModel>>>();
        private readonly IContentPageService<ContentPageModel> fakeContentPageService = A.Fake<IContentPageService<ContentPageModel>>();

        [Fact]
        public async Task EventMessageServiceGetAllCachedItemsReturnsSuccess()
        {
            // arrange
            var expectedResult = A.CollectionOfFake<ContentPageModel>(2);

            A.CallTo(() => fakeContentPageService.GetAllAsync()).Returns(expectedResult);

            var eventMessageService = new EventMessageService<ContentPageModel>(fakeLogger, fakeContentPageService);

            // act
            var result = await eventMessageService.GetAllCachedItemsAsync().ConfigureAwait(false);

            // assert
            A.CallTo(() => fakeContentPageService.GetAllAsync()).MustHaveHappenedOnceExactly();
            A.Equals(result, expectedResult);
        }

        [Fact]
        public async Task EventMessageServiceCreateAsyncReturnsSuccess()
        {
            // arrange
            ContentPageModel? existingContentPageModel = null;
            var contentPageModel = A.Fake<ContentPageModel>();
            var expectedResult = HttpStatusCode.OK;

            A.CallTo(() => fakeContentPageService.GetByIdAsync(A<Guid>.Ignored)).Returns(existingContentPageModel);
            A.CallTo(() => fakeContentPageService.UpsertAsync(A<ContentPageModel>.Ignored)).Returns(expectedResult);

            var eventMessageService = new EventMessageService<ContentPageModel>(fakeLogger, fakeContentPageService);

            // act
            var result = await eventMessageService.CreateAsync(contentPageModel).ConfigureAwait(false);

            // assert
            A.CallTo(() => fakeContentPageService.GetByIdAsync(A<Guid>.Ignored)).MustHaveHappenedOnceExactly();
            A.CallTo(() => fakeContentPageService.UpsertAsync(A<ContentPageModel>.Ignored)).MustHaveHappenedOnceExactly();
            A.Equals(result, expectedResult);
        }

        [Fact]
        public async Task EventMessageServiceCreateAsyncReturnsBadRequestWhenNullSupplied()
        {
            // arrange
            ContentPageModel? contentPageModel = null;
            var expectedResult = HttpStatusCode.BadRequest;

            var eventMessageService = new EventMessageService<ContentPageModel>(fakeLogger, fakeContentPageService);

            // act
            var result = await eventMessageService.CreateAsync(contentPageModel).ConfigureAwait(false);

            // assert
            A.CallTo(() => fakeContentPageService.GetByIdAsync(A<Guid>.Ignored)).MustNotHaveHappened();
            A.CallTo(() => fakeContentPageService.UpsertAsync(A<ContentPageModel>.Ignored)).MustNotHaveHappened();
            A.Equals(result, expectedResult);
        }

        [Fact]
        public async Task EventMessageServiceCreateAsyncReturnsAlreadyReportedWhenAlreadyExists()
        {
            // arrange
            var existingContentPageModel = A.Fake<ContentPageModel>();
            var contentPageModel = A.Fake<ContentPageModel>();
            var expectedResult = HttpStatusCode.AlreadyReported;

            A.CallTo(() => fakeContentPageService.GetByIdAsync(A<Guid>.Ignored)).Returns(existingContentPageModel);

            var eventMessageService = new EventMessageService<ContentPageModel>(fakeLogger, fakeContentPageService);

            // act
            var result = await eventMessageService.CreateAsync(contentPageModel).ConfigureAwait(false);

            // assert
            A.CallTo(() => fakeContentPageService.GetByIdAsync(A<Guid>.Ignored)).MustHaveHappenedOnceExactly();
            A.CallTo(() => fakeContentPageService.UpsertAsync(A<ContentPageModel>.Ignored)).MustNotHaveHappened();
            A.Equals(result, expectedResult);
        }

        [Fact]
        public async Task EventMessageServiceUpdateAsyncReturnsSuccessForSamePartitionKey()
        {
            // arrange
            var existingContentPageModel = A.Fake<ContentPageModel>();
            var contentPageModel = A.Fake<ContentPageModel>();
            var expectedResult = HttpStatusCode.OK;

            existingContentPageModel.Version = Guid.NewGuid();
            contentPageModel.Version = Guid.NewGuid();
            contentPageModel.PartitionKey = "a-partition-key";
            existingContentPageModel.PartitionKey = contentPageModel.PartitionKey;

            A.CallTo(() => fakeContentPageService.GetByIdAsync(A<Guid>.Ignored)).Returns(existingContentPageModel);
            A.CallTo(() => fakeContentPageService.UpsertAsync(A<ContentPageModel>.Ignored)).Returns(expectedResult);

            var eventMessageService = new EventMessageService<ContentPageModel>(fakeLogger, fakeContentPageService);

            // act
            var result = await eventMessageService.UpdateAsync(contentPageModel).ConfigureAwait(false);

            // assert
            A.CallTo(() => fakeContentPageService.GetByIdAsync(A<Guid>.Ignored)).MustHaveHappenedOnceExactly();
            A.CallTo(() => fakeContentPageService.DeleteAsync(A<Guid>.Ignored)).MustNotHaveHappened();
            A.CallTo(() => fakeContentPageService.UpsertAsync(A<ContentPageModel>.Ignored)).MustHaveHappenedOnceExactly();
            A.Equals(result, expectedResult);
        }

        [Fact]
        public async Task EventMessageServiceUpdateAsyncReturnsSuccessForDifferentPartitionKey()
        {
            // arrange
            var existingContentPageModel = A.Fake<ContentPageModel>();
            var contentPageModel = A.Fake<ContentPageModel>();
            var expectedResult = HttpStatusCode.Created;

            existingContentPageModel.Version = Guid.NewGuid();
            contentPageModel.Version = Guid.NewGuid();
            contentPageModel.PartitionKey = "a-partition-key";
            existingContentPageModel.PartitionKey = "a-different-partition-key";

            A.CallTo(() => fakeContentPageService.GetByIdAsync(A<Guid>.Ignored)).Returns(existingContentPageModel);
            A.CallTo(() => fakeContentPageService.DeleteAsync(A<Guid>.Ignored)).Returns(true);
            A.CallTo(() => fakeContentPageService.UpsertAsync(A<ContentPageModel>.Ignored)).Returns(expectedResult);

            var eventMessageService = new EventMessageService<ContentPageModel>(fakeLogger, fakeContentPageService);

            // act
            var result = await eventMessageService.UpdateAsync(contentPageModel).ConfigureAwait(false);

            // assert
            A.CallTo(() => fakeContentPageService.GetByIdAsync(A<Guid>.Ignored)).MustHaveHappenedOnceExactly();
            A.CallTo(() => fakeContentPageService.DeleteAsync(A<Guid>.Ignored)).MustHaveHappenedOnceExactly();
            A.CallTo(() => fakeContentPageService.UpsertAsync(A<ContentPageModel>.Ignored)).MustHaveHappenedOnceExactly();
            A.Equals(result, expectedResult);
        }

        [Fact]
        public async Task EventMessageServiceUpdateAsyncReturnsSuccessForDifferentPartitionKeyDeleteError()
        {
            // arrange
            var existingContentPageModel = A.Fake<ContentPageModel>();
            var contentPageModel = A.Fake<ContentPageModel>();
            var expectedResult = HttpStatusCode.Created;

            existingContentPageModel.Version = Guid.NewGuid();
            contentPageModel.Version = Guid.NewGuid();
            contentPageModel.PartitionKey = "a-partition-key";
            existingContentPageModel.PartitionKey = "a-different-partition-key";

            A.CallTo(() => fakeContentPageService.GetByIdAsync(A<Guid>.Ignored)).Returns(existingContentPageModel);
            A.CallTo(() => fakeContentPageService.DeleteAsync(A<Guid>.Ignored)).Returns(false);
            A.CallTo(() => fakeContentPageService.UpsertAsync(A<ContentPageModel>.Ignored)).Returns(expectedResult);

            var eventMessageService = new EventMessageService<ContentPageModel>(fakeLogger, fakeContentPageService);

            // act
            var result = await eventMessageService.UpdateAsync(contentPageModel).ConfigureAwait(false);

            // assert
            A.CallTo(() => fakeContentPageService.GetByIdAsync(A<Guid>.Ignored)).MustHaveHappenedOnceExactly();
            A.CallTo(() => fakeContentPageService.DeleteAsync(A<Guid>.Ignored)).MustHaveHappenedOnceExactly();
            A.CallTo(() => fakeContentPageService.UpsertAsync(A<ContentPageModel>.Ignored)).MustNotHaveHappened();
            A.Equals(result, expectedResult);
        }

        [Fact]
        public async Task EventMessageServiceUpdateAsyncReturnsBadRequestWhenNullSupplied()
        {
            // arrange
            ContentPageModel? contentPageModel = null;
            var expectedResult = HttpStatusCode.BadRequest;

            var eventMessageService = new EventMessageService<ContentPageModel>(fakeLogger, fakeContentPageService);

            // act
            var result = await eventMessageService.UpdateAsync(contentPageModel).ConfigureAwait(false);

            // assert
            A.CallTo(() => fakeContentPageService.GetByIdAsync(A<Guid>.Ignored)).MustNotHaveHappened();
            A.CallTo(() => fakeContentPageService.UpsertAsync(A<ContentPageModel>.Ignored)).MustNotHaveHappened();
            A.Equals(result, expectedResult);
        }

        [Fact]
        public async Task EventMessageServiceUpdateAsyncReturnsNotFoundWhenNotExists()
        {
            // arrange
            ContentPageModel? existingContentPageModel = null;
            var contentPageModel = A.Fake<ContentPageModel>();
            var expectedResult = HttpStatusCode.NotFound;

            A.CallTo(() => fakeContentPageService.GetByIdAsync(A<Guid>.Ignored)).Returns(existingContentPageModel);

            var eventMessageService = new EventMessageService<ContentPageModel>(fakeLogger, fakeContentPageService);

            // act
            var result = await eventMessageService.UpdateAsync(contentPageModel).ConfigureAwait(false);

            // assert
            A.CallTo(() => fakeContentPageService.GetByIdAsync(A<Guid>.Ignored)).MustHaveHappenedOnceExactly();
            A.CallTo(() => fakeContentPageService.UpsertAsync(A<ContentPageModel>.Ignored)).MustNotHaveHappened();
            A.Equals(result, expectedResult);
        }

        [Fact]
        public async Task EventMessageServiceDeleteAsyncReturnsSuccess()
        {
            // arrange
            var expectedResult = HttpStatusCode.OK;

            A.CallTo(() => fakeContentPageService.DeleteAsync(A<Guid>.Ignored)).Returns(true);

            var eventMessageService = new EventMessageService<ContentPageModel>(fakeLogger, fakeContentPageService);

            // act
            var result = await eventMessageService.DeleteAsync(Guid.NewGuid()).ConfigureAwait(false);

            // assert
            A.CallTo(() => fakeContentPageService.DeleteAsync(A<Guid>.Ignored)).MustHaveHappenedOnceExactly();
            A.Equals(result, expectedResult);
        }

        [Fact]
        public async Task EventMessageServiceDeleteAsyncReturnsNotFound()
        {
            // arrange
            var expectedResult = HttpStatusCode.NotFound;

            A.CallTo(() => fakeContentPageService.DeleteAsync(A<Guid>.Ignored)).Returns(false);

            var eventMessageService = new EventMessageService<ContentPageModel>(fakeLogger, fakeContentPageService);

            // act
            var result = await eventMessageService.DeleteAsync(Guid.NewGuid()).ConfigureAwait(false);

            // assert
            A.CallTo(() => fakeContentPageService.DeleteAsync(A<Guid>.Ignored)).MustHaveHappenedOnceExactly();
            A.Equals(result, expectedResult);
        }

        [Fact]
        public void EventMessageServiceExtractPageLocationReturnsSuccess()
        {
            // arrange
            var contentPageModel = BuildContentPageModelWithPageLocations();
            const string expectedResult = "/a/b/c";

            var eventMessageService = new EventMessageService<ContentPageModel>(fakeLogger, fakeContentPageService);

            // act
            var result = eventMessageService.ExtractPageLocation(contentPageModel);

            // assert
            A.Equals(expectedResult, result);
        }

        [Fact]
        public void EventMessageServiceExtractPageLocationReturnsNullForMissingModel()
        {
            // arrange
            ContentPageModel? contentPageModel = null;

            var eventMessageService = new EventMessageService<ContentPageModel>(fakeLogger, fakeContentPageService);

            // act
            var result = eventMessageService.ExtractPageLocation(contentPageModel);

            // assert
            Assert.Null(result);
        }

        [Fact]
        public void EventMessageServiceExtractPageLocationReturnsnullForMissingContentItems()
        {
            // arrange
            var contentPageModel = new ContentPageModel();

            var eventMessageService = new EventMessageService<ContentPageModel>(fakeLogger, fakeContentPageService);

            // act
            var result = eventMessageService.ExtractPageLocation(contentPageModel);

            // assert
            Assert.Null(result);
        }

        private ContentPageModel BuildContentPageModelWithPageLocations()
        {
            var model = new ContentPageModel
            {
                ContentItems = new List<ContentItemModel>
                {
                    new ContentItemModel
                    {
                        ContentType = Constants.ContentTypePageLocation,
                        BreadcrumbLinkSegment = "c",
                        ContentItems = new List<ContentItemModel>
                        {
                            new ContentItemModel
                            {
                                ContentType = Constants.ContentTypePageLocation,
                                BreadcrumbLinkSegment = null,
                                ContentItems = new List<ContentItemModel>
                                {
                                    new ContentItemModel
                                    {
                                        ContentType = Constants.ContentTypePageLocation,
                                        BreadcrumbLinkSegment = "b",
                                        ContentItems = new List<ContentItemModel>
                                        {
                                            new ContentItemModel
                                            {
                                                ContentType = Constants.ContentTypePageLocation,
                                                BreadcrumbLinkSegment = "a",
                                                ContentItems = new List<ContentItemModel>(),
                                            },
                                        },
                                    },
                                },
                            },
                        },
                    },
                },
            };

            return model;
        }
    }
}

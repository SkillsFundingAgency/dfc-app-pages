using AutoMapper;
using DFC.App.Pages.Controllers;
using DFC.App.Pages.Models.Api;
using FakeItEasy;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using Xunit;

namespace DFC.App.Pages.UnitTests.ControllerTests.ApiControllerTests
{
    public class ApiControllerApiTests
    {
        private readonly ILogger<ApiController> logger;
        private readonly IMapper fakeMapper;
        //private readonly IContentPageService<ContentPageModel> fakeContentPageService;

        public ApiControllerApiTests()
        {
            logger = A.Fake<ILogger<ApiController>>();
            fakeMapper = A.Fake<IMapper>();
            //fakeContentPageService = A.Fake<IContentPageService<ContentPageModel>>();
        }

        //TODO: Replace Cosmos call with Redis call
        /*[Fact]
        public async Task IndexWhenNoDateInApiReturnEmptyList()
        {
            // arrange
            List<ContentPageModel>? nullContentPageModels = null;
            A.CallTo(() => fakeContentPageService.GetAllAsync(A<string>.Ignored)).Returns(nullContentPageModels);

            using var controller = new ApiController(logger, fakeContentPageService, fakeMapper);

            // act
            var result = await controller.Index().ConfigureAwait(false) as OkObjectResult;

            // assert
            A.CallTo(() => fakeContentPageService.GetAllAsync(A<string>.Ignored)).MustHaveHappenedOnceExactly();
            A.CallTo(() => fakeMapper.Map<GetIndexModel>(A<ContentPageModel>.Ignored)).MustNotHaveHappened();

            Assert.NotNull(result);
            Assert.IsType<Dictionary<Guid, GetIndexModel>>(result!.Value);
            Assert.Empty(result.Value as Dictionary<Guid, GetIndexModel>);
        }

        //TODO: Replace Cosmos call with Redis call
        [Fact]
        public async Task IndexWhenNoDateInApiReturnsData()
        {
            // arrange
            var expectedContentPageModel1 = new ContentPageModel
            {
                Id = Guid.NewGuid(),
                CanonicalName = "test-test",
                PageLocation = "/top-of-the-tree",
                RedirectLocations = new List<string>
                    {
                        "/test/test",
                    },
                Url = new Uri("http://www.test.com"),
            };
            var expectedContentPageModel2 = new ContentPageModel
            {
                Id = Guid.NewGuid(),
                CanonicalName = "default-page",
                PageLocation = "/top-of-the-tree",
                IsDefaultForPageLocation = true,
                RedirectLocations = new List<string>
                    {
                        "/test/test",
                    },
                Url = new Uri("http://www.test.com"),
            };
            var expectedContentPageModels = new List<ContentPageModel> { expectedContentPageModel1, expectedContentPageModel2, };
            var expectedGetIndexModel1 = new GetIndexModel
            {
                Id = expectedContentPageModel1.Id,
                Locations = expectedContentPageModel1.RedirectLocations,
            };
            var expectedGetIndexModel2 = new GetIndexModel
            {
                Id = expectedContentPageModel2.Id,
                Locations = expectedContentPageModel2.RedirectLocations,
            };
            A.CallTo(() => fakeContentPageService.GetAllAsync(A<string>.Ignored)).Returns(expectedContentPageModels);

            using var controller = new ApiController(logger, fakeContentPageService, fakeMapper);

            A.CallTo(() => fakeMapper.Map<GetIndexModel>(expectedContentPageModel1)).Returns(expectedGetIndexModel1);
            A.CallTo(() => fakeMapper.Map<GetIndexModel>(expectedContentPageModel2)).Returns(expectedGetIndexModel2);

            // act
            var result = await controller.Index().ConfigureAwait(false) as OkObjectResult;

            // assert
            A.CallTo(() => fakeContentPageService.GetAllAsync(A<string>.Ignored)).MustHaveHappenedOnceExactly();
            A.CallTo(() => fakeMapper.Map<GetIndexModel>(expectedContentPageModel1)).MustHaveHappenedOnceExactly();
            A.CallTo(() => fakeMapper.Map<GetIndexModel>(expectedContentPageModel2)).MustHaveHappenedOnceExactly();

            Assert.NotNull(result);
            Assert.IsType<Dictionary<Guid, GetIndexModel>>(result!.Value);
            Assert.NotEmpty(result.Value as Dictionary<Guid, GetIndexModel>);
        }

        //TODO: Replace Cosmos call with Redis call
        [Fact]
        public async Task ApiControllerDocuemntReturnsSuccess()
        {
            // arrange
            const HttpStatusCode expectedStatusCode = HttpStatusCode.OK;
            var expectedContentPageModel = new ContentPageModel
            {
                Id = Guid.NewGuid(),
                CanonicalName = "test-test",
                RedirectLocations = new List<string>
                {
                    "/test/test",
                },
                Url = new Uri("http://www.test.com"),
            };
            var expectedGetIndexModel = new GetIndexModel
            {
                Locations = expectedContentPageModel.RedirectLocations,
            };
            A.CallTo(() => fakeContentPageService.GetByIdAsync(A<Guid>.Ignored, A<string>.Ignored)).Returns(expectedContentPageModel);
            A.CallTo(() => fakeMapper.Map<GetIndexModel>(expectedContentPageModel)).Returns(expectedGetIndexModel);

            using var controller = new ApiController(logger, fakeContentPageService, fakeMapper);

            // act
            var result = await controller.Document(expectedContentPageModel.Id).ConfigureAwait(false);

            // assert
            A.CallTo(() => fakeContentPageService.GetByIdAsync(A<Guid>.Ignored, A<string>.Ignored)).MustHaveHappenedOnceExactly();
            A.CallTo(() => fakeMapper.Map<GetIndexModel>(expectedContentPageModel)).MustHaveHappenedOnceExactly();

            var jsonResult = Assert.IsType<OkObjectResult>(result);
            var modelResult = Assert.IsAssignableFrom<GetIndexModel>(jsonResult.Value);
            Assert.Equal((int)expectedStatusCode, jsonResult.StatusCode);
            Assert.Equal(expectedGetIndexModel, modelResult);
        }

        //TODO: Replace Cosmos call with Redis call
        [Fact]
        public async Task ApiControllerDocuemntReturnsNoContentWhenNoData()
        {
            // arrange
            const HttpStatusCode expectedStatusCode = HttpStatusCode.NoContent;
            ContentPageModel? nullContentPageModel = null;
            A.CallTo(() => fakeContentPageService.GetByIdAsync(A<Guid>.Ignored, A<string>.Ignored)).Returns(nullContentPageModel);

            using var controller = new ApiController(logger, fakeContentPageService, fakeMapper);

            // act
            var result = await controller.Document(Guid.NewGuid()).ConfigureAwait(false);

            // assert
            A.CallTo(() => fakeContentPageService.GetByIdAsync(A<Guid>.Ignored, A<string>.Ignored)).MustHaveHappenedOnceExactly();
            A.CallTo(() => fakeMapper.Map<GetIndexModel>(A<ContentPageModel>.Ignored)).MustNotHaveHappened();

            var jsonResult = Assert.IsType<NoContentResult>(result);
            Assert.Equal(expectedStatusCode, (HttpStatusCode)jsonResult.StatusCode);
        }*/

        [Fact]
        public void ModelTest()
        {
            _ = new GetIndexModel
            {
                Locations = new List<string>(),
            };
        }
    }
}

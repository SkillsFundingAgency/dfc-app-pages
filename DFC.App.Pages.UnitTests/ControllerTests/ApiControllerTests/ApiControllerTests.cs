using AutoMapper;
using DFC.App.Pages.Controllers;
using DFC.App.Pages.Data.Models;
using DFC.App.Pages.Models.Api;
using DFC.Compui.Cosmos.Contracts;
using FakeItEasy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.VisualStudio.Web.CodeGeneration;
using System;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using Xunit;

namespace DFC.App.Pages.UnitTests.ControllerTests.ApiControllerTests
{
    public class ApiControllerTests
    {
        private readonly ILogger<ApiController> logger;
        private readonly IMapper fakeMapper;
        private readonly IContentPageService<ContentPageModel> fakeContentPageService;

        public ApiControllerTests()
        {
            logger = A.Fake<ILogger<ApiController>>();
            fakeMapper = A.Fake<AutoMapper.IMapper>();
            fakeContentPageService = A.Fake<IContentPageService<ContentPageModel>>();
        }

        [Fact]
        public async Task IndexWhenNoDateInApiReturnEmptyList()
        {
            using var controller = new ApiController(logger, fakeContentPageService, fakeMapper);
            var result = await controller.Index().ConfigureAwait(false) as OkObjectResult;

            A.CallTo(() => fakeContentPageService.GetAllAsync()).MustHaveHappenedOnceExactly();
            Assert.NotNull(result);
            Assert.IsType<List<GetIndexModel>>(result.Value);
            Assert.Empty(result.Value as List<GetIndexModel>);
        }

        [Fact]
        public async Task IndexWhenNoDateInApiReturnsData()
        {
            A.CallTo(() => fakeContentPageService.GetAllAsync()).Returns(new List<ContentPageModel>
            {
                new ContentPageModel
                {
                    CanonicalName = "test-test",
                    RedirectLocations = new List<string>
                    {
                        "/test/test",
                    },
                    Url = new Uri("http://www.test.com"),
                },
            });

            using var controller = new ApiController(logger, fakeContentPageService, fakeMapper);
            var result = await controller.Index().ConfigureAwait(false) as OkObjectResult;

            A.CallTo(() => fakeContentPageService.GetAllAsync()).MustHaveHappenedOnceExactly();
            Assert.NotNull(result);
            Assert.IsType<List<GetIndexModel>>(result!.Value);
            Assert.NotEmpty(result.Value as List<GetIndexModel>);
        }

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
                Id = expectedContentPageModel.Id,
                CanonicalName = expectedContentPageModel.CanonicalName,
                RedirectLocations = expectedContentPageModel.RedirectLocations,
                Url = expectedContentPageModel.Url,
            };
            A.CallTo(() => fakeContentPageService.GetByIdAsync(A<Guid>.Ignored)).Returns(expectedContentPageModel);
            A.CallTo(() => fakeMapper.Map<GetIndexModel>(expectedContentPageModel)).Returns(expectedGetIndexModel);

            using var controller = new ApiController(logger, fakeContentPageService, fakeMapper);

            // act
            var result = await controller.Document(expectedContentPageModel.Id).ConfigureAwait(false);

            // assert
            A.CallTo(() => fakeContentPageService.GetByIdAsync(A<Guid>.Ignored)).MustHaveHappenedOnceExactly();
            A.CallTo(() => fakeMapper.Map<GetIndexModel>(expectedContentPageModel)).MustHaveHappenedOnceExactly();

            var jsonResult = Assert.IsType<OkObjectResult>(result);
            var modelResult = Assert.IsAssignableFrom<GetIndexModel>(jsonResult.Value);
            Assert.Equal(expectedStatusCode, (HttpStatusCode)jsonResult.StatusCode);
            Assert.Equal(expectedGetIndexModel, modelResult);
        }

        [Fact]
        public async Task ApiControllerDocuemntReturnsNoContentWhenNoData()
        {
            // arrange
            const HttpStatusCode expectedStatusCode = HttpStatusCode.NoContent;
            ContentPageModel? nullContentPageModel = null;
            A.CallTo(() => fakeContentPageService.GetByIdAsync(A<Guid>.Ignored)).Returns(nullContentPageModel);

            using var controller = new ApiController(logger, fakeContentPageService, fakeMapper);

            // act
            var result = await controller.Document(Guid.NewGuid()).ConfigureAwait(false);

            // assert
            A.CallTo(() => fakeContentPageService.GetByIdAsync(A<Guid>.Ignored)).MustHaveHappenedOnceExactly();
            A.CallTo(() => fakeMapper.Map<GetIndexModel>(A<ContentPageModel>.Ignored)).MustNotHaveHappened();

            var jsonResult = Assert.IsType<NoContentResult>(result);
            Assert.Equal(expectedStatusCode, (HttpStatusCode)jsonResult.StatusCode);
        }

        [Fact]
        public void ModelTest()
        {
            _ = new GetIndexModel
            {
                Id = Guid.NewGuid(),
                CanonicalName = "test",
                RedirectLocations = new List<string>(),
                Url = new Uri("http://www.test.com"),
            };
        }
    }
}

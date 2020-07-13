using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AutoMapper;
using DFC.App.Pages.Controllers;
using DFC.App.Pages.Data.Models;
using DFC.App.Pages.Models.Api;
using DFC.Compui.Cosmos.Contracts;
using FakeItEasy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
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
        public async Task WhenNoDateInApiReturnEmptyList()
        {
            var controller = new ApiController(logger, fakeContentPageService, fakeMapper);
            var result = await controller.Index().ConfigureAwait(false) as OkObjectResult;

            Assert.NotNull(result);
            Assert.IsType<List<GetIndexModel>>(result.Value);
            Assert.Empty(result.Value as List<GetIndexModel>);
        }

        [Fact]
        public async Task WhenNoDateInApiReturnsData()
        {
            var ContentPageService = A.Fake<IContentPageService<ContentPageModel>>();

            A.CallTo(() => ContentPageService.GetAllAsync()).Returns(new List<ContentPageModel>
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

            var controller = new ApiController(logger, ContentPageService, fakeMapper);
            var result = await controller.Index().ConfigureAwait(false) as OkObjectResult;

            Assert.NotNull(result);
            Assert.IsType<List<GetIndexModel>>(result.Value);
            Assert.NotEmpty(result.Value as List<GetIndexModel>);
        }

        [Fact]
        public void ModelTest()
        {
            var model = new GetIndexModel
            {
                CanonicalName = "tets",
                RedirectLocations = new List<string>(),
                Url = new Uri("http://www.test.com"),
            };
        }
    }
}

using AutoMapper;
using DFC.App.Pages.Cms.Data.Content;
using DFC.App.Pages.Controllers;
using DFC.App.Pages.Models.Api;
using DFC.Common.SharedContent.Pkg.Netcore.Interfaces;
using DFC.Common.SharedContent.Pkg.Netcore.Model.ContentItems;
using DFC.Common.SharedContent.Pkg.Netcore.Model.Response;
using FakeItEasy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;
using System;
using System.Collections.Generic;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace DFC.App.Pages.UnitTests.ControllerTests.ApiControllerTests
{
    public class ApiControllerApiTests
    {
        private readonly ILogger<ApiController> logger;
        private readonly IMapper fakeMapper;
        private readonly ISharedContentRedisInterface sharedContentRedisInterface;

        public ApiControllerApiTests()
        {
            logger = A.Fake<ILogger<ApiController>>();
            fakeMapper = A.Fake<IMapper>();
            sharedContentRedisInterface = A.Fake<ISharedContentRedisInterface>();
        }

        [Fact]
        public async Task ApiControllerDocumentReturnsSuccess()
        {
            // arrange
            const HttpStatusCode expectedStatusCode = HttpStatusCode.OK;

            var expectedContentPageModel = new GetByPageApiResponse
            {
                Page = new List<PageApi>
                {
                    new PageApi()
                    {
                        DisplayText = "Test",
                        GraphSync = new()
                        {
                            NodeId = "6a2e6816-ee97-4b80-a6ef-c336cbb55adb",
                        },
                        PageLocation = new()
                        {
                            DefaultPageForLocation = false,
                            FullUrl = "Test",
                            UrlName = "test",
                        },
                    },
                },
            };

            Guid id = Guid.Parse("6a2e6816-ee97-4b80-a6ef-c336cbb55adb");

            var settings = new contentModeOptions()
            {
                contentMode = "contentMode",
                value = "PUBLISHED",
            };
            var monitor = Mock.Of<IOptionsMonitor<contentModeOptions>>(x => x.CurrentValue == settings);

            A.CallTo(() => sharedContentRedisInterface.GetDataAsync<GetByPageApiResponse>(A<string>.Ignored, A<string>.Ignored, A<double>.Ignored)).Returns(expectedContentPageModel);

            using var controller = new ApiController(logger, fakeMapper, sharedContentRedisInterface, monitor);

            // act
            var result = await controller.Document(id).ConfigureAwait(false);

            // assert
            var jsonResult = Assert.IsType<OkObjectResult>(result);
            var modelResult = Assert.IsAssignableFrom<GetIndexModel>(jsonResult.Value);
            Assert.Equal((int)expectedStatusCode, jsonResult.StatusCode);
        }

        [Fact]
        public async Task ApiControllerDocumentReturnsNoContentWhenNoData()
        {
            // arrange
            const HttpStatusCode expectedStatusCode = HttpStatusCode.NoContent;

            var settings = new contentModeOptions()
            {
                contentMode = "contentMode",
                value = "PUBLISHED",
            };
            var monitor = Mock.Of<IOptionsMonitor<contentModeOptions>>(x => x.CurrentValue == settings);

            A.CallTo(() => sharedContentRedisInterface.GetDataAsync<GetByPageApiResponse>(A<string>.Ignored, A<string>.Ignored, A<double>.Ignored)).Returns((GetByPageApiResponse)null);

            using var controller = new ApiController(logger, fakeMapper, sharedContentRedisInterface, monitor);

            // act
            var result = await controller.Document(Guid.NewGuid()).ConfigureAwait(false);

            // assert
            var jsonResult = Assert.IsType<NoContentResult>(result);
            Assert.Equal(expectedStatusCode, (HttpStatusCode)jsonResult.StatusCode);
        }

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

using AutoMapper;
using DFC.App.Pages.Cms.Data.Content;
using DFC.App.Pages.Controllers;
using DFC.App.Pages.Models.Api;
using DFC.Common.SharedContent.Pkg.Netcore.Interfaces;
using DFC.Common.SharedContent.Pkg.Netcore.Model.Common;
using DFC.Common.SharedContent.Pkg.Netcore.Model.ContentItems;
using DFC.Common.SharedContent.Pkg.Netcore.Model.Response;
using FakeItEasy;
using FluentAssertions;
using FluentAssertions.Events;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;
using NuGet.Protocol;
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
        private readonly IConfiguration configuration;

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

            var settings = new ContentModeOptions()
            {
                contentMode = "contentMode",
                value = "PUBLISHED",
            };
            var monitor = Mock.Of<IOptionsMonitor<ContentModeOptions>>(x => x.CurrentValue == settings);

            A.CallTo(() => sharedContentRedisInterface.GetDataAsyncWithExpiry<GetByPageApiResponse>(A<string>.Ignored, A<string>.Ignored, A<double>.Ignored)).Returns(expectedContentPageModel);

            using var controller = new ApiController(configuration, logger, fakeMapper, sharedContentRedisInterface, monitor);

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

            var settings = new ContentModeOptions()
            {
                contentMode = "contentMode",
                value = "PUBLISHED",
            };
            var monitor = Mock.Of<IOptionsMonitor<ContentModeOptions>>(x => x.CurrentValue == settings);

            A.CallTo(() => sharedContentRedisInterface.GetDataAsyncWithExpiry<GetByPageApiResponse>(A<string>.Ignored, A<string>.Ignored, A<double>.Ignored)).Returns((GetByPageApiResponse)null);

            using var controller = new ApiController(configuration, logger, fakeMapper, sharedContentRedisInterface, monitor);

            // act
            var result = await controller.Document(Guid.NewGuid()).ConfigureAwait(false);

            // assert
            var jsonResult = Assert.IsType<NoContentResult>(result);
            Assert.Equal(expectedStatusCode, (HttpStatusCode)jsonResult.StatusCode);
        }

        [Fact]
        public async Task TriageLookupResponseReturnsSuccess()
        {
            var redisMock = new Mock<ISharedContentRedisInterface>();
            var mapperMock = new Mock<IMapper>();
            var loggerMock = new Mock<ILogger<ApiController>>();
            var settings = new ContentModeOptions()
            {
                contentMode = "contentMode",
                value = "PUBLISHED",
            };
            var monitor = Mock.Of<IOptionsMonitor<ContentModeOptions>>(x => x.CurrentValue == settings);

            redisMock.Setup(r => r.GetDataAsyncWithExpiry<TriageLookupResponse>("TriageTool/Lookup", "contentMode", 4))
                     .ReturnsAsync(GetTriageLookupResponse());
            var controller = new ApiController(configuration,loggerMock.Object, mapperMock.Object, redisMock.Object, monitor);
            var result = await controller.TraigeLevelTwo();
            var objectResult = result as OkObjectResult;
            var triageLookup = objectResult.Value as TriageLookupResponse;
            triageLookup.Should().BeEquivalentTo(GetExpectedTriageLookupResponse());
        }

        [Fact]
        public async Task TriageLookupResponseReturnsNull()
        {
            var redisMock = new Mock<ISharedContentRedisInterface>();
            var mapperMock = new Mock<IMapper>();
            var loggerMock = new Mock<ILogger<ApiController>>();
            var settings = new ContentModeOptions()
            {
                contentMode = "contentMode",
                value = "PUBLISHED",
            };
            var monitor = Mock.Of<IOptionsMonitor<ContentModeOptions>>(x => x.CurrentValue == settings);

            redisMock.Setup(r => r.GetDataAsyncWithExpiry<TriageLookupResponse>("TriageTool/Lookup", "contentMode", 4))
                     .ReturnsAsync(new TriageLookupResponse());
            var controller = new ApiController(configuration, loggerMock.Object, mapperMock.Object, redisMock.Object, monitor);
            var result = await controller.TraigeLevelTwo();
            var objectResult = result as OkObjectResult;
            var triageLookup = objectResult.Value as TriageLookupResponse;
            triageLookup.FilterAdviceGroup.Should().BeNull();
            triageLookup.TriageLevelOne.Should().BeNull();
            triageLookup.TriageLevelTwo.Should().BeNull();
        }

        [Fact]
        public void ModelTest()
        {
            _ = new GetIndexModel
            {
                Locations = new List<string>(),
            };
        }

        private static TriageLookupResponse GetExpectedTriageLookupResponse()
        {
            var response = new TriageLookupResponse();
            response.TriageLevelOne = new List<TriageLevelOne>
            {
                new TriageLevelOne
                {
                    ContentItemId = "1",
                    Title = "Education",
                    Ordinal = 3,
                    Value = "Education",
                    LevelTwo = new TriageLevelTwo
                    {
                        ContentItems = new List<TriageLevelTwo>
                        {
                            new TriageLevelTwo
                            {
                                ContentItemId = "LevelTwo1",
                                Title = "LevelTwo1",
                                Value = "LevelTwo1",
                                FilterAdviceGroup = new FilterAdviceGroup
                                {
                                    ContentItems = new List<FilterAdviceGroup>
                                    {
                                        new FilterAdviceGroup { ContentItemId = "1", },
                                        new FilterAdviceGroup { ContentItemId = "2", },
                                    },
                                },
                            },
                            new TriageLevelTwo
                            {
                                ContentItemId = "LevelTwo2",
                                Title = "LevelTwo2",
                                Value = "LevelTwo2",
                                FilterAdviceGroup = new FilterAdviceGroup
                                {
                                    ContentItems = new List<FilterAdviceGroup>
                                    {
                                         new FilterAdviceGroup { ContentItemId = "1", },
                                         new FilterAdviceGroup { ContentItemId = "3", },
                                    },
                                },
                            },
                            new TriageLevelTwo
                            {
                                ContentItemId = "LevelTwo3",
                                Title = "LevelTwo3",
                                Value = "LevelTwo3",
                                FilterAdviceGroup = new FilterAdviceGroup
                                {
                                    ContentItems = new List<FilterAdviceGroup>
                                    {
                                          new FilterAdviceGroup { ContentItemId = "1", },
                                          new FilterAdviceGroup { ContentItemId = "2", },
                                    },
                                },
                            },
                        },
                    },
                },
                new TriageLevelOne
                {
                    ContentItemId = "2",
                    Title = "Employed",
                    Ordinal = 2,
                    Value = "Employed",
                    LevelTwo = new TriageLevelTwo
                    {
                        ContentItems = new List<TriageLevelTwo>
                        {
                            new TriageLevelTwo
                            {
                                ContentItemId = "LevelTwo4",
                                Title = "LevelTwo4",
                                Value = "LevelTwo4",
                                FilterAdviceGroup = new FilterAdviceGroup
                                {
                                    ContentItems = new List<FilterAdviceGroup>
                                    {
                                        new FilterAdviceGroup { ContentItemId = "1", },
                                        new FilterAdviceGroup { ContentItemId = "3", },
                                    },
                                },
                            },
                            new TriageLevelTwo
                            {
                                ContentItemId = "LevelTwo5",
                                Title = "LevelTwo5",
                                Value = "LevelTwo5",
                                FilterAdviceGroup = new FilterAdviceGroup
                                {
                                    ContentItems = new List<FilterAdviceGroup>
                                    {
                                       new FilterAdviceGroup { ContentItemId = "1", },
                                       new FilterAdviceGroup { ContentItemId = "4", },
                                    },
                                },
                            },
                            new TriageLevelTwo
                            {
                                ContentItemId = "LevelTwo6",
                                Title = "LevelTwo6",
                                Value = "LevelTwo6",
                                FilterAdviceGroup = new FilterAdviceGroup
                                {
                                    ContentItems = new List<FilterAdviceGroup>
                                    {
                                         new FilterAdviceGroup { ContentItemId = "1", },
                                         new FilterAdviceGroup { ContentItemId = "2", },
                                         new FilterAdviceGroup { ContentItemId = "3", },
                                         new FilterAdviceGroup { ContentItemId = "4", },
                                    },
                                },
                            },
                        },
                    },
                },
                new TriageLevelOne
                {
                    ContentItemId = "3",
                    Title = "Not In Work",
                    Ordinal = 1,
                    Value = "Not In Work",
                    LevelTwo = new TriageLevelTwo
                    {
                        ContentItems = new List<TriageLevelTwo>
                        {
                        new TriageLevelTwo
                        {
                            ContentItemId = "LevelTwo7",
                            Title = "LevelTwo7",
                            Value = "LevelTwo7",
                            FilterAdviceGroup = new FilterAdviceGroup
                            {
                                ContentItems = new List<FilterAdviceGroup>
                                {
                                     new FilterAdviceGroup { ContentItemId = "3", },
                                     new FilterAdviceGroup { ContentItemId = "4", },
                                },
                            },
                        },
                        new TriageLevelTwo
                        {
                            ContentItemId = "LevelTwo8",
                            Title = "LevelTwo8",
                            Value = "LevelTwo8",
                            FilterAdviceGroup = new FilterAdviceGroup
                            {
                                ContentItems = new List<FilterAdviceGroup>
                                {
                                    new FilterAdviceGroup { ContentItemId = "1", },
                                    new FilterAdviceGroup { ContentItemId = "2", },
                                    new FilterAdviceGroup { ContentItemId = "3", },
                                    new FilterAdviceGroup { ContentItemId = "4", },
                                },
                            },
                        },
                        new TriageLevelTwo
                        {
                            ContentItemId = "LevelTwo9",
                            Title = "LevelTwo9",
                            Value = "LevelTwo9",
                            FilterAdviceGroup = new FilterAdviceGroup
                            {
                                ContentItems = new List<FilterAdviceGroup>
                                {
                                   new FilterAdviceGroup { ContentItemId = "1", },
                                   new FilterAdviceGroup { ContentItemId = "2", },
                                   new FilterAdviceGroup { ContentItemId = "3", },
                                   new FilterAdviceGroup { ContentItemId = "4", },
                                },
                            },
                        },
                        },
                    },
                },
            };
            response.TriageLevelTwo = new List<TriageLevelTwo>
            {
                new TriageLevelTwo
                {
                    ContentItemId = "LevelTwo1",
                    Title = "LevelTwo1",
                    Value = "LevelTwo1",
                    FilterAdviceGroup = new FilterAdviceGroup
                    {
                        ContentItems = new List<FilterAdviceGroup>
                        {
                            new FilterAdviceGroup { ContentItemId = "1" },
                            new FilterAdviceGroup { ContentItemId = "2" },
                        },
                    },
                },
                new TriageLevelTwo
                {
                    ContentItemId = "LevelTwo2",
                    Title = "LevelTwo2",
                    Value = "LevelTwo2",
                    FilterAdviceGroup = new FilterAdviceGroup
                    {
                        ContentItems = new List<FilterAdviceGroup>
                        {
                            new FilterAdviceGroup { ContentItemId = "1" },
                            new FilterAdviceGroup { ContentItemId = "3" },
                        },
                    },
                },
                new TriageLevelTwo
                {
                    ContentItemId = "LevelTwo3",
                    Title = "LevelTwo3",
                    Value = "LevelTwo3",
                    FilterAdviceGroup = new FilterAdviceGroup
                    {
                        ContentItems = new List<FilterAdviceGroup>
                        {
                            new FilterAdviceGroup { ContentItemId = "1" },
                            new FilterAdviceGroup { ContentItemId = "2" },
                        },
                    },
                },
                new TriageLevelTwo
                {
                    ContentItemId = "LevelTwo4",
                    Title = "LevelTwo4",
                    Value = "LevelTwo4",
                    FilterAdviceGroup = new FilterAdviceGroup
                    {
                        ContentItems = new List<FilterAdviceGroup>
                        {
                            new FilterAdviceGroup { ContentItemId = "1" },
                            new FilterAdviceGroup { ContentItemId = "3" },
                        },
                    },
                },
                new TriageLevelTwo
                {
                    ContentItemId = "LevelTwo5",
                    Title = "LevelTwo5",
                    Value = "LevelTwo5",
                    FilterAdviceGroup = new FilterAdviceGroup
                    {
                        ContentItems = new List<FilterAdviceGroup>
                        {
                            new FilterAdviceGroup { ContentItemId = "1" },
                            new FilterAdviceGroup { ContentItemId = "4" },
                        },
                    },
                },
                new TriageLevelTwo
                {
                    ContentItemId = "LevelTwo6",
                    Title = "LevelTwo6",
                    Value = "LevelTwo6",
                    FilterAdviceGroup = new FilterAdviceGroup
                    {
                        ContentItems = new List<FilterAdviceGroup>
                        {
                            new FilterAdviceGroup { ContentItemId = "1" },
                            new FilterAdviceGroup { ContentItemId = "2" },
                            new FilterAdviceGroup { ContentItemId = "3" },
                            new FilterAdviceGroup { ContentItemId = "4" },
                        },
                    },
                },
                new TriageLevelTwo
                {
                    ContentItemId = "LevelTwo7",
                    Title = "LevelTwo7",
                    Value = "LevelTwo7",
                    FilterAdviceGroup = new FilterAdviceGroup
                    {
                        ContentItems = new List<FilterAdviceGroup>
                        {
                            new FilterAdviceGroup { ContentItemId = "3" },
                            new FilterAdviceGroup { ContentItemId = "4" },
                        },
                    },
                },
                new TriageLevelTwo
                {
                    ContentItemId = "LevelTwo8",
                    Title = "LevelTwo8",
                    Value = "LevelTwo8",
                    FilterAdviceGroup = new FilterAdviceGroup
                    {
                        ContentItems = new List<FilterAdviceGroup>
                        {
                            new FilterAdviceGroup { ContentItemId = "1" },
                            new FilterAdviceGroup { ContentItemId = "2" },
                            new FilterAdviceGroup { ContentItemId = "3" },
                            new FilterAdviceGroup { ContentItemId = "4" },
                        },
                    },
                },
                new TriageLevelTwo
                {
                    ContentItemId = "LevelTwo9",
                    Title = "LevelTwo9",
                    Value = "LevelTwo9",
                    FilterAdviceGroup = new FilterAdviceGroup
                    {
                        ContentItems = new List<FilterAdviceGroup>
                        {
                            new FilterAdviceGroup { ContentItemId = "1" },
                            new FilterAdviceGroup { ContentItemId = "2" },
                            new FilterAdviceGroup { ContentItemId = "3" },
                            new FilterAdviceGroup { ContentItemId = "4" },
                        },
                    },
                },
            };
            response.FilterAdviceGroup = new List<FilterAdviceGroup>
            {
                new FilterAdviceGroup { ContentItemId = "1", Title = "CV" },
                new FilterAdviceGroup { ContentItemId = "2", Title = "Options to work" },
                new FilterAdviceGroup { ContentItemId = "3", Title = "Interview tips" },
                new FilterAdviceGroup { ContentItemId = "4", Title = "Support from others" },
            };
            return response;
        }

        private static TriageLookupResponse GetTriageLookupResponse()
        {
            var response = new TriageLookupResponse();
            response.TriageLevelOne = new List<TriageLevelOne>
            {
                new TriageLevelOne
                {
                    ContentItemId = "1",
                    Title = "Education",
                    Ordinal = 3,
                    Value = "Education",
                    LevelTwo = new TriageLevelTwo
                    {
                        ContentItems = new List<TriageLevelTwo>
                        {
                                new TriageLevelTwo
                                {
                                     ContentItemId = "LevelTwo1"
                                },
                                new TriageLevelTwo
                                {
                                     ContentItemId = "LevelTwo2"
                                },
                                new TriageLevelTwo
                                {
                                     ContentItemId = "LevelTwo3"
                                },
                        },
                    },
                },
                new TriageLevelOne
                {
                    ContentItemId = "2",
                    Title = "Employed",
                    Ordinal = 2,
                    Value = "Employed",
                    LevelTwo = new TriageLevelTwo
                    {
                        ContentItems = new List<TriageLevelTwo>
                        {
                                new TriageLevelTwo
                                {
                                     ContentItemId = "LevelTwo4"
                                },
                                new TriageLevelTwo
                                {
                                     ContentItemId = "LevelTwo5"
                                },
                                new TriageLevelTwo
                                {
                                     ContentItemId = "LevelTwo6"
                                },
                        },
                    },
                },
                new TriageLevelOne
                {
                    ContentItemId = "3",
                    Title = "Not In Work",
                    Ordinal = 1,
                    Value = "Not In Work",
                    LevelTwo = new TriageLevelTwo
                    {
                        ContentItems = new List<TriageLevelTwo>
                        {
                                new TriageLevelTwo
                                {
                                     ContentItemId = "LevelTwo7"
                                },
                                new TriageLevelTwo
                                {
                                     ContentItemId = "LevelTwo8"
                                },
                                new TriageLevelTwo
                                {
                                     ContentItemId = "LevelTwo9"
                                },
                        },
                    },
                },
            };
            response.TriageLevelTwo = new List<TriageLevelTwo>
            {
                new TriageLevelTwo
                {
                    ContentItemId = "LevelTwo1",
                    Title = "LevelTwo1",
                    Value = "LevelTwo1",
                    FilterAdviceGroup = new FilterAdviceGroup
                    {
                        ContentItems = new List<FilterAdviceGroup>
                        {
                            new FilterAdviceGroup { ContentItemId = "1" },
                            new FilterAdviceGroup { ContentItemId = "2" },
                        },
                    },
                },
                new TriageLevelTwo
                {
                    ContentItemId = "LevelTwo2",
                    Title = "LevelTwo2",
                    Value = "LevelTwo2",
                    FilterAdviceGroup = new FilterAdviceGroup
                    {
                        ContentItems = new List<FilterAdviceGroup>
                        {
                            new FilterAdviceGroup { ContentItemId = "1" },
                            new FilterAdviceGroup { ContentItemId = "3" },
                        },
                    },
                },
                new TriageLevelTwo
                {
                    ContentItemId = "LevelTwo3",
                    Title = "LevelTwo3",
                    Value = "LevelTwo3",
                    FilterAdviceGroup = new FilterAdviceGroup
                    {
                        ContentItems = new List<FilterAdviceGroup>
                        {
                            new FilterAdviceGroup { ContentItemId = "1" },
                            new FilterAdviceGroup { ContentItemId = "2" },
                        },
                    },
                },
                new TriageLevelTwo
                {
                    ContentItemId = "LevelTwo4",
                    Title = "LevelTwo4",
                    Value = "LevelTwo4",
                    FilterAdviceGroup = new FilterAdviceGroup
                    {
                        ContentItems = new List<FilterAdviceGroup>
                        {
                            new FilterAdviceGroup { ContentItemId = "1" },
                            new FilterAdviceGroup { ContentItemId = "3" },
                        },
                    },
                },
                new TriageLevelTwo
                {
                    ContentItemId = "LevelTwo5",
                    Title = "LevelTwo5",
                    Value = "LevelTwo5",
                    FilterAdviceGroup = new FilterAdviceGroup
                    {
                        ContentItems = new List<FilterAdviceGroup>
                        {
                            new FilterAdviceGroup { ContentItemId = "1" },
                            new FilterAdviceGroup { ContentItemId = "4" },
                        },
                    },
                },
                new TriageLevelTwo
                {
                    ContentItemId = "LevelTwo6",
                    Title = "LevelTwo6",
                    Value = "LevelTwo6",
                    FilterAdviceGroup = new FilterAdviceGroup
                    {
                        ContentItems = new List<FilterAdviceGroup>
                        {
                            new FilterAdviceGroup { ContentItemId = "1" },
                            new FilterAdviceGroup { ContentItemId = "2" },
                            new FilterAdviceGroup { ContentItemId = "3" },
                            new FilterAdviceGroup { ContentItemId = "4" },
                        },
                    },
                },
                new TriageLevelTwo
                {
                    ContentItemId = "LevelTwo7",
                    Title = "LevelTwo7",
                    Value = "LevelTwo7",
                    FilterAdviceGroup = new FilterAdviceGroup
                    {
                        ContentItems = new List<FilterAdviceGroup>
                        {
                            new FilterAdviceGroup { ContentItemId = "3" },
                            new FilterAdviceGroup { ContentItemId = "4" },
                        },
                    },
                },
                new TriageLevelTwo
                {
                    ContentItemId = "LevelTwo8",
                    Title = "LevelTwo8",
                    Value = "LevelTwo8",
                    FilterAdviceGroup = new FilterAdviceGroup
                    {
                        ContentItems = new List<FilterAdviceGroup>
                        {
                            new FilterAdviceGroup { ContentItemId = "1" },
                            new FilterAdviceGroup { ContentItemId = "2" },
                            new FilterAdviceGroup { ContentItemId = "3" },
                            new FilterAdviceGroup { ContentItemId = "4" },
                        },
                    },
                },
                new TriageLevelTwo
                {
                    ContentItemId = "LevelTwo9",
                    Title = "LevelTwo9",
                    Value = "LevelTwo9",
                    FilterAdviceGroup = new FilterAdviceGroup
                    {
                        ContentItems = new List<FilterAdviceGroup>
                        {
                            new FilterAdviceGroup { ContentItemId = "1" },
                            new FilterAdviceGroup { ContentItemId = "2" },
                            new FilterAdviceGroup { ContentItemId = "3" },
                            new FilterAdviceGroup { ContentItemId = "4" },
                        },
                    },
                },
            };
            response.FilterAdviceGroup = new List<FilterAdviceGroup>
            {
                new FilterAdviceGroup { ContentItemId = "1", Title = "CV"},
                new FilterAdviceGroup { ContentItemId = "2", Title = "Options to work" },
                new FilterAdviceGroup { ContentItemId = "3", Title = "Interview tips" },
                new FilterAdviceGroup { ContentItemId = "4", Title = "Support from others" },
            };
            return response;
        }
    }
}

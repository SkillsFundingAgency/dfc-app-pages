using AutoMapper;
using DFC.App.Pages.Cms.Data.Content;
using DFC.App.Pages.Controllers;
using DFC.App.Pages.Data.Contracts;
using DFC.Common.SharedContent.Pkg.Netcore.Interfaces;
using FakeItEasy;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.Net.Http.Headers;
using System.Collections.Generic;
using System.Net.Mime;

namespace DFC.App.Pages.UnitTests.ControllerTests.PagesControllerTests
{
    public abstract class BasePagesControllerTests
    {
        protected BasePagesControllerTests()
        {
            FakePagesControllerHelper = A.Fake<IPagesControllerHelper>();
            Logger = A.Fake<ILogger<PagesController>>();
            FakeMapper = A.Fake<IMapper>();
            FakeSharedContentRedisInterface =A.Fake<ISharedContentRedisInterface>();
            FakeContentOptions = A.Fake<IOptionsMonitor<contentModeOptions>>();
        }

        public static IEnumerable<object[]> HtmlMediaTypes => new List<object[]>
        {
            new string[] { "*/*" },
            new string[] { MediaTypeNames.Text.Html },
        };

        public static IEnumerable<object[]> InvalidMediaTypes => new List<object[]>
        {
            new string[] { MediaTypeNames.Text.Plain },
        };

        public static IEnumerable<object[]> JsonMediaTypes => new List<object[]>
        {
            new string[] { MediaTypeNames.Application.Json },
        };

        protected IPagesControllerHelper FakePagesControllerHelper { get; }

        protected ILogger<PagesController> Logger { get; }

        protected IMapper FakeMapper { get; }

        protected ISharedContentRedisInterface FakeSharedContentRedisInterface { get; }

        protected IOptionsMonitor<contentModeOptions> FakeContentOptions { get; }

        protected PagesController BuildPagesController(string mediaTypeName)
        {
            var httpContext = new DefaultHttpContext();

            httpContext.Request.Headers[HeaderNames.Accept] = mediaTypeName;

            var controller = new PagesController(FakePagesControllerHelper, Logger, FakeMapper, FakeSharedContentRedisInterface, FakeContentOptions)
            {
                ControllerContext = new ControllerContext()
                {
                    HttpContext = httpContext,
                },
            };

            return controller;
        }
    }
}

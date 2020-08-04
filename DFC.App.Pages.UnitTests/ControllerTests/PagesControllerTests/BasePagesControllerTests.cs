using DFC.App.Pages.Controllers;
using DFC.App.Pages.Data.Contracts;
using DFC.App.Pages.Data.Models;
using DFC.Compui.Cosmos.Contracts;
using FakeItEasy;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Net.Http.Headers;
using System.Collections.Generic;
using System.Net.Mime;

namespace DFC.App.Pages.UnitTests.ControllerTests.PagesControllerTests
{
    public abstract class BasePagesControllerTests
    {
        protected BasePagesControllerTests()
        {
            Logger = A.Fake<ILogger<PagesController>>();
            FakeContentPageService = A.Fake<IContentPageService<ContentPageModel>>();
            FakeMapper = A.Fake<AutoMapper.IMapper>();
            FakePagesControlerHelpers = A.Fake<IPagesControlerHelpers>();
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

        protected ILogger<PagesController> Logger { get; }

        protected IContentPageService<ContentPageModel> FakeContentPageService { get; }

        protected AutoMapper.IMapper FakeMapper { get; }

        protected IPagesControlerHelpers FakePagesControlerHelpers { get; }

        protected PagesController BuildPagesController(string mediaTypeName)
        {
            var httpContext = new DefaultHttpContext();

            httpContext.Request.Headers[HeaderNames.Accept] = mediaTypeName;

            var controller = new PagesController(Logger, FakeContentPageService, FakeMapper, FakePagesControlerHelpers)
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

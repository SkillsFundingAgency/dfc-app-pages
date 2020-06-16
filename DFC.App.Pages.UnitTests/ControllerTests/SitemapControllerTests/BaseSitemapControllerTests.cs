using DFC.App.Pages.Controllers;
using DFC.App.Pages.Data.Models;
using DFC.Compui.Cosmos.Contracts;
using FakeItEasy;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace DFC.App.Pages.UnitTests.ControllerTests.SitemapControllerTests
{
    public class BaseSitemapControllerTests
    {
        public BaseSitemapControllerTests()
        {
            FakeLogger = A.Fake<ILogger<SitemapController>>();
            FakeContentPageService = A.Fake<IContentPageService<ContentPageModel>>();
        }

        protected ILogger<SitemapController> FakeLogger { get; }

        protected IContentPageService<ContentPageModel> FakeContentPageService { get; }

        protected SitemapController BuildSitemapController()
        {
            var controller = new SitemapController(FakeLogger, FakeContentPageService)
            {
                ControllerContext = new ControllerContext()
                {
                    HttpContext = new DefaultHttpContext(),
                },
            };

            return controller;
        }
    }
}

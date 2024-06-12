﻿using DFC.App.Pages.Cms.Data.Content;
using DFC.App.Pages.Controllers;
using DFC.Common.SharedContent.Pkg.Netcore.Interfaces;
using FakeItEasy;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace DFC.App.Pages.UnitTests.ControllerTests.SitemapControllerTests
{
    public class BaseSitemapControllerTests
    {
        public BaseSitemapControllerTests()
        {
            FakeLogger = A.Fake<ILogger<SitemapController>>();
            FakeSharedContentRedisInterface = A.Fake<ISharedContentRedisInterface>();
            FakeContentOptions = A.Fake<IOptionsMonitor<ContentModeOptions>>();

        }

        protected ILogger<SitemapController> FakeLogger { get; }

        protected IOptionsMonitor<ContentModeOptions> FakeContentOptions { get; }


        protected ISharedContentRedisInterface FakeSharedContentRedisInterface { get; }


        protected SitemapController BuildSitemapController()
        {
            var controller = new SitemapController(FakeLogger, FakeSharedContentRedisInterface, FakeContentOptions)
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

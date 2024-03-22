using DFC.App.Pages.Controllers;
using FakeItEasy;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Extensions.Logging;
using Microsoft.Net.Http.Headers;
using System;
using System.Collections.Generic;
using System.Net.Mime;
using System.Threading.Tasks;

namespace DFC.App.Pages.UnitTests.ControllerTests.HealthControllerTests
{
    public class BaseHealthControllerTests
    {
        public BaseHealthControllerTests()
        {
            FakeLogger = A.Fake<ILogger<HealthController>>();
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

        protected ILogger<HealthController> FakeLogger { get; }

        protected HealthCheckService CreateHealthChecksService(Action<IHealthChecksBuilder> configure)
        {
            var services = new ServiceCollection();
            services.AddLogging();
            services.AddOptions();

            var builder = services.AddHealthChecks();
            configure?.Invoke(builder);

            return services.BuildServiceProvider(validateScopes: true).GetRequiredService<HealthCheckService>();
        }

        protected HealthController BuildHealthController(string mediaTypeName, HealthCheckService healthCheckService)
        {
            var httpContext = new DefaultHttpContext();

            httpContext.Request.Headers[HeaderNames.Accept] = mediaTypeName;

            var controller = new HealthController(FakeLogger, healthCheckService)
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

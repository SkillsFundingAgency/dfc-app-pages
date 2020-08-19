using DFC.App.Pages.Controllers;
using FakeItEasy;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace DFC.App.Pages.UnitTests.ControllerTests.RobotControllerTests
{
    public class BaseRobotControllerTests
    {
        public BaseRobotControllerTests()
        {
            FakeLogger = A.Fake<ILogger<RobotController>>();
            FakeHostingEnvironment = A.Fake<IWebHostEnvironment>();
        }

        protected ILogger<RobotController> FakeLogger { get; }

        protected IWebHostEnvironment FakeHostingEnvironment { get; }

        protected RobotController BuildRobotController()
        {
            var controller = new RobotController(FakeLogger, FakeHostingEnvironment)
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

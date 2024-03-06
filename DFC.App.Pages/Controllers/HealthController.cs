using DFC.App.Pages.Data.Models;
using DFC.App.Pages.Extensions;
using DFC.App.Pages.ViewModels;
using DFC.Compui.Cosmos.Contracts;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;

namespace DFC.App.Pages.Controllers
{
    public class HealthController : Controller
    {
        public const string HealthViewCanonicalName = "health";

        private readonly ILogger<HealthController> logger;
        private readonly string resourceName = typeof(Program).Namespace!;

        public HealthController(ILogger<HealthController> logger)
        {
            this.logger = logger;
        }

        [HttpGet]
        [Route("pages/health")]
        public async Task<IActionResult> HealthView()
        {
            var result = await Health().ConfigureAwait(false);

            return result;
        }

        [HttpGet]
        [Route("health")]
        public async Task<IActionResult> Health()
        {
            logger.LogInformation($"{nameof(Health)} has been called");
            const string message = "Document sotre is available";
            logger.LogInformation($"{nameof(Health)} responded with: {resourceName} - {message}");

            var viewModel = CreateHealthViewModel(message);
            logger.LogInformation("Generated Health report");


            return this.NegotiateContentResult(viewModel, viewModel.HealthItems);
        }

        [HttpGet]
        [Route("health/ping")]
        public IActionResult Ping()
        {
            logger.LogInformation($"{nameof(Ping)} has been called");

            return Ok();
        }

        private HealthViewModel CreateHealthViewModel(string message)
        {
            return new HealthViewModel
            {
                HealthItems = new List<HealthItemViewModel>
                {
                    new HealthItemViewModel
                    {
                        Service = resourceName,
                        Message = message,
                    },
                },
            };
        }
    }
}
using DFC.App.Pages.Extensions;
using DFC.App.Pages.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
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

        //TODO: Add Redis and GraphQl ping
        [HttpGet]
        [Route("health")]
        public async Task<IActionResult> Health()
        {
            logger.LogInformation($"{nameof(Health)} has been called");

            const string message = "Document store is available";
            var viewModel = CreateHealthViewModel(message);

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
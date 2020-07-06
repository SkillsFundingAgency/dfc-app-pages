using DFC.App.Pages.Data.Contracts;
using DFC.App.Pages.Data.Models;
using DFC.Compui.Telemetry.HostedService;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System.Diagnostics.CodeAnalysis;
using System.Threading;
using System.Threading.Tasks;

namespace DFC.App.Pages.HostedServices
{
    [ExcludeFromCodeCoverage]
    public class CreateSubscriptionBackgroundService : BackgroundService
    {
        private readonly ILogger<CreateSubscriptionBackgroundService> logger;
        private readonly IEventGridSubscriptionService eventGridSubscriptionService;
        private readonly IHostedServiceTelemetryWrapper hostedServiceTelemetryWrapper;

        public CreateSubscriptionBackgroundService(ILogger<CreateSubscriptionBackgroundService> logger, IEventGridSubscriptionService eventGridSubscriptionService, IHostedServiceTelemetryWrapper hostedServiceTelemetryWrapper)
        {
            this.logger = logger;
            this.eventGridSubscriptionService = eventGridSubscriptionService;
            this.hostedServiceTelemetryWrapper = hostedServiceTelemetryWrapper;
        }

        public override Task StartAsync(CancellationToken cancellationToken)
        {
            logger.LogInformation("Event subscription create started");

            return base.StartAsync(cancellationToken);
        }

        public override Task StopAsync(CancellationToken cancellationToken)
        {
            logger.LogInformation("Event subscription create stopped");

            return base.StopAsync(cancellationToken);
        }

        protected override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            logger.LogInformation("Event subscription create executing");

            var task = hostedServiceTelemetryWrapper.Execute(() => eventGridSubscriptionService.CreateAsync(), nameof(CreateSubscriptionBackgroundService));

            logger.LogInformation("Event subscription create execute");

            return task;
        }
    }
}

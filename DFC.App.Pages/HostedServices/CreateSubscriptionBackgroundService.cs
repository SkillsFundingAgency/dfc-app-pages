using DFC.App.Pages.Data.Contracts;
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

        protected async override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            logger.LogInformation("Event subscription create executing");

            await Task.Delay(System.TimeSpan.FromSeconds(120)).ConfigureAwait(false);

            var task = hostedServiceTelemetryWrapper.Execute(() => eventGridSubscriptionService.CreateAsync(), nameof(CreateSubscriptionBackgroundService));

            if (!task.IsCompletedSuccessfully)
            {
                logger.LogInformation("Event subscription create didn't complete successfully");
                if (task.Exception != null)
                {
                    logger.LogError(task.Exception.ToString());
                    throw task.Exception;
                }
            }

            return;
        }
    }
}

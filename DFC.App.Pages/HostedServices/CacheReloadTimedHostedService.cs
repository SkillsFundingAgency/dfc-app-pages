using DFC.App.Pages.Data.Contracts;
using DFC.App.Pages.Models;
using DFC.Compui.Telemetry.HostedService;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Diagnostics.CodeAnalysis;
using System.Threading;
using System.Threading.Tasks;

namespace DFC.App.Pages.HostedServices
{
    [ExcludeFromCodeCoverage]
    public class CacheReloadTimedHostedService : IHostedService, IDisposable
    {
        private readonly ILogger<CacheReloadTimedHostedService> logger;
        private readonly ICacheReloadService cacheReloadService;
        private readonly CacheReloadTimerOptions cacheReloadTimerOptions;
        private readonly IHostedServiceTelemetryWrapper hostedServiceTelemetryWrapper;
        private Timer? timer;
        private bool disposedValue;

        public CacheReloadTimedHostedService(ILogger<CacheReloadTimedHostedService> logger, ICacheReloadService cacheReloadService, CacheReloadTimerOptions cacheReloadTimerOptions, IHostedServiceTelemetryWrapper hostedServiceTelemetryWrapper)
        {
            this.logger = logger;
            this.cacheReloadService = cacheReloadService;
            this.cacheReloadTimerOptions = cacheReloadTimerOptions;
            this.hostedServiceTelemetryWrapper = hostedServiceTelemetryWrapper;
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            if (cacheReloadTimerOptions.Enabled)
            {
                logger.LogInformation("Timed cache reload background service is starting.");

                timer = new Timer(DoWork, null, cacheReloadTimerOptions.DelayStart, cacheReloadTimerOptions.Interval);
            }
            else
            {
                logger.LogInformation("Timed cache reload background service is not enabled.");
            }

            return Task.CompletedTask;
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            logger.LogInformation("Timed cache reload background service is stopping.");

            timer?.Change(Timeout.Infinite, 0);

            return Task.CompletedTask;
        }

        public void Dispose()
        {
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    timer?.Dispose();
                }

                disposedValue = true;
            }
        }

        private void DoWork(object? state)
        {
            logger.LogInformation("Timed cache reload background service is working.");

            var task = hostedServiceTelemetryWrapper.Execute(() => cacheReloadService.Reload(new CancellationToken(false)), nameof(CacheReloadTimedHostedService));

            if (!task.IsCompletedSuccessfully)
            {
                logger.LogInformation("Timed cache reload didn't complete successfully");
                if (task.Exception != null)
                {
                    logger.LogError(task.Exception.ToString());
                    throw task.Exception;
                }
            }

            logger.LogInformation("Timed cache reload background service has finished.");
        }
    }
}

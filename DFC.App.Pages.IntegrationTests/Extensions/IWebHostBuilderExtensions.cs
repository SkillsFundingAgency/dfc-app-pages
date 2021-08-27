using DFC.App.Pages.Data.Contracts;
using DFC.App.Pages.Data.Models;
using DFC.App.Pages.IntegrationTests.Fakes;
using DFC.Compui.Cosmos.Contracts;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;

namespace DFC.App.Pages.IntegrationTests.Extensions
{
    public static class IWebHostBuilderExtensions
    {
        public static IWebHostBuilder RegisterServices(
            this IWebHostBuilder webHostBuilder, 
            ICosmosRepository<ContentPageModel> cosmosRepository, 
            IContentPageService<ContentPageModel> contentPageService)
        {
            return webHostBuilder.ConfigureTestServices(services =>
            {
                services.AddTransient(sp => cosmosRepository);
                services.AddTransient(sp => contentPageService);
                services.AddTransient<ICacheReloadService, FakeCacheReloadService>();
                services.AddHostedService<FakeCacheReloadBackgroundService>();
            });
        }
    }
}

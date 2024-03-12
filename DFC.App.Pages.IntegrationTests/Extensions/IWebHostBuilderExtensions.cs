using DFC.Common.SharedContent.Pkg.Netcore.Interfaces;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;

namespace DFC.App.Pages.IntegrationTests.Extensions
{
    public static class IWebHostBuilderExtensions
    {
        public static IWebHostBuilder RegisterServices(
            this IWebHostBuilder webHostBuilder, ISharedContentRedisInterface sharedContentRedisInterface)
        {
            return webHostBuilder.ConfigureTestServices(services =>
            {
                services.AddScoped(_ => sharedContentRedisInterface);
            });
        }
    }
}

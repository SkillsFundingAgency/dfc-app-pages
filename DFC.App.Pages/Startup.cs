using AutoMapper;
using DFC.App.Pages.Data.Contracts;
using DFC.App.Pages.Data.Models;
using DFC.App.Pages.Data.Models.ClientOptions;
using DFC.App.Pages.Data.Models.CmsApiModels;
using DFC.App.Pages.Extensions;
using DFC.App.Pages.Helpers;
using DFC.App.Pages.HostedServices;
using DFC.App.Pages.HttpClientPolicies;
using DFC.App.Pages.Models;
using DFC.App.Pages.Services.AppRegistryService;
using DFC.App.Pages.Services.CacheContentService;
using DFC.App.Pages.Services.CacheContentService.ContentItemUpdaters;
using DFC.App.Pages.Services.EventProcessorService;
using DFC.Compui.Cosmos;
using DFC.Compui.Cosmos.Contracts;
using DFC.Compui.Subscriptions.Pkg.Netstandard.Extensions;
using DFC.Compui.Telemetry;
using DFC.Content.Pkg.Netcore.Data.Models.ClientOptions;
using DFC.Content.Pkg.Netcore.Extensions;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Documents.Client;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System.Diagnostics.CodeAnalysis;

namespace DFC.App.Pages
{
    [ExcludeFromCodeCoverage]
    public class Startup
    {
        private const string CosmosDbContentPagesConfigAppSettings = "Configuration:CosmosDbConnections:ContentPages";

        private readonly IConfiguration configuration;
        private readonly IWebHostEnvironment env;

        public Startup(IConfiguration configuration, IWebHostEnvironment env)
        {
            this.configuration = configuration;
            this.env = env;
        }

        public static void Configure(IApplicationBuilder app, IWebHostEnvironment env, IMapper mapper)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();
            app.UseRouting();
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapRazorPages();

                // add the default route
                endpoints.MapControllerRoute("default", "{controller=Health}/{action=Ping}");
            });
            mapper?.ConfigurationProvider.AssertConfigurationIsValid();
        }

        public void ConfigureServices(IServiceCollection services)
        {
            var cosmosDbConnectionContentPages = configuration.GetSection(CosmosDbContentPagesConfigAppSettings).Get<CosmosDbConnection>();
            var cosmosRetryOptions = new RetryOptions { MaxRetryAttemptsOnThrottledRequests = 20, MaxRetryWaitTimeInSeconds = 60 };
            services.AddContentPageServices<ContentPageModel>(cosmosDbConnectionContentPages, env.IsDevelopment(), cosmosRetryOptions);

            services.AddApplicationInsightsTelemetry();
            services.AddHttpContextAccessor();
            services.AddTransient<IPagesControlerHelpers, PagesControlerHelpers>();
            services.AddTransient<IEventMessageService<ContentPageModel>, EventMessageService<ContentPageModel>>();
            services.AddTransient<ICacheReloadService, CacheReloadService>();
            services.AddTransient<IWebhooksService, WebhooksService>();
            services.AddTransient<IWebhookContentProcessor, WebhookContentProcessor>();
            services.AddTransient<IPageLocatonUpdater, PageLocatonUpdater>();
            services.AddTransient<IContentItemUpdater, ContentItemUpdater>();
            services.AddTransient<IMarkupContentItemUpdater<CmsApiHtmlModel>, MarkupContentItemUpdater<CmsApiHtmlModel>>();
            services.AddTransient<IMarkupContentItemUpdater<CmsApiHtmlSharedModel>, MarkupContentItemUpdater<CmsApiHtmlSharedModel>>();
            services.AddTransient<IMarkupContentItemUpdater<CmsApiSharedContentModel>, MarkupContentItemUpdater<CmsApiSharedContentModel>>();
            services.AddTransient<IEventGridService, EventGridService>();
            services.AddTransient<IEventGridClientService, EventGridClientService>();
            services.AddAutoMapper(typeof(Startup).Assembly);
            services.AddSingleton(configuration.GetSection(nameof(CmsApiClientOptions)).Get<CmsApiClientOptions>() ?? new CmsApiClientOptions());
            services.AddSingleton(configuration.GetSection(nameof(EventGridPublishClientOptions)).Get<EventGridPublishClientOptions>() ?? new EventGridPublishClientOptions());
            services.AddSingleton(configuration.GetSection(nameof(AppRegistryClientOptions)).Get<AppRegistryClientOptions>() ?? new AppRegistryClientOptions());
            services.AddSingleton(configuration.GetSection(nameof(CacheReloadTimerOptions)).Get<CacheReloadTimerOptions>() ?? new CacheReloadTimerOptions());
            services.AddHostedServiceTelemetryWrapper();
            services.AddSubscriptionBackgroundService(configuration);
            services.AddHostedService<CacheReloadBackgroundService>();
            services.AddHostedService<CacheReloadTimedHostedService>();

            const string AppSettingsPolicies = "Policies";
            var policyOptions = configuration.GetSection(AppSettingsPolicies).Get<PolicyOptions>() ?? new PolicyOptions();
            var policyRegistry = services.AddPolicyRegistry();

            services.AddApiServices(configuration, policyRegistry);

            services
                .AddPolicies(policyRegistry, nameof(AppRegistryClientOptions), policyOptions)
                .AddHttpClient<IAppRegistryApiService, AppRegistryApiService, AppRegistryClientOptions>(configuration, nameof(AppRegistryClientOptions), nameof(PolicyOptions.HttpRetry), nameof(PolicyOptions.HttpCircuitBreaker));

            services.AddMvc(config =>
                {
                    config.RespectBrowserAcceptHeader = true;
                    config.ReturnHttpNotAcceptable = true;
                })
                .AddNewtonsoftJson()
                .SetCompatibilityVersion(CompatibilityVersion.Version_3_0);
        }
    }
}
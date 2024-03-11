using AutoMapper;
using DFC.App.Pages.Cms.Data.Content;
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
using DFC.Common.SharedContent.Pkg.Netcore;
using DFC.Common.SharedContent.Pkg.Netcore.Constant;
using DFC.Common.SharedContent.Pkg.Netcore.Infrastructure;
using DFC.Common.SharedContent.Pkg.Netcore.Infrastructure.Strategy;
using DFC.Common.SharedContent.Pkg.Netcore.Interfaces;
using DFC.Common.SharedContent.Pkg.Netcore.Model.ContentItems;
using DFC.Common.SharedContent.Pkg.Netcore.Model.ContentItems.PageBreadcrumb;
using DFC.Common.SharedContent.Pkg.Netcore.Model.ContentItems.Sitemap;
using DFC.Common.SharedContent.Pkg.Netcore.Model.Response;
using DFC.Common.SharedContent.Pkg.Netcore.RequestHandler;
using DFC.Compui.Cosmos;
using DFC.Compui.Cosmos.Contracts;
using DFC.Compui.Subscriptions.Pkg.Netstandard.Extensions;
using DFC.Compui.Telemetry;
using DFC.Content.Pkg.Netcore.Data.Models.ClientOptions;
using DFC.Content.Pkg.Netcore.Extensions;
using GraphQL.Client.Abstractions;
using GraphQL.Client.Http;
using GraphQL.Client.Serializer.Newtonsoft;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Documents.Client;
using Microsoft.Azure.Management.EventGrid.Models;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using RestSharp;
using System;
using System.Diagnostics.CodeAnalysis;
using System.Net.Http;
using System.Threading;

namespace DFC.App.Pages
{
    [ExcludeFromCodeCoverage]
    public class Startup
    {
        private const string CosmosDbContentPagesConfigAppSettings = "Configuration:CosmosDbConnections:ContentPages";
        private const string RedisCacheConnectionStringAppSettings = "Cms:RedisCacheConnectionString";
        private const string GraphApiUrlAppSettings = "Cms:GraphApiUrl";
        private const string WorkerThreadsConfigAppSettings = "ThreadSettings:WorkerThreads";
        private const string IocpThreadsConfigAppSettings = "ThreadSettings:IocpThreads";


        private readonly IConfiguration configuration;
        private readonly IWebHostEnvironment env;
        private readonly ILogger<Startup> logger;

        public Startup(IConfiguration configuration, IWebHostEnvironment env, ILogger<Startup> logger)
        {
            this.configuration = configuration;
            this.env = env;
            this.logger = logger;
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
            ConfigureMinimumThreads();

            services.AddStackExchangeRedisCache(options => { options.Configuration = configuration.GetSection(RedisCacheConnectionStringAppSettings).Get<string>(); });

            services.AddSingleton<IGraphQLClient>(s =>
            {
                var option = new GraphQLHttpClientOptions()
                {
                    EndPoint = new Uri(configuration[ConfigKeys.GraphApiUrl]),
                    HttpMessageHandler = new CmsRequestHandler(s.GetService<IHttpClientFactory>(), s.GetService<IConfiguration>(), s.GetService<IHttpContextAccessor>()),
                };
                var client = new GraphQLHttpClient(option, new NewtonsoftJsonSerializer());
                return client;
            });
            services.AddSingleton<IRestClient>(s =>
            {
                var option = new RestClientOptions()
                {
                    BaseUrl = new Uri(configuration[ConfigKeys.SqlApiUrl]),
                    ConfigureMessageHandler = handler => new CmsRequestHandler(s.GetService<IHttpClientFactory>(), s.GetService<IConfiguration>(), s.GetService<IHttpContextAccessor>()),
                };
                JsonSerializerSettings defaultSettings = new JsonSerializerSettings
                {
                    ContractResolver = new CamelCasePropertyNamesContractResolver(),
                    DefaultValueHandling = DefaultValueHandling.Include,
                    TypeNameHandling = TypeNameHandling.None,
                    NullValueHandling = NullValueHandling.Ignore,
                    Formatting = Formatting.None,
                    ConstructorHandling = ConstructorHandling.AllowNonPublicDefaultConstructor,
                };

                var client = new RestClient(option);
                return client;
            });
            services.AddSingleton<ISharedContentRedisInterfaceStrategy<PageUrlResponse>, PageUrlQueryStrategy>();
            services.AddSingleton<ISharedContentRedisInterfaceStrategy<Page>, PageQueryStrategy>();
            services.AddSingleton<ISharedContentRedisInterfaceStrategy<PageBreadcrumb>, PageBreadcrumbQueryStrategy>();
            services.AddSingleton<ISharedContentRedisInterfaceStrategyFactory, SharedContentRedisStrategyFactory>();
            services.AddSingleton<ISharedContentRedisInterfaceStrategy<SitemapResponse>, PageSitemapStrategy>();

            services.AddScoped<ISharedContentRedisInterface, SharedContentRedis>();
            services.ConfigureOptions<contentOptionsSetup>();

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
            services.AddTransient<IMarkupContentItemUpdater<CmsApiFormModel>, MarkupContentItemUpdater<CmsApiFormModel>>();
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

        private void ConfigureMinimumThreads()
        {
            var workerThreads = Convert.ToInt32(configuration[WorkerThreadsConfigAppSettings]);

            var iocpThreads = Convert.ToInt32(configuration[IocpThreadsConfigAppSettings]);

            if (ThreadPool.SetMinThreads(workerThreads, iocpThreads))
            {
                logger.LogInformation(
                    "ConfigureMinimumThreads: Minimum configuration value set. IOCP = {0} and WORKER threads = {1}",
                    iocpThreads,
                    workerThreads);
            }
            else
            {
                logger.LogWarning("ConfigureMinimumThreads: The minimum number of threads was not changed");
            }
        }
    }
}
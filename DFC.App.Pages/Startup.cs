using AutoMapper;
using DFC.App.Pages.Cms.Data.Content;
using DFC.App.Pages.Data.Contracts;
using DFC.App.Pages.Data.Models.ClientOptions;
using DFC.App.Pages.Extensions;
using DFC.App.Pages.HttpClientPolicies;
using DFC.App.Pages.Services.AppRegistryService;
using DFC.Common.SharedContent.Pkg.Netcore;
using DFC.Common.SharedContent.Pkg.Netcore.Constant;
using DFC.Common.SharedContent.Pkg.Netcore.Infrastructure;
using DFC.Common.SharedContent.Pkg.Netcore.Infrastructure.Strategy;
using DFC.Common.SharedContent.Pkg.Netcore.Interfaces;
using DFC.Common.SharedContent.Pkg.Netcore.Model.ContentItems;
using DFC.Common.SharedContent.Pkg.Netcore.Model.ContentItems.PageBreadcrumb;
using DFC.Common.SharedContent.Pkg.Netcore.Model.Response;
using DFC.Common.SharedContent.Pkg.Netcore.RequestHandler;
using DFC.Compui.Telemetry;
using DFC.Content.Pkg.Netcore.Extensions;
using GraphQL.Client.Abstractions;
using GraphQL.Client.Http;
using GraphQL.Client.Serializer.Newtonsoft;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using RestSharp;
using StackExchange.Redis;
using System;
using System.Diagnostics.CodeAnalysis;
using System.Net.Http;

namespace DFC.App.Pages
{
    [ExcludeFromCodeCoverage]
    public class Startup
    {
        private const string CosmosDbContentPagesConfigAppSettings = "Configuration:CosmosDbConnections:ContentPages";
        private const string RedisCacheConnectionStringAppSettings = "Cms:RedisCacheConnectionString";
        private const string GraphApiUrlAppSettings = "Cms:GraphApiUrl";

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
            var redisCacheConnectionString = ConfigurationOptions.Parse(configuration.GetSection(RedisCacheConnectionStringAppSettings).Get<string>() ??
                throw new ArgumentNullException($"{nameof(RedisCacheConnectionStringAppSettings)} is missing or has an invalid value."));

            services.AddStackExchangeRedisCache(options => { options.Configuration = configuration.GetSection(RedisCacheConnectionStringAppSettings).Get<string>(); });
            services.AddHealthChecks().AddCheck<HealthCheck>("RedisConnectionCheck");
            services.AddSingleton<IConnectionMultiplexer>(option =>
            ConnectionMultiplexer.Connect(new ConfigurationOptions
            {
                EndPoints = { redisCacheConnectionString.EndPoints[0] },
                AbortOnConnectFail = false,
                Ssl = true,
                Password = redisCacheConnectionString.Password,
            }));

            services.AddSingleton<IGraphQLClient>(s =>
            {
                var option = new GraphQLHttpClientOptions()
                {
                    EndPoint = new Uri(configuration[ConfigKeys.GraphApiUrl] ??
                throw new ArgumentNullException($"{nameof(ConfigKeys.GraphApiUrl)} is missing or has an invalid value.")),
                    HttpMessageHandler = new CmsRequestHandler(s.GetService<IHttpClientFactory>(), s.GetService<IConfiguration>(), s.GetService<IHttpContextAccessor>()),
                };
                var client = new GraphQLHttpClient(option, new NewtonsoftJsonSerializer());
                return client;
            });
            services.AddSingleton<IRestClient>(s =>
            {
                var option = new RestClientOptions()
                {
                    BaseUrl = new Uri(configuration[ConfigKeys.SqlApiUrl] ??
                throw new ArgumentNullException($"{nameof(ConfigKeys.SqlApiUrl)} is missing or has an invalid value.")),
                    ConfigureMessageHandler = handler => new CmsRequestHandler(s.GetService<IHttpClientFactory>(), s.GetService<IConfiguration>(), s.GetService<IHttpContextAccessor>()),
                };
                JsonSerializerSettings defaultSettings = new ()
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
            services.AddSingleton<ISharedContentRedisInterfaceStrategy<PageApiResponse>, PageApiStrategy>();
            services.AddSingleton<ISharedContentRedisInterfaceStrategy<GetByPageApiResponse>, GetByIdPageApiStrategy>();

            services.AddScoped<ISharedContentRedisInterface, SharedContentRedis>();
            services.ConfigureOptions<contentOptionsSetup>();

            services.AddApplicationInsightsTelemetry();
            services.AddHttpContextAccessor();
            services.AddAutoMapper(typeof(Startup).Assembly);
            services.AddSingleton(configuration.GetSection(nameof(AppRegistryClientOptions)).Get<AppRegistryClientOptions>() ?? new AppRegistryClientOptions());
            services.AddHostedServiceTelemetryWrapper();

            const string AppSettingsPolicies = "Policies";
            var policyOptions = configuration.GetSection(AppSettingsPolicies).Get<PolicyOptions>() ?? new PolicyOptions();
            var policyRegistry = services.AddPolicyRegistry();

            services.AddRazorPages();

            services.AddApiServices(configuration, policyRegistry);

            services
                .AddPolicies(policyRegistry, nameof(AppRegistryClientOptions), policyOptions)
                .AddHttpClient<IAppRegistryApiService, AppRegistryApiService, AppRegistryClientOptions>(configuration, nameof(AppRegistryClientOptions), nameof(PolicyOptions.HttpRetry), nameof(PolicyOptions.HttpCircuitBreaker));

            services.AddControllers(options =>
            {
                options.RespectBrowserAcceptHeader = true;
                options.ReturnHttpNotAcceptable = true;
            }).AddNewtonsoftJson();
        }
    }
}
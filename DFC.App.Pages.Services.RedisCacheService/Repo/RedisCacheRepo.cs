using DFC.App.Pages.Services.RedisCacheService.Constant;
using DFC.App.Pages.Services.RedisCacheService.Enum;
using DFC.App.Pages.Services.RedisCacheService.Model;
using DfE.NCS.Framework.Cache.Interface;
using GraphQL.Client.Abstractions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DFC.App.Pages.Services.RedisCacheService.Repo
{
    public class RedisCacheRepo : IRedisCacheRepo
    {
        private readonly ICacheService cache;
        private readonly IConfiguration config;
        private readonly IGraphQLClient client;
        private readonly IRestClient sqlClient;
        private readonly ILogger<RedisCacheRepo> logger;
        private readonly bool isConfigCacheEnabled;

        /// <summary>
        /// Initializes a new instance of the <see cref="RedisCacheRepo"/> class.
        /// </summary>
        /// <param name="cache">The cache.</param>
        /// 
        public RedisCacheRepo(ICacheService cache, IConfiguration config, IGraphQLClient client, IRestClient sqlClient, ILogger<RedisCacheRepo> logger)
        {
            this.cache = cache;
            this.config = config;
            this.client = client;
            this.sqlClient = sqlClient;
            this.logger = logger;
            isConfigCacheEnabled = this.config.GetValue<bool>(ConfigKeys.RedisCacheEnabled);
        }

        public async Task<CacheRepoResponse<TResponse>> GetGraphQLData<TResponse>(string query, string cacheKey, bool disableCache = false) where TResponse : class
        {
            logger.LogInformation("Redis cache -> GetGraphQLData started");
            bool isCacheEnabled = !disableCache;

            cacheKey = BuildCacheKey(cacheKey);
            logger.LogInformation("Redis cache key->" + cacheKey);
            if (isCacheEnabled)
            {
                // check if the data is available in the cache
                var cacheResponse = cache.GetEntity<TResponse>(cacheKey);
                if (cacheResponse?.Value != null)
                {
                    logger.LogInformation("Redis cache key->" + cacheKey + " had value and returned.");
                    logger.LogInformation("Redis cache -> GetGraphQLData completed.");
                    return new CacheRepoResponse<TResponse>()
                    {
                        Data = cacheResponse.Value,
                        MetaData = new CacheMetaData()
                        {
                            CacheAction = CacheActionEnum.Read,
                            Expires = cacheResponse.Expires,
                            IsCacheEnabled = true,
                        },
                    };
                }
            }
            logger.LogInformation("Redis cache -> ExecuteGraphQL started.");
            var graphQLResponse = await ExecuteGraphQL<TResponse>(query);
            var response = new CacheRepoResponse<TResponse>()
            {
                Data = graphQLResponse,
                MetaData = new CacheMetaData(),
            };
            try
            {
                logger.LogInformation("Redis cache -> SaveEntity started.");
                var cacheResponse = cache.SaveEntity(cacheKey, graphQLResponse);
                response.MetaData.Expires = cacheResponse.Expires;
                response.MetaData.IsCacheEnabled = isCacheEnabled;
                response.MetaData.CacheAction = CacheActionEnum.Add;
                logger.LogInformation("Redis cache -> SaveEntity completed.");
            }
            catch (Exception ex)
            {
                logger.LogWarning("Error Message: " + ex.Message);
            }
            logger.LogInformation("Redis cache -> GetGraphQLData completed.");
            return response;
        }

        public async Task<CacheRepoResponse<TResponse>> GetSqlData<TResponse>(string query, string cacheKey, bool disableCache = false) where TResponse : class
        {
            logger.LogInformation("Redis cache -> GetSqlData started");
            bool isCacheEnabled = !disableCache;
            cacheKey = BuildCacheKey(cacheKey);
            logger.LogInformation("Redis cache key->" + cacheKey);
            if (isCacheEnabled)
            {
                // check if the data is available in the cache
                var cacheResponse = cache.GetEntity<TResponse>(cacheKey);
                if (cacheResponse?.Value != null)
                {
                    logger.LogInformation("Redis cache key->" + cacheKey + " had value and returned.");
                    logger.LogInformation("Redis cache -> GetSqlData completed.");
                    return new CacheRepoResponse<TResponse>()
                    {
                        Data = cacheResponse.Value,
                        MetaData = new CacheMetaData()
                        {
                            CacheAction = CacheActionEnum.Read,
                            Expires = cacheResponse.Expires,
                            IsCacheEnabled = true,
                        },
                    };
                }
            }
            logger.LogInformation("Redis cache -> ExecuteSQL started.");
            var sqlResponse = await ExecuteSQL<TResponse>(query);
            var response = new CacheRepoResponse<TResponse>()
            {
                Data = sqlResponse,
                MetaData = new CacheMetaData(),
            };
            try
            {
                logger.LogInformation("Redis cache -> SaveEntity started.");
                var cacheResponse = cache.SaveEntity(cacheKey, sqlResponse);
                response.MetaData.Expires = cacheResponse.Expires;
                response.MetaData.IsCacheEnabled = isCacheEnabled;
                response.MetaData.CacheAction = CacheActionEnum.Add;
                logger.LogInformation("Redis cache -> SaveEntity completed.");
            }
            catch (Exception ex)
            {
                logger.LogWarning("Error Message: " + ex.Message);
            }
            logger.LogInformation("Redis cache -> GetSqlData completed.");
            return response;
        }

        private async Task<TResponse> ExecuteSQL<TResponse>(string queryName) where TResponse : class
        {
            var request = new RestRequest(queryName);
            var response = await this.sqlClient.GetAsync<TResponse>(request);
            return response;
        }

        private async Task<TResponse> ExecuteGraphQL<TResponse>(string query)
        {
            var response = await client.SendQueryAsync<TResponse>(query);
            return response.Data;
        }

        private string BuildCacheKey(string cacheKey)
        {
            string cacheKeyPrefix = this.config[ConfigKeys.RedisCacheKeyPrefix];

            return cacheKeyPrefix + ":" + cacheKey;
        }
    }
}
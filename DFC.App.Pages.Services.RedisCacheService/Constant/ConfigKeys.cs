using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DFC.App.Pages.Services.RedisCacheService.Constant
{
    public class ConfigKeys
    {
        /// <summary>
        /// The redis cache enabled.
        /// </summary>
        public const string RedisCacheEnabled = "RedisCache:CacheEnabled";

        /// <summary>
        /// The redis cache key prefix. Used only in debug mode for local environmet cache key conflict.
        /// </summary>
        public const string RedisCacheKeyPrefix = "RedisCache:CacheKeyPrefix";
    }
}
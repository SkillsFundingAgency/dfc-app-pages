using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DFC.App.Pages.Services.RedisCacheService.Interface
{
    public interface IRedisCMSRepo
    {
        /// <summary>
        /// Gets the data.
        /// </summary>
        /// <typeparam name="TResponse">The type of the response.</typeparam>
        /// <param name="query">The query.</param>
        /// <returns>The response.</returns>
        Task<TResponse> GetGraphQLData<TResponse>(string query, string cacheKey, bool disableCache = false)
            where TResponse : class;

        Task<TResponse> GetSqlData<TResponse>(string query, string cacheKey, bool disableCache = false);
    }
}

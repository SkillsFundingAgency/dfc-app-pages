using DFC.App.Pages.Services.RedisCacheService.Repo;
using DfE.NCS.Framework.Cache.Interface;
using DfE.NCS.Framework.Cache.Model;
using GraphQL.Client.Abstractions;
using Microsoft.Extensions.Configuration;
using Moq;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DFC.App.Pages.Services.RedisCacheService.UnitTests
{
    public class RedisCacheRepoTests
    {
        // <summary>
        /// Tests the GetData method when in preview mode and cache is enabled.
        /// </summary>
        /// <returns>Task result.</returns>
        //[Fact]
        /*public async Task GetGraphQLData_WithCacheEnabled_CacheHit()
        {
            // Arrange
            var mockCache = new Mock<ICacheService>();
            var cacheValue = new CacheValue<MockResponse>(3600, new MockResponse() { });
            mockCache.Setup(x => x.GetEntity<MockResponse>(It.IsAny<string>())).Returns(cacheValue);

            var cacheKey = "true";
            var configuration = CreateConfiguration("true");

            var redisCacheRepo = new RedisCacheRepo(
                mockCache.Object,
                configuration,
                Mock.Of<IGraphQLClient>(),
                Mock.Of<IRestClient>()
                );

            // Act
            var result = await redisCacheRepo.GetGraphQLData<MockResponse>("query", cacheKey, false);

            // Assert
            Assert.NotNull(result);
        }*/

        /// <summary>
        /// mock response.
        /// </summary>
        public class MockResponse
        {
        }

        private IConfiguration CreateConfiguration(string configValue)
        {
            var config = new Dictionary<string, string> { { "true", configValue } };
            var initialData = config.Select(x => new KeyValuePair<string, string?>(x.Key, x.Value));
            return new ConfigurationBuilder().AddInMemoryCollection(initialData).Build();
        }
    }
}
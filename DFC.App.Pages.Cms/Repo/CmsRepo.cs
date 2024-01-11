using DFC.App.Pages.Cms.Interface;
using GraphQL.Client.Abstractions;
using RestSharp;

namespace DFC.App.Pages.Cms.Repo
{
    /// <summary>
    /// The SSP CMS Repo.
    /// </summary>
    /// <seealso cref="DfE.NCS.SSP.CMS.Service.Interface.Repo.ISSPCmsRepo" />
    public class CmsRepo : ICmsRepo
    {
        private readonly IConfiguration config;
        private readonly IGraphQLClient client;
        private readonly IRestClient sqlClient;

        /// <summary>
        /// Initializes a new instance of the <see cref="SSPCMSRepo"/> class.
        /// </summary>
        /// <param name="client">The client.</param>
        /// <param name="config">The configuration.</param>
        public CmsRepo(IGraphQLClient client, IConfiguration config, IRestClient sqlClient)
        {
            this.config = config;
            this.client = client;
            this.sqlClient = sqlClient;
        }

        public async Task<TResponse> GetData<TResponse>(string query) where TResponse : class
        {
            //if not available in the cache get it from the graph ql api
            var graphQLResponse = await this.client.SendQueryAsync<TResponse>(query);
            return graphQLResponse.Data;
        }

        public async Task<TResponse> GetSqlData<TResponse>(string queryName)
        {
            var request = new RestRequest(queryName);
            var response = await this.sqlClient.GetAsync<TResponse>(request);
            return response;
        }


    }
}

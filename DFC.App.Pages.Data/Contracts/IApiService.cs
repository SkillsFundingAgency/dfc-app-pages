using System;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;

namespace DFC.App.Pages.Data.Contracts
{
    public interface IApiService
    {
        Task<string?> GetAsync(HttpClient? httpClient, Uri url, string acceptHeader);

        Task<HttpStatusCode> PostAsync(HttpClient? httpClient, Uri url);

        Task<HttpStatusCode> PostAsync<TApiModel>(HttpClient? httpClient, Uri url, TApiModel model)
            where TApiModel : class;

        Task<HttpStatusCode> DeleteAsync(HttpClient? httpClient, Uri url);
    }
}
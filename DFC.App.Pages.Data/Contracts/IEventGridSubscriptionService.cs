using System;
using System.Net;
using System.Threading.Tasks;

namespace DFC.App.Pages.Data.Contracts
{
    public interface IEventGridSubscriptionService
    {
        Task<HttpStatusCode> CreateAsync(Guid id);

        Task<HttpStatusCode> DeleteAsync(Guid id);
    }
}

using System.Net;
using System.Threading.Tasks;

namespace DFC.App.Pages.Data.Contracts
{
    public interface IEventGridSubscriptionService
    {
        Task<HttpStatusCode> CreateAsync();

        Task<HttpStatusCode> DeleteAsync();
    }
}

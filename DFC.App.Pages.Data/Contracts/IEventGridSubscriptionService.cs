using System;
using System.Threading.Tasks;

namespace DFC.App.Pages.Data.Contracts
{
    public interface IEventGridSubscriptionService
    {
        Task CreateAsync(Guid id);

        Task DeleteAsync(Guid id);
    }
}

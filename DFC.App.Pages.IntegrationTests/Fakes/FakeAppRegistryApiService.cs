using DFC.App.Pages.Data.Contracts;
using System.Threading.Tasks;

namespace DFC.App.Pages.IntegrationTests.Fakes
{
    public class FakeAppRegistryApiService : IAppRegistryApiService
    {
        public Task PagesDataLoadAsync()
        {
            return Task.CompletedTask;
        }
    }
}

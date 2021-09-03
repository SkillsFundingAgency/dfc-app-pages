using DFC.Compui.Subscriptions.Pkg.NetStandard.Data.Contracts;
using System.Threading.Tasks;

namespace DFC.App.Pages.IntegrationTests.Fakes
{
    public class FakeSubscriptionRegistrationService : ISubscriptionRegistrationService
    {
        public Task RegisterSubscription(string subscriptionName)
        {
            return Task.CompletedTask;
        }
    }
}

namespace DFC.App.Pages.Data.Models.SubscriptionModels
{
    public class EventGridSubscriptionModel
    {
        public string? Name { get; set; }

        public string? Endpoint { get; set; }

        public SubscriptionFilterModel Filter { get; set; } = new SubscriptionFilterModel();
    }
}

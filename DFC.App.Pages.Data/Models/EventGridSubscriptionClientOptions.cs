namespace DFC.App.Pages.Data.Models
{
    public class EventGridSubscriptionClientOptions : ClientOptionsModel
    {
        public string Endpoint { get; set; } = "subscription/";

        public string? ApplicationName { get; set; }

        public string? EventGridTopic { get; set; }

        public string? EventGridResourceGroup { get; set; }
    }
}

using System;
using System.Diagnostics.CodeAnalysis;

namespace DFC.App.Pages.Data.Models.ClientOptions
{
    [ExcludeFromCodeCoverage]
    public class EventGridPublishClientOptions
    {
        public string? TopicEndpoint { get; set; }

        public string? SubjectPrefix { get; set; }

        public string? TopicKey { get; set; }

        public Uri? ApiEndpoint { get; set; }
    }
}

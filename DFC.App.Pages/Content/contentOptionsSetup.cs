using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;

namespace DFC.App.Pages.Cms.Data.Content
{
    public class ContentOptionsSetup : IConfigureOptions<ContentModeOptions>
    {
        private const string SectionName = "contentMode";
        private readonly IConfiguration _configuration;

        public ContentOptionsSetup(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public void Configure(ContentModeOptions options)
        {
            _configuration
                .GetSection(SectionName)
                .Bind(options);
        }
    }
}
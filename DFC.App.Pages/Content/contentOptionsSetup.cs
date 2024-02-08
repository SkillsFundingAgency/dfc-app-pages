using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;

namespace DFC.App.Pages.Cms.Data.Content
{
    public class contentOptionsSetup : IConfigureOptions<contentModeOptions>
    {
        private const string SectionName = "contentMode";
        private readonly IConfiguration _configuration;

        public contentOptionsSetup(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public void Configure(contentModeOptions options)
        {
            _configuration
                .GetSection(SectionName)
                .Bind(options);
        }
    }
}

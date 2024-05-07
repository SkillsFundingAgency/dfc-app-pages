using DFC.App.Pages.Cms.Data.Content;
using Microsoft.Extensions.Options;
using Xunit;
using Xunit.Abstractions;

namespace DFC.App.Pages.UnitTests.ControllerTests.GraphQL_Tests
{
    public class QueryStatusTests
    {
        private contentModeOptions _options;

        public contentOptionsSetup setup;
        private readonly ITestOutputHelper output;

        public QueryStatusTests(ITestOutputHelper output)
        {
            this.output = output;
        }

        [Fact]
        public void RetrieveStatusPublished()
        {
            IOptions<contentModeOptions> sampleOptions = Options.Create<contentModeOptions>(new contentModeOptions());
            // Arrange
            var settings = new contentModeOptions()
            {
                contentMode = "contentMode",
                value = "PUBLISHED",
            };
            IOptions<contentModeOptions> appSettingsMock = Options.Create(settings);
            string expectedValue = "PUBLISHED";

            // Act
            _options = appSettingsMock.Value;
            output.WriteLine(expectedValue);
            output.WriteLine(_options.ToString());



            //Assert
            expectedValue = _options.value;
            output.WriteLine(expectedValue);

        }

        [Fact]
        public void RetrieveStatusDraft()
        {
            IOptions<contentModeOptions> sampleOptions = Options.Create<contentModeOptions>(new contentModeOptions());
            // Arrange
            var settings = new contentModeOptions()
            {
                contentMode = "contentMode",
                value = "DRAFT",
            };
            IOptions<contentModeOptions> appSettingsMock = Options.Create(settings);
            string expectedValue = "DRAFT";

            // Act
            _options = appSettingsMock.Value;
            output.WriteLine(expectedValue);
            output.WriteLine(_options.ToString());



            //Assert
            expectedValue = _options.value;
            output.WriteLine(expectedValue);

        }
    }
}

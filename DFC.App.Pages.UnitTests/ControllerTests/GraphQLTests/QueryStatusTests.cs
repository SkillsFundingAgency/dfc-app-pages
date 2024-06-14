using DFC.App.Pages.Cms.Data.Content;
using Microsoft.Extensions.Options;
using Xunit;
using Xunit.Abstractions;

namespace DFC.App.Pages.UnitTests.ControllerTests.GraphQL_Tests
{
    public class QueryStatusTests
    {
        private ContentModeOptions _options;

        private readonly ITestOutputHelper output;

        public QueryStatusTests(ITestOutputHelper output)
        {
            this.output = output;
        }

        [Fact]
        public void RetrieveStatusPublished()
        {
            IOptions<ContentModeOptions> sampleOptions = Options.Create<ContentModeOptions>(new ContentModeOptions());
            // Arrange
            var settings = new ContentModeOptions()
            {
                contentMode = "contentMode",
                value = "PUBLISHED",
            };
            IOptions<ContentModeOptions> appSettingsMock = Options.Create(settings);
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
            IOptions<ContentModeOptions> sampleOptions = Options.Create<ContentModeOptions>(new ContentModeOptions());
            // Arrange
            var settings = new ContentModeOptions()
            {
                contentMode = "contentMode",
                value = "DRAFT",
            };
            IOptions<ContentModeOptions> appSettingsMock = Options.Create(settings);
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

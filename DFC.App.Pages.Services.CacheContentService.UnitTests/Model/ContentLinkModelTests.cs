using DFC.App.Pages.Data.Models;
using Newtonsoft.Json.Linq;
using System.IO;
using System.Linq;
using Xunit;

namespace DFC.App.Pages.Services.CacheContentService.UnitTests.Model
{
    public class ContentLinkModelTests
    {
        [Theory]
        [InlineData(true, 4)]
        [InlineData(false, 5)]
        public void WhenContentLinksIsNotNullThenReturnLinks(bool excludePageLocation, int epresctedResultsCount)
        {
            var jsonData = File.ReadAllText(Directory.GetCurrentDirectory() + "/Model/LinksData.json");
            var model = new ContentLinksModel(JObject.Parse(jsonData));
            model.ExcludePageLocation = excludePageLocation;
            var links = model.ContentLinks;
            Assert.Equal(epresctedResultsCount, links.SelectMany(x => x.Value).Count());
        }

        [Fact]
        public void WhenContentLinksIsNullThenReturnEmptyList()
        {
            var jsonData = "{}";
            var model = new ContentLinksModel(JObject.Parse(jsonData));
            var links = model.ContentLinks;
            Assert.False(links.Any());
        }
    }
}

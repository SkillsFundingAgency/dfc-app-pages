using DFC.App.Pages.Data.Models;
using System.Collections.Generic;
using Xunit;

namespace DFC.App.Pages.Services.CacheContentService.UnitTests.Model
{
    public class PagesApiDataModelTests
    {
        [Fact]
        public void PageApiDataModelReturnCorrectPageLocation()
        {
            var model = new PagesApiDataModel
            {
                CanonicalName = "test",
                RedirectLocations = new List<string>
                {
                    "location",
                },
            };

            Assert.Equal("location", model.PageLocation);
        }

        [Fact]
        public void WhenRedirectLocationsEmptyThenPageApiDataModelReturnsCorrectPageLocation()
        {
            var model = new PagesApiDataModel
            {
                CanonicalName = "test",
                RedirectLocations = new List<string>(),
            };

            Assert.Equal("/", model.PageLocation);
        }
    }
}

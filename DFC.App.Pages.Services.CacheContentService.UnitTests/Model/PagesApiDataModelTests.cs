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
                TaxonomyTerms = new List<string>
                {
                    "location",
                },
            };

            Assert.Equal("location", model.Pagelocation);
        }

        [Fact]
        public void WhenTaxonomyTermsEmptyThenPageApiDataModelReturnsCorrectPageLocation()
        {
            var model = new PagesApiDataModel
            {
                CanonicalName = "test",
                TaxonomyTerms = new List<string>(),
            };

            Assert.Equal("/", model.Pagelocation);
        }
    }
}

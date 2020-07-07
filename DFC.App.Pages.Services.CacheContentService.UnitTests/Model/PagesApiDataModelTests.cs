using System;
using System.Collections.Generic;
using System.Text;
using DFC.App.Pages.Data.Models;
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

            Assert.Equal(model.Pagelocation, "location/test");
        }

        [Fact]
        public void WhenTaxonomyTermsEmptyThenPageApiDataModelReturnsCorrectPageLocation()
        {
            var model = new PagesApiDataModel
            {
                CanonicalName = "test",
                TaxonomyTerms = new List<string>(),
            };

            Assert.Equal(model.Pagelocation, "/test");
        }
    }
}

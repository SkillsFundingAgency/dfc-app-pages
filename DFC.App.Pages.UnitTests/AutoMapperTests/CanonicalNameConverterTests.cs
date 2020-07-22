using AutoMapper;
using DFC.App.Pages.AutoMapperProfiles.ValuerConverters;
using DFC.App.Pages.Data.Models;
using System.Collections.Generic;
using Xunit;

namespace DFC.App.Pages.UnitTests.AutoMapperTests
{
    [Trait("Category", "AutoMapper")]
    public class CanonicalNameConverterTests
    {
        [Fact]
        public void CanonicalNameConverterTestsReturnsNullForNullSourceMember()
        {
            // Arrange
            var converter = new CanonicalNameConverter();
            PagesApiDataModel? sourceMember = null;
            var context = new ResolutionContext(null, null);

            // Act
            var result = converter.Convert(sourceMember, context);

            // Assert
            Assert.Null(result);
        }

        [Fact]
        public void CanonicalNameConverterTestsReturnsCanonicalName()
        {
            // Arrange
            const string expectedResult = "a-canonical-name";
            var converter = new CanonicalNameConverter();
            var sourceMember = new PagesApiDataModel { CanonicalName = expectedResult };
            var context = new ResolutionContext(null, null);

            // Act
            var result = converter.Convert(sourceMember, context);

            // Assert
            Assert.Equal(expectedResult, result);
        }

        [Fact]
        public void CanonicalNameConverterTestsReturnsNullForNoSlashTaxonomyTerms()
        {
            // Arrange
            var converter = new CanonicalNameConverter();
            var sourceMember = new PagesApiDataModel { TaxonomyTerms = new List<string> { "root" } };
            var context = new ResolutionContext(null, null);

            // Act
            var result = converter.Convert(sourceMember, context);

            // Assert
            Assert.Null(result);
        }

        [Fact]
        public void CanonicalNameConverterTestsReturnsNullForRootTaxonomyTerms()
        {
            // Arrange
            var converter = new CanonicalNameConverter();
            var sourceMember = new PagesApiDataModel();
            var context = new ResolutionContext(null, null);

            // Act
            var result = converter.Convert(sourceMember, context);

            // Assert
            Assert.Null(result);
        }

        [Fact]
        public void CanonicalNameConverterTestsReturnsNullForLeafTaxonomyTerms()
        {
            // Arrange
            const string expectedResult = "leaf";
            var converter = new CanonicalNameConverter();
            var sourceMember = new PagesApiDataModel { TaxonomyTerms = new List<string> { "/root/branch/leaf" } };
            var context = new ResolutionContext(null, null);

            // Act
            var result = converter.Convert(sourceMember, context);

            // Assert
            Assert.Equal(expectedResult, result);
        }
    }
}

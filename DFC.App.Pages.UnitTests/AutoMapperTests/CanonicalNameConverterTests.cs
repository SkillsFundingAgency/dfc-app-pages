using AutoMapper;
using DFC.App.Pages.AutoMapperProfiles.ValuerConverters;
using DFC.App.Pages.Data.Models;
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
        public void CanonicalNameConverterTestsReturnsNullForNoSlashPageLocation()
        {
            // Arrange
            var converter = new CanonicalNameConverter();
            var sourceMember = new PagesApiDataModel { PageLocation = "root" };
            var context = new ResolutionContext(null, null);

            // Act
            var result = converter.Convert(sourceMember, context);

            // Assert
            Assert.Null(result);
        }

        [Fact]
        public void CanonicalNameConverterTestsReturnsNullForRootPageLocation()
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
        public void CanonicalNameConverterTestsReturnsLeafFromPageLocation()
        {
            // Arrange
            const string expectedResult = "leaf";
            var converter = new CanonicalNameConverter();
            var sourceMember = new PagesApiDataModel { PageLocation = "/root/branch/leaf" };
            var context = new ResolutionContext(null, null);

            // Act
            var result = converter.Convert(sourceMember, context);

            // Assert
            Assert.Equal(expectedResult, result);
        }

        [Fact]
        public void CanonicalNameConverterTestsReturnsRootFromPageLocation()
        {
            // Arrange
            const string expectedResult = "root";
            var converter = new CanonicalNameConverter();
            var sourceMember = new PagesApiDataModel { PageLocation = "/root" };
            var context = new ResolutionContext(null, null);

            // Act
            var result = converter.Convert(sourceMember, context);

            // Assert
            Assert.Equal(expectedResult, result);
        }
    }
}

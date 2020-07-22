using AutoMapper;
using DFC.App.Pages.AutoMapperProfiles.ValuerConverters;
using DFC.App.Pages.Data.Models;
using Xunit;

namespace DFC.App.Pages.UnitTests.AutoMapperTests
{
    [Trait("Category", "AutoMapper")]
    public class LocationPathConverterTests
    {
        [Fact]
        public void LocationPathConverterReturnsNullForNullSourceMember()
        {
            // Arrange
            var converter = new LocationPathConverter();
            ContentPageModel? sourceMember = null;
            var context = new ResolutionContext(null, null);

            // Act
            var result = converter.Convert(sourceMember, context);

            // Assert
            Assert.Null(result);
        }

        [Fact]
        public void LocationPathConverterReturnsSlashForNoData()
        {
            // Arrange
            const string expectedResult = "/";
            var converter = new LocationPathConverter();
            var sourceMember = new ContentPageModel { PageLocation = string.Empty, CanonicalName = string.Empty };
            var context = new ResolutionContext(null, null);

            // Act
            var result = converter.Convert(sourceMember, context);

            // Assert
            Assert.Equal(expectedResult, result);
        }

        [Fact]
        public void LocationPathConverterReturnsSlashForNoCanonicalName()
        {
            // Arrange
            const string expectedResult = "/";
            var converter = new LocationPathConverter();
            var sourceMember = new ContentPageModel { PageLocation = "/", CanonicalName = string.Empty };
            var context = new ResolutionContext(null, null);

            // Act
            var result = converter.Convert(sourceMember, context);

            // Assert
            Assert.Equal(expectedResult, result);
        }

        [Fact]
        public void LocationPathConverterReturnsPathForNoPageLocation()
        {
            // Arrange
            const string expectedResult = "/world";
            var converter = new LocationPathConverter();
            var sourceMember = new ContentPageModel { PageLocation = string.Empty, CanonicalName = "world" };
            var context = new ResolutionContext(null, null);

            // Act
            var result = converter.Convert(sourceMember, context);

            // Assert
            Assert.Equal(expectedResult, result);
        }

        [Fact]
        public void LocationPathConverterReturnsPathForNoCanonicalName()
        {
            // Arrange
            const string expectedResult = "/hello";
            var converter = new LocationPathConverter();
            var sourceMember = new ContentPageModel { PageLocation = "/hello", CanonicalName = string.Empty };
            var context = new ResolutionContext(null, null);

            // Act
            var result = converter.Convert(sourceMember, context);

            // Assert
            Assert.Equal(expectedResult, result);
        }

        [Fact]
        public void LocationPathConverterReturnsPathForData()
        {
            // Arrange
            const string expectedResult = "/hello/world";
            var converter = new LocationPathConverter();
            var sourceMember = new ContentPageModel { PageLocation = "/hello", CanonicalName = "world" };
            var context = new ResolutionContext(null, null);

            // Act
            var result = converter.Convert(sourceMember, context);

            // Assert
            Assert.Equal(expectedResult, result);
        }

        [Fact]
        public void LocationPathConverterReturnsPathForMissingSlash()
        {
            // Arrange
            const string expectedResult = "/hello/world";
            var converter = new LocationPathConverter();
            var sourceMember = new ContentPageModel { PageLocation = "hello", CanonicalName = "world" };
            var context = new ResolutionContext(null, null);

            // Act
            var result = converter.Convert(sourceMember, context);

            // Assert
            Assert.Equal(expectedResult, result);
        }

        [Fact]
        public void LocationPathConverterReturnsPathForDefaultPageLocation()
        {
            // Arrange
            const string expectedResult = "/hello";
            var converter = new LocationPathConverter();
            var sourceMember = new ContentPageModel { PageLocation = "hello", CanonicalName = "world", IsDefaultForPageLocation = true };
            var context = new ResolutionContext(null, null);

            // Act
            var result = converter.Convert(sourceMember, context);

            // Assert
            Assert.Equal(expectedResult, result);
        }
    }
}

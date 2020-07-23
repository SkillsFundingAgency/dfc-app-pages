using AutoMapper;
using DFC.App.Pages.AutoMapperProfiles.ValuerConverters;
using DFC.App.Pages.Data.Models;
using System.Collections.Generic;
using Xunit;

namespace DFC.App.Pages.UnitTests.AutoMapperTests
{
    [Trait("Category", "AutoMapper")]
    public class LocationsConverterTests
    {
        [Fact]
        public void LocationsConverterReturnsNullForNullSourceMember()
        {
            // Arrange
            var converter = new LocationsConverter();
            ContentPageModel? sourceMember = null;
            var context = new ResolutionContext(null, null);

            // Act
            var result = converter.Convert(sourceMember, context);

            // Assert
            Assert.Null(result);
        }

        [Fact]
        public void LocationsConverterReturnsSlashForNoData()
        {
            // Arrange
            var expectedResult = new List<string> { "/" };
            var converter = new LocationsConverter();
            var sourceMember = new ContentPageModel { PageLocation = string.Empty, CanonicalName = string.Empty };
            var context = new ResolutionContext(null, null);

            // Act
            var result = converter.Convert(sourceMember, context);

            // Assert
            Assert.Equal(expectedResult, result);
        }

        [Fact]
        public void LocationsConverterReturnsSlashForNoCanonicalName()
        {
            // Arrange
            var expectedResult = new List<string> { "/" };
            var converter = new LocationsConverter();
            var sourceMember = new ContentPageModel { PageLocation = "/", CanonicalName = string.Empty };
            var context = new ResolutionContext(null, null);

            // Act
            var result = converter.Convert(sourceMember, context);

            // Assert
            Assert.Equal(expectedResult, result);
        }

        [Fact]
        public void LocationsConverterReturnsPathForNoPageLocation()
        {
            // Arrange
            var expectedResult = new List<string> { "/world" };
            var converter = new LocationsConverter();
            var sourceMember = new ContentPageModel { PageLocation = string.Empty, CanonicalName = "world" };
            var context = new ResolutionContext(null, null);

            // Act
            var result = converter.Convert(sourceMember, context);

            // Assert
            Assert.Equal(expectedResult, result);
        }

        [Fact]
        public void LocationsConverterReturnsPathForNoCanonicalName()
        {
            // Arrange
            var expectedResult = new List<string> { "/hello" };
            var converter = new LocationsConverter();
            var sourceMember = new ContentPageModel { PageLocation = "/hello", CanonicalName = string.Empty };
            var context = new ResolutionContext(null, null);

            // Act
            var result = converter.Convert(sourceMember, context);

            // Assert
            Assert.Equal(expectedResult, result);
        }

        [Fact]
        public void LocationsConverterReturnsPathForData()
        {
            // Arrange
            var expectedResult = new List<string> { "/hello/world" };
            var converter = new LocationsConverter();
            var sourceMember = new ContentPageModel { PageLocation = "/hello", CanonicalName = "world" };
            var context = new ResolutionContext(null, null);

            // Act
            var result = converter.Convert(sourceMember, context);

            // Assert
            Assert.Equal(expectedResult, result);
        }

        [Fact]
        public void LocationsConverterReturnsPathForMissingSlash()
        {
            // Arrange
            var expectedResult = new List<string> { "/hello/world" };
            var converter = new LocationsConverter();
            var sourceMember = new ContentPageModel { PageLocation = "hello", CanonicalName = "world" };
            var context = new ResolutionContext(null, null);

            // Act
            var result = converter.Convert(sourceMember, context);

            // Assert
            Assert.Equal(expectedResult, result);
        }

        [Fact]
        public void LocationsConverterReturnsPathForDefaultPageLocation()
        {
            // Arrange
            var expectedResult = new List<string> { "/hello", "/hello/world" };
            var converter = new LocationsConverter();
            var sourceMember = new ContentPageModel { PageLocation = "hello", CanonicalName = "world", IsDefaultForPageLocation = true };
            var context = new ResolutionContext(null, null);

            // Act
            var result = converter.Convert(sourceMember, context);

            // Assert
            Assert.Equal(expectedResult, result);
        }

        [Fact]
        public void LocationsConverterReturnsPathWithRedirectLocations()
        {
            // Arrange
            var expectedResult = new List<string> { "/hello/world", "/hello/cruel/world", "/hello/big/wide/world" };
            var converter = new LocationsConverter();
            var sourceMember = new ContentPageModel { PageLocation = "hello", CanonicalName = "world", RedirectLocations = new List<string> { "/hello/cruel/world", "/hello/big/wide/world" } };
            var context = new ResolutionContext(null, null);

            // Act
            var result = converter.Convert(sourceMember, context);

            // Assert
            Assert.Equal(expectedResult, result);
        }
    }
}

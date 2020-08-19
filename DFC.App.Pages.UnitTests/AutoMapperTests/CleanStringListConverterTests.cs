using AutoMapper;
using DFC.App.Pages.AutoMapperProfiles.ValuerConverters;
using System.Collections.Generic;
using Xunit;

namespace DFC.App.Pages.UnitTests.AutoMapperTests
{
    [Trait("Category", "AutoMapper")]
    public class CleanStringListConverterTests
    {
        [Fact]
        public void RCleanStringListConverterTestsReturnsNullForNullSourceMember()
        {
            // Arrange
            var converter = new CleanStringListConverter();
            List<string>? sourceMember = null;
            var context = new ResolutionContext(null, null);

            // Act
            var result = converter.Convert(sourceMember, context);

            // Assert
            Assert.Null(result);
        }

        [Fact]
        public void RCleanStringListConverterTestsNullForEmptyList()
        {
            // Arrange
            var converter = new CleanStringListConverter();
            List<string>? expectedResult = null;
            var sourceMember = new List<string>();
            var context = new ResolutionContext(null, null);

            // Act
            var result = converter.Convert(sourceMember, context);

            // Assert
            Assert.Equal(expectedResult, result);
        }

        [Fact]
        public void RCleanStringListConverterTestsReturnsSameList()
        {
            // Arrange
            var converter = new CleanStringListConverter();
            var expectedResult = new List<string> { "/item1", "/item2" };
            var sourceMember = expectedResult;
            var context = new ResolutionContext(null, null);

            // Act
            var result = converter.Convert(sourceMember, context);

            // Assert
            Assert.Equal(expectedResult, result);
        }

        [Fact]
        public void RCleanStringListConverterTestsRemovesBlankItems()
        {
            // Arrange
            var converter = new CleanStringListConverter();
            var expectedResult = new List<string> { "/item1", "/item3" };
            var sourceMember = new List<string> { "/item1", string.Empty, "/item3" };
            var context = new ResolutionContext(null, null);

            // Act
            var result = converter.Convert(sourceMember, context);

            // Assert
            Assert.Equal(expectedResult, result);
        }
    }
}

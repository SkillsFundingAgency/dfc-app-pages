using AutoMapper;
using DFC.App.Pages.AutoMapperProfiles.ValuerConverters;
using Xunit;

namespace DFC.App.Pages.UnitTests.AutoMapperTests
{
    [Trait("Category", "AutoMapper")]
    public class PageLocationConverterTests
    {
        [Fact]
        public void PageLocationConverterTestsReturnsSlashForNullSourceMember()
        {
            // Arrange
            const string expectedResult = "/";
            var converter = new PageLocationConverter();
            string? sourceMember = null;
            var context = new ResolutionContext(null, null);

            // Act
            var result = converter.Convert(sourceMember, context);

            // Assert
            Assert.Equal(expectedResult, result);
        }

        [Fact]
        public void PageLocationConverterTestsReturnsSlashForNoSlashes()
        {
            // Arrange
            const string expectedResult = "/";
            var converter = new PageLocationConverter();
            var sourceMember = "root-branch";
            var context = new ResolutionContext(null, null);

            // Act
            var result = converter.Convert(sourceMember, context);

            // Assert
            Assert.Equal(expectedResult, result);
        }

        [Fact]
        public void PageLocationConverterTestsReturnsSlashForRootLevelOnly()
        {
            // Arrange
            const string expectedResult = "/";
            var converter = new PageLocationConverter();
            var sourceMember = "/root";
            var context = new ResolutionContext(null, null);

            // Act
            var result = converter.Convert(sourceMember, context);

            // Assert
            Assert.Equal(expectedResult, result);
        }

        [Fact]
        public void PageLocationConverterTestsReturnsRootFromPath()
        {
            // Arrange
            const string expectedResult = "/root";
            var converter = new PageLocationConverter();
            var sourceMember = "/root/leaf";
            var context = new ResolutionContext(null, null);

            // Act
            var result = converter.Convert(sourceMember, context);

            // Assert
            Assert.Equal(expectedResult, result);
        }

        [Fact]
        public void PageLocationConverterTestsReturnsRootBranchFromPath()
        {
            // Arrange
            const string expectedResult = "/root/branch";
            var converter = new PageLocationConverter();
            var sourceMember = "/root/branch/leaf";
            var context = new ResolutionContext(null, null);

            // Act
            var result = converter.Convert(sourceMember, context);

            // Assert
            Assert.Equal(expectedResult, result);
        }
    }
}

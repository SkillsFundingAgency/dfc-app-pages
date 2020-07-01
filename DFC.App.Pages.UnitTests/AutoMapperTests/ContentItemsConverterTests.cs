using AutoMapper;
using DFC.App.Pages.AutoMapperProfiles.ValuerConverters;
using DFC.App.Pages.Data.Models;
using System;
using System.Collections.Generic;
using Xunit;

namespace DFC.App.Pages.UnitTests
{
    [Trait("Category", "AutoMapper")]
    public class ContentItemsConverterTests
    {
        [Fact]
        public void ContentItemsConverterTestsWithAlignmentReturnsSuccess()
        {
            // Arrange
            var expectedResult = "<div class=\"govuk-grid-column-one-half\"><div class=\"dfc-app-pages-alignment-centre\">this is content</div></div>";
            var converter = new ContentItemsConverter();
            IList<ContentItemModel> sourceMember = new List<ContentItemModel>
            {
                new ContentItemModel
                {
                    ItemId = Guid.NewGuid(),
                    Url = new Uri("https://somewhere.com/some-item"),
                    Ordinal = 1,
                    Alignment = "Centre",
                    Size = 50,
                    Content = "this is content",
                },
            };
            var context = new ResolutionContext(null, null);

            // Act
            var result = converter.Convert(sourceMember, context);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(expectedResult, result!.Value);
        }

        [Fact]
        public void ContentItemsConverterTestsWithoutAlignmentReturnsSuccess()
        {
            // Arrange
            var expectedResult = "<div class=\"govuk-grid-column-one-half\">this is content</div>";
            var converter = new ContentItemsConverter();
            IList<ContentItemModel> sourceMember = new List<ContentItemModel>
            {
                new ContentItemModel
                {
                    ItemId = Guid.NewGuid(),
                    Url = new Uri("https://somewhere.com/some-item"),
                    Ordinal = 1,
                    Size = 50,
                    Content = "this is content",
                },
            };
            var context = new ResolutionContext(null, null);

            // Act
            var result = converter.Convert(sourceMember, context);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(expectedResult, result!.Value);
        }

        [Fact]
        public void ContentItemsConverterTestsReturnsNullForNullSourceMember()
        {
            // Arrange
            var converter = new ContentItemsConverter();
            IList<ContentItemModel>? sourceMember = null;
            var context = new ResolutionContext(null, null);

            // Act
            var result = converter.Convert(sourceMember, context);

            // Assert
            Assert.Null(result);
        }

        [Fact]
        public void ContentItemsConverterTestsWithChildReturnsSuccess()
        {
            // Arrange
            var expectedResult = "<div class=\"govuk-grid-column-one-half\"><div class=\"dfc-app-pages-alignment-centre\">Test</div></div>";
            var converter = new ContentItemsConverter();
            IList<ContentItemModel> sourceMember = new List<ContentItemModel>
            {
                new ContentItemModel
                {
                    ItemId = Guid.NewGuid(),
                    Url = new Uri("https://somewhere.com/some-item"),
                    Ordinal = 1,
                    Alignment = "Centre",
                    Size = 50,
                    Content = "",
                    ContentItems = new List<SharedContentItemModel>
                    {
                        new SharedContentItemModel
                        {
                            Content = "Test",
                        },
                    },
                },
            };
            var context = new ResolutionContext(null, null);

            // Act
            var result = converter.Convert(sourceMember, context);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(expectedResult, result!.Value);
        }
    }
}

using AutoMapper;
using DFC.App.Pages.AutoMapperProfiles.ValuerConverters;
using DFC.App.Pages.Data.Models;
using Microsoft.AspNetCore.Html;
using System;
using System.Collections.Generic;
using Xunit;

namespace DFC.App.Pages.UnitTests
{
    [Trait("Category", "Automapper")]
    public class ContentItemsConverterTests
    {
        [Fact]
        public void ContentItemsConverterTestsReturnsSuccess()
        {
            // Arrange
            var expectedResult = new HtmlString("<div class=\"govuk-grid-column-one-half\">this is content</div>");
            var converter = new ContentItemsConverter();
            IList<ContentItemModel> sourceMember = new List<ContentItemModel>
            {
                new ContentItemModel
                {
                    ItemId = Guid.NewGuid(),
                    Url = new Uri("https://somewhere.com/some-item"),
                    Ordinal = 1,
                    Justify = 1,
                    Width = 50,
                    Content = "this is content",
                },
            };
            var context = new ResolutionContext(null, null);

            // Act
            var result = converter.Convert(sourceMember, context);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(expectedResult.Value, result!.Value);
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
    }
}

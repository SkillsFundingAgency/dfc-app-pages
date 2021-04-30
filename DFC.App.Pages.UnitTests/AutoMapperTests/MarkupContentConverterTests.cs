using AutoMapper;
using DFC.App.Pages.AutoMapperProfiles.ValuerConverters;
using DFC.App.Pages.Data.Common;
using DFC.App.Pages.Data.Models;
using System;
using System.Collections.Generic;
using Xunit;

namespace DFC.App.Pages.UnitTests
{
    [Trait("Category", "AutoMapper")]
    public class MarkupContentConverterTests
    {
        [Theory]
        [InlineData(Constants.ContentTypeHtml, "<div class=\"govuk-grid-column-one-half\"><div class=\"dfc-app-pages-alignment-centre\">this is content</div></div>")]
        [InlineData(Constants.ContentTypeHtmlShared, "<div class=\"govuk-grid-column-one-half\"><div class=\"dfc-app-pages-alignment-centre\">this is content</div></div>")]
        [InlineData(Constants.ContentTypeSharedContent, "<div class=\"govuk-grid-column-one-half\"><div class=\"dfc-app-pages-alignment-centre\">this is content</div></div>")]
        [InlineData(Constants.ContentTypeForm, "<div class=\"govuk-grid-column-one-half\"><div class=\"dfc-app-pages-alignment-centre\"><form action=\"an action\" method=\"a method\" enctype=\"an enc type\"></form></div></div>")]
        public void ContentItemsConverterTestsWithAlignmentReturnsSuccess(string contentType, string expectedResult)
        {
            // Arrange
            var converter = new MarkupContentConverter();
            IList<ContentItemModel> sourceMember = new List<ContentItemModel>
            {
                new ContentItemModel
                {
                    ItemId = Guid.NewGuid(),
                    ContentType = contentType,
                    Url = new Uri("https://somewhere.com/some-item"),
                    Ordinal = 1,
                    Alignment = "Centre",
                    Size = 50,
                    Content = "this is content",
                    HtmlBody = "this is html body",
                    Action = "an action",
                    Method = "a method",
                    EncType = "an enc type",
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
            var converter = new MarkupContentConverter();
            IList<ContentItemModel> sourceMember = new List<ContentItemModel>
            {
                new ContentItemModel
                {
                    ItemId = Guid.NewGuid(),
                    ContentType = Constants.ContentTypeHtml,
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
            var converter = new MarkupContentConverter();
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
            var expectedResult = "<div class=\"govuk-grid-column-one-half\"><div class=\"dfc-app-pages-alignment-centre\">some contentsome more html body</div></div>";
            var converter = new MarkupContentConverter();
            IList<ContentItemModel> sourceMember = new List<ContentItemModel>
            {
                new ContentItemModel
                {
                    ItemId = Guid.NewGuid(),
                    ContentType = Constants.ContentTypeHtml,
                    Url = new Uri("https://somewhere.com/some-item"),
                    Ordinal = 1,
                    Alignment = "Centre",
                    Size = 50,
                    Content = string.Empty,
                    ContentItems = new List<ContentItemModel>
                    {
                        new ContentItemModel
                        {
                            ContentType = Constants.ContentTypeHtml,
                            Content = "some content",
                            HtmlBody = "some html body",
                        },
                        new ContentItemModel
                        {
                            ContentType = Constants.ContentTypeHtmlShared,
                            Content = null,
                            HtmlBody = "some more html body",
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

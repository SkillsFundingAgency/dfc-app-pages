using AutoMapper;
using DFC.App.Pages.AutoMapperProfiles;
using DFC.App.Pages.AutoMapperProfiles.ValuerConverters;
using DFC.App.Pages.Data.Common;
using DFC.App.Pages.Data.Models;
using DFC.App.Pages.Data.Models.CmsApiModels;
using DFC.Content.Pkg.Netcore.Data.Contracts;
using System;
using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace DFC.App.Pages.UnitTests.AutoMapperTests
{
    [Trait("Category", "AutoMapper")]
    public class MarkupContentItemsConverterTests
    {
        [Fact]
        public void MarkupContentItemsConverterReturnsNullForNullSourceMember()
        {
            // Arrange
            var converter = new MarkupContentItemsConverter();
            IList<IBaseContentItemModel>? sourceMember = null;
            var context = new ResolutionContext(null, null);

            // Act
            var result = converter.Convert(sourceMember, context);

            // Assert
            Assert.Null(result);
        }

        [Fact]
        public void MarkupContentItemsConverterReturnsNullForNullContext()
        {
            // Arrange
            var converter = new MarkupContentItemsConverter();
            IList<IBaseContentItemModel>? sourceMember = null;
            ResolutionContext? context = null;

            // Act
            Assert.Throws<ArgumentNullException>(() => converter.Convert(sourceMember, context));
        }

        [Fact]
        public void MarkupContentItemsConverterReturnsNullForNoContentItems()
        {
            // Arrange
            var converter = new MarkupContentItemsConverter();
            var sourceMember = new List<IBaseContentItemModel>();
            var context = new ResolutionContext(null, null);

            // Act
            var result = converter.Convert(sourceMember, context);

            // Assert
            Assert.Null(result);
        }

        [Fact]
        public void MarkupContentHtmlItemsConverterReturnsSuccessForContentItems()
        {
            // Arrange
            var converter = new MarkupContentItemsConverter();
            var sourceMember = new List<IBaseContentItemModel>
            {
                new CmsApiHtmlModel
                {
                    ContentType = Constants.ContentTypeHtml,
                    Content = "some content",
                    HtmlBody = "an html body",
                    Title = "a Title",
                },
            };
            var expectedResult = new List<ContentItemModel>
            {
                new ContentItemModel
                {
                    ContentType = Constants.ContentTypeHtml,
                    Content = "some content",
                    HtmlBody = "an html body",
                    Title = "a Title",
                },
            };
            var configuration = new MapperConfiguration(cfg => cfg.AddProfile<ContentPageModelProfile>());
            var context = new Mapper(configuration);

            // Act
            var result = converter.Convert(sourceMember, context.DefaultContext);

            // Assert
            Assert.Equal(expectedResult.First().ContentType, result.First().ContentType);
            Assert.Equal(expectedResult.First().Content, result.First().Content);
            Assert.Equal(expectedResult.First().HtmlBody, result.First().HtmlBody);
            Assert.Equal(expectedResult.First().Title, result.First().Title);
        }

        [Fact]
        public void MarkupContentHtmlSharedItemsConverterReturnsSuccessForContentItems()
        {
            // Arrange
            var converter = new MarkupContentItemsConverter();
            var sourceMember = new List<IBaseContentItemModel>
            {
                new CmsApiHtmlSharedModel
                {
                    ContentType = Constants.ContentTypeHtmlShared,
                    Content = "some content",
                    HtmlBody = "an html body",
                    Title = "a Title",
                },
            };
            var expectedResult = new List<ContentItemModel>
            {
                new ContentItemModel
                {
                    ContentType = Constants.ContentTypeHtmlShared,
                    Content = "some content",
                    HtmlBody = "an html body",
                    Title = "a Title",
                },
            };
            var configuration = new MapperConfiguration(cfg => cfg.AddProfile<ContentPageModelProfile>());
            var context = new Mapper(configuration);

            // Act
            var result = converter.Convert(sourceMember, context.DefaultContext);

            // Assert
            Assert.Equal(expectedResult.First().ContentType, result.First().ContentType);
            Assert.Equal(expectedResult.First().Content, result.First().Content);
            Assert.Equal(expectedResult.First().HtmlBody, result.First().HtmlBody);
            Assert.Equal(expectedResult.First().Title, result.First().Title);
        }

        [Fact]
        public void MarkupContentSharedContentItemsConverterReturnsSuccessForContentItems()
        {
            // Arrange
            var converter = new MarkupContentItemsConverter();
            var sourceMember = new List<IBaseContentItemModel>
            {
                new CmsApiSharedContentModel
                {
                    ContentType = Constants.ContentTypeSharedContent,
                    Content = "some content",
                    HtmlBody = "an html body",
                    Title = "a Title",
                },
            };
            var expectedResult = new List<ContentItemModel>
            {
                new ContentItemModel
                {
                    ContentType = Constants.ContentTypeSharedContent,
                    Content = "some content",
                    HtmlBody = "an html body",
                    Title = "a Title",
                },
            };
            var configuration = new MapperConfiguration(cfg => cfg.AddProfile<ContentPageModelProfile>());
            var context = new Mapper(configuration);

            // Act
            var result = converter.Convert(sourceMember, context.DefaultContext);

            // Assert
            Assert.Equal(expectedResult.First().ContentType, result.First().ContentType);
            Assert.Equal(expectedResult.First().Content, result.First().Content);
            Assert.Equal(expectedResult.First().HtmlBody, result.First().HtmlBody);
            Assert.Equal(expectedResult.First().Title, result.First().Title);
        }

        [Fact]
        public void MarkupContentFormItemsConverterReturnsSuccessForContentItems()
        {
            // Arrange
            var converter = new MarkupContentItemsConverter();
            var sourceMember = new List<IBaseContentItemModel>
            {
                new CmsApiFormModel
                {
                    ContentType = Constants.ContentTypeForm,
                    Action = "some action",
                    EnableAntiForgeryToken = true,
                    Method = "a method",
                    EncType = "an EncType",
                },
            };
            var expectedResult = new List<ContentItemModel>
            {
                new ContentItemModel
                {
                    ContentType = Constants.ContentTypeForm,
                    Action = "some action",
                    EnableAntiForgeryToken = true,
                    Method = "a method",
                    EncType = "an EncType",
                },
            };
            var configuration = new MapperConfiguration(cfg => cfg.AddProfile<ContentPageModelProfile>());
            var context = new Mapper(configuration);

            // Act
            var result = converter.Convert(sourceMember, context.DefaultContext);

            // Assert
            Assert.Equal(expectedResult.First().ContentType, result.First().ContentType);
            Assert.Equal(expectedResult.First().Action, result.First().Action);
            Assert.Equal(expectedResult.First().EnableAntiForgeryToken, result.First().EnableAntiForgeryToken);
            Assert.Equal(expectedResult.First().Method, result.First().Method);
            Assert.Equal(expectedResult.First().EncType, result.First().EncType);
        }
    }
}

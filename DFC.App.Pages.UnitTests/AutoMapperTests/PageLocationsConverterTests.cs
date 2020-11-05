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
    public class PageLocationsConverterTests
    {
        [Fact]
        public void PageLocationsConverterReturnsNullForNullSourceMember()
        {
            // Arrange
            var converter = new PageLocationsConverter();
            IList<IBaseContentItemModel>? sourceMember = null;
            var context = new ResolutionContext(null, null);

            // Act
            var result = converter.Convert(sourceMember, context);

            // Assert
            Assert.Null(result);
        }

        [Fact]
        public void PageLocationsConverterReturnsNullForNullContext()
        {
            // Arrange
            var converter = new PageLocationsConverter();
            IList<IBaseContentItemModel>? sourceMember = null;
            ResolutionContext? context = null;

            // Act
            Assert.Throws<ArgumentNullException>(() => converter.Convert(sourceMember, context));
        }

        [Fact]
        public void PageLocationsConverterReturnsNullForNoContentItems()
        {
            // Arrange
            var converter = new PageLocationsConverter();
            var sourceMember = new List<IBaseContentItemModel>();
            var context = new ResolutionContext(null, null);

            // Act
            var result = converter.Convert(sourceMember, context);

            // Assert
            Assert.Null(result);
        }

        [Fact]
        public void PageLocationsConverterReturnsSuccessForContentItems()
        {
            // Arrange
            var converter = new PageLocationsConverter();
            var sourceMember = new List<IBaseContentItemModel>
            {
                new CmsApiPageLocationModel
                {
                    ContentType = Constants.ContentTypePageLocation,
                    BreadcrumbText = "breadcrumb-text",
                    Title = "Breadcrumb Title",
                },
            };
            var expectedResult = new List<PageLocationModel>
            {
                new PageLocationModel
                {
                    ContentType = Constants.ContentTypePageLocation,
                    BreadcrumbText = "breadcrumb-text",
                    BreadcrumbLinkSegment = "Breadcrumb Title",
                },
            };
            var configuration = new MapperConfiguration(cfg => cfg.AddProfile<ContentPageModelProfile>());
            var context = new Mapper(configuration);

            // Act
            var result = converter.Convert(sourceMember, context.DefaultContext);

            // Assert
            Assert.Equal(expectedResult.First().ContentType, result.First().ContentType);
            Assert.Equal(expectedResult.First().BreadcrumbLinkSegment, result.First().BreadcrumbLinkSegment);
            Assert.Equal(expectedResult.First().BreadcrumbText, result.First().BreadcrumbText);
        }
    }
}

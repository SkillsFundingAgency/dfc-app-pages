using AutoMapper;
using DFC.App.Pages.AutoMapperProfiles.ValuerConverters;
using DFC.App.Pages.Data.Common;
using DFC.App.Pages.Data.Models;
using DFC.App.Pages.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace DFC.App.Pages.UnitTests.AutoMapperTests
{
    [Trait("Category", "AutoMapper")]
    public class BreadcrumbConverterTests
    {
        [Fact]
        public void BreadcrumbConverterReturnsNullForNullSourceMember()
        {
            // Arrange
            var converter = new BreadcrumbConverter();
            ContentPageModel? sourceMember = null;
            var context = new ResolutionContext(null, null);

            // Act
            var result = converter.Convert(sourceMember, context);

            // Assert
            Assert.Null(result);
        }

        [Fact]
        public void BreadcrumbConverterReturnsNullForNoContentItems()
        {
            // Arrange
            var converter = new BreadcrumbConverter();
            var sourceMember = new ContentPageModel();
            var context = new ResolutionContext(null, null);

            // Act
            var result = converter.Convert(sourceMember, context);

            // Assert
            Assert.Null(result);
        }

        [Fact]
        public void BreadcrumbConverterReturnsSuccessForContentItems()
        {
            // Arrange
            var converter = new BreadcrumbConverter();
            var sourceMember = BuildContentPageModel();
            var context = new ResolutionContext(null, null);
            var expectedResult = new List<BreadcrumbItemViewModel>
            {
                new BreadcrumbItemViewModel
                {
                    Route = "/",
                    Title = "Home",
                    AddHyperlink = true,
                },
                new BreadcrumbItemViewModel
                {
                    Route = "/segment-1",
                    Title = "Segment #1",
                    AddHyperlink = true,
                },
                new BreadcrumbItemViewModel
                {
                    Route = "/segment-1/segment-2",
                    Title = "Segment #2",
                    AddHyperlink = true,
                },
                new BreadcrumbItemViewModel
                {
                    Route = "/segment-1/segment-2/segment-3",
                    Title = "Segment #3",
                    AddHyperlink = true,
                },
                new BreadcrumbItemViewModel
                {
                    Route = "/segment-1/segment-2/segment-3/a-canonical-name",
                    Title = "A page title",
                    AddHyperlink = false,
                },
            };

            // Act
            var result = converter.Convert(sourceMember, context);

            // Assert
            Assert.Equal(expectedResult.Last().Route, result.Last().Route);
        }

        [Fact]
        public void BreadcrumbConverterReturnsSuccessForContentItemsWithNoPageTitle()
        {
            // Arrange
            var converter = new BreadcrumbConverter();
            var sourceMember = BuildContentPageModel();
            var context = new ResolutionContext(null, null);
            var expectedResult = new List<BreadcrumbItemViewModel>
            {
                new BreadcrumbItemViewModel
                {
                    Route = "/",
                    Title = "Home",
                    AddHyperlink = true,
                },
                new BreadcrumbItemViewModel
                {
                    Route = "/segment-1",
                    Title = "Segment #1",
                    AddHyperlink = true,
                },
                new BreadcrumbItemViewModel
                {
                    Route = "/segment-1/segment-2",
                    Title = "Segment #2",
                    AddHyperlink = true,
                },
                new BreadcrumbItemViewModel
                {
                    Route = "/segment-1/segment-2/segment-3",
                    Title = "Segment #3",
                    AddHyperlink = true,
                },
            };

            sourceMember.MetaTags.Title = null;

            // Act
            var result = converter.Convert(sourceMember, context);

            // Assert
            Assert.Equal(expectedResult.Last().Route, result.Last().Route);
        }

        private ContentPageModel BuildContentPageModel()
        {
            var item = new ContentPageModel
            {
                Id = Guid.NewGuid(),
                CanonicalName = "a-canonical-name",
                MetaTags = new Compui.Cosmos.Models.MetaTagsModel
                {
                    Title = "A page title",
                },
                PageLocations = new List<PageLocationModel>
                {
                    new PageLocationModel
                    {
                        ItemId = Guid.NewGuid(),
                        ContentType = Constants.ContentTypePageLocation,
                        BreadcrumbLinkSegment = "segment-3",
                        BreadcrumbText = "Segment #3",
                        PageLocations = new List<PageLocationModel>
                        {
                            new PageLocationModel
                            {
                                ItemId = Guid.NewGuid(),
                                ContentType = Constants.ContentTypePageLocation,
                                BreadcrumbLinkSegment = "segment-2",
                                BreadcrumbText = "Segment #2",
                                PageLocations = new List<PageLocationModel>
                                {
                                    new PageLocationModel
                                    {
                                        ItemId = Guid.NewGuid(),
                                        ContentType = Constants.ContentTypePageLocation,
                                        BreadcrumbLinkSegment = "segment-1",
                                        BreadcrumbText = "Segment #1",
                                        PageLocations = new List<PageLocationModel>
                                        {
                                            new PageLocationModel
                                            {
                                                ItemId = Guid.NewGuid(),
                                                ContentType = Constants.ContentTypePageLocation,
                                                BreadcrumbLinkSegment = "/",
                                                BreadcrumbText = "Home",
                                            },
                                        },
                                    },
                                },
                            },
                        },
                    },
                },
            };

            return item;
        }
    }
}

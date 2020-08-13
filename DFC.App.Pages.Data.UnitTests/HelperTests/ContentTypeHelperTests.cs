using DFC.App.Pages.Data.Enums;
using DFC.App.Pages.Data.Helpers;
using System;
using System.Collections.Generic;
using System.IO;
using Xunit;

namespace DFC.App.Pages.Data.UnitTests.HelperTests
{
    [Trait("Category", "ContentTypeHelper Unit Tests")]
    public class ContentTypeHelperTests
    {
        [Fact]
        public void ContentTypeHelperWhenPassedSharedContentReturnsSharedContent()
        {
            // Arrange
            var url = new Uri($"http://somewhere.com/someresource/sharedcontent/{Guid.NewGuid()}");

            // Act
            var result = url.GetContentTypeFromUrl();

            // Assert
            Assert.Equal(EventContentType.SharedContent, result);
        }

        [Fact]
        public void ContentTypeHelperWhenPassedPageLocationReturnsPageLocation()
        {
            // Arrange
            var url = new Uri($"http://somewhere.com/someresource/pagelocation/{Guid.NewGuid()}");

            // Act
            var result = url.GetContentTypeFromUrl();

            // Assert
            Assert.Equal(EventContentType.PageLocation, result);
        }

        [Fact]
        public void ContentTypeHelperWhenPassedPageReturnsPage()
        {
            // Arrange
            var url = new Uri($"http://somewhere.com/someresource/page/{Guid.NewGuid()}");

            // Act
            var result = url.GetContentTypeFromUrl();

            // Assert
            Assert.Equal(EventContentType.Page, result);
        }

        [Fact]
        public void ContentTypeHelperWhenPassedIncorrectStringThrowsException()
        {
            // Arrange
            var url = new Uri($"http://somewhere.com/someresource/qwerty/{Guid.NewGuid()}");

            // Act
            // Assert
            Assert.Throws<InvalidDataException>(() => url.GetContentTypeFromUrl());
        }
    }
}

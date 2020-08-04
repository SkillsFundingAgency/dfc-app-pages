using DFC.App.Pages.Data.Common;
using DFC.App.Pages.Data.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Globalization;
using System.Linq;
using Xunit;

namespace DFC.App.Pages.Data.UnitTests.ValidationTests
{
    [Trait("Category", "ContentPageModel Validation Unit Tests")]
    public class ContentPageModelValidationTests
    {
        private const string GuidEmpty = "00000000-0000-0000-0000-000000000000";

        [Theory]
        [InlineData(null)]
        [InlineData(GuidEmpty)]
        public void CanCheckIfDocumentIdIsInvalid(Guid documentId)
        {
            // Arrange
            var model = CreateModel(documentId, "location1", "canonicalname1", "content1", new List<string>());

            // Act
            var vr = Validate(model);

            // Assert
            Assert.True(vr.Count == 1);
            Assert.NotNull(vr.First(f => f.MemberNames.Any(a => a == nameof(model.Id))));
            Assert.Equal(string.Format(CultureInfo.InvariantCulture, ValidationMessage.FieldInvalidGuid, nameof(model.Id)), vr.First(f => f.MemberNames.Any(a => a == nameof(model.Id))).ErrorMessage);
        }

        [Theory]
        [InlineData("abcdefghijklmnopqrstuvwxyz")]
        [InlineData("0123456789")]
        [InlineData("abc")]
        [InlineData("xyz123")]
        [InlineData("abc_def")]
        [InlineData("abc-def")]
        public void CanCheckIfPageLocationIsValid(string pageLocation)
        {
            // Arrange
            var model = CreateModel(Guid.NewGuid(), pageLocation, "canonicalname1", "content", new List<string>());

            // Act
            var vr = Validate(model);

            // Assert
            Assert.True(vr.Count == 0);
        }

        [Theory]
        [InlineData("ABCDEF")]
        public void CanCheckIfPageLocationIsInvalid(string pageLocation)
        {
            // Arrange
            var model = CreateModel(Guid.NewGuid(), pageLocation, "canonicalname1", "content", new List<string>());

            // Act
            var vr = Validate(model);

            // Assert
            Assert.True(vr.Count > 0);
            Assert.NotNull(vr.First(f => f.MemberNames.Any(a => a == nameof(model.PageLocation))));
            Assert.Equal(string.Format(CultureInfo.InvariantCulture, ValidationMessage.FieldNotLowercase, nameof(model.PageLocation)), vr.First(f => f.MemberNames.Any(a => a == nameof(model.PageLocation))).ErrorMessage);
        }

        [Theory]
        [InlineData("abcdefghijklmnopqrstuvwxyz")]
        [InlineData("0123456789")]
        [InlineData("abc")]
        [InlineData("xyz123")]
        [InlineData("abc_def")]
        [InlineData("abc-def")]
        public void CanCheckIfCanonicalNameIsValid(string canonicalName)
        {
            // Arrange
            var model = CreateModel(Guid.NewGuid(), "location1", canonicalName, "content", new List<string>());

            // Act
            var vr = Validate(model);

            // Assert
            Assert.True(vr.Count == 0);
        }

        [Theory]
        [InlineData("ABCDEF")]
        public void CanCheckIfCanonicalNameIsInvalid(string canonicalName)
        {
            // Arrange
            var model = CreateModel(Guid.NewGuid(), "location1", canonicalName, "content", new List<string>());

            // Act
            var vr = Validate(model);

            // Assert
            Assert.True(vr.Count > 0);
            Assert.NotNull(vr.First(f => f.MemberNames.Any(a => a == nameof(model.CanonicalName))));
            Assert.Equal(string.Format(CultureInfo.InvariantCulture, ValidationMessage.FieldNotLowercase, nameof(model.CanonicalName)), vr.First(f => f.MemberNames.Any(a => a == nameof(model.CanonicalName))).ErrorMessage);
        }

        [Theory]
        [InlineData("abcdefghijklmnopqrstuvwxyz")]
        [InlineData("0123456789")]
        [InlineData("abc")]
        [InlineData("xyz123")]
        [InlineData("abc_def")]
        [InlineData("abc-def")]
        public void CanCheckIfRedirectLocationsIsValid(string redirectLocations)
        {
            // Arrange
            var model = CreateModel(Guid.NewGuid(), "location1", "canonicalname1", "content1", new List<string>() { redirectLocations });

            // Act
            var vr = Validate(model);

            // Assert
            Assert.True(vr.Count == 0);
        }

        [Theory]
        [InlineData("ABCDEF")]
        public void CanCheckIfRedirectLocationsIsInvalid(string redirectLocations)
        {
            // Arrange
            var model = CreateModel(Guid.NewGuid(), "location1", "canonicalname1", "content1", new List<string>() { redirectLocations });

            // Act
            var vr = Validate(model);

            // Assert
            Assert.True(vr.Count > 0);
            Assert.NotNull(vr.First(f => f.MemberNames.Any(a => a == nameof(model.RedirectLocations))));
            Assert.Equal(string.Format(CultureInfo.InvariantCulture, ValidationMessage.FieldNotLowercase, nameof(model.RedirectLocations)), vr.First(f => f.MemberNames.Any(a => a == nameof(model.RedirectLocations))).ErrorMessage);
        }

        private ContentPageModel CreateModel(Guid documentId, string pageLocation, string canonicalName, string content, List<string> redirectLocations)
        {
            var model = new ContentPageModel
            {
                Id = documentId,
                PageLocation = pageLocation,
                CanonicalName = canonicalName,
                Version = Guid.NewGuid(),
                Url = new Uri("aaa-bbb", UriKind.Relative),
                Content = content,
                RedirectLocations = redirectLocations.ToArray(),
                LastReviewed = DateTime.UtcNow,
            };

            return model;
        }

        private List<ValidationResult> Validate(ContentPageModel model)
        {
            var vr = new List<ValidationResult>();
            var vc = new ValidationContext(model);
            Validator.TryValidateObject(model, vc, vr, true);

            return vr;
        }
    }
}

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
            var model = CreateModel(documentId, "canonicalname1", "content1", new List<string>());

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
        public void CanCheckIfCanonicalNameIsValid(string canonicalName)
        {
            // Arrange
            var model = CreateModel(Guid.NewGuid(), canonicalName, "content", new List<string>());

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
            var model = CreateModel(Guid.NewGuid(), canonicalName, "content", new List<string>());

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
        public void CanCheckIfAlternativeNameIsValid(string alternativeName)
        {
            // Arrange
            var model = CreateModel(Guid.NewGuid(), "canonicalname1", "content1", new List<string>() { alternativeName });

            // Act
            var vr = Validate(model);

            // Assert
            Assert.True(vr.Count == 0);
        }

        [Theory]
        [InlineData("ABCDEF")]
        public void CanCheckIfAlternativeNameIsInvalid(string alternativeName)
        {
            // Arrange
            var model = CreateModel(Guid.NewGuid(), "canonicalname1", "content1", new List<string>() { alternativeName });

            // Act
            var vr = Validate(model);

            // Assert
            Assert.True(vr.Count > 0);
            Assert.NotNull(vr.First(f => f.MemberNames.Any(a => a == nameof(model.AlternativeNames))));
            Assert.Equal(string.Format(CultureInfo.InvariantCulture, ValidationMessage.FieldNotLowercase, nameof(model.AlternativeNames)), vr.First(f => f.MemberNames.Any(a => a == nameof(model.AlternativeNames))).ErrorMessage);
        }

        private ContentPageModel CreateModel(Guid documentId, string canonicalName, string content, List<string> alternativeNames)
        {
            var model = new ContentPageModel
            {
                Id = documentId,
                CanonicalName = canonicalName,
                BreadcrumbTitle = canonicalName,
                Version = Guid.NewGuid(),
                Url = new Uri("aaa-bbb", UriKind.Relative),
                Content = content,
                AlternativeNames = alternativeNames.ToArray(),
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

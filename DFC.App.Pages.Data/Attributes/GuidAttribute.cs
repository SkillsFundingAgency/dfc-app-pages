using DFC.App.Pages.Data.Common;
using System;
using System.ComponentModel.DataAnnotations;
using System.Globalization;

namespace DFC.App.Pages.Data.Attributes
{
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
    public class GuidAttribute : ValidationAttribute
    {
        protected override ValidationResult IsValid(object? value, ValidationContext? validationContext)
        {
            if (value == null)
            {
                throw new ArgumentNullException(nameof(value));
            }

            if (validationContext == null)
            {
                throw new ArgumentNullException(nameof(validationContext));
            }

            if (!Guid.TryParse(value.ToString(), out var guid) || guid == Guid.Empty)
            {
                return new ValidationResult(string.Format(CultureInfo.InvariantCulture, ValidationMessage.FieldInvalidGuid, validationContext.DisplayName), new[] { validationContext.MemberName });
            }

            return ValidationResult.Success;
        }
    }
}

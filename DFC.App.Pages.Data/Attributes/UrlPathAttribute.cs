using DFC.App.Pages.Data.Common;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Globalization;
using System.Linq;

namespace DFC.App.Pages.Data.Attributes
{
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
    public class UrlPathAttribute : ValidationAttribute
    {
        protected override ValidationResult IsValid(object? value, ValidationContext? validationContext)
        {
            if (value == null)
            {
                return ValidationResult.Success;
            }

            if (validationContext == null)
            {
                throw new ArgumentNullException(nameof(validationContext));
            }

            var validChars = "abcdefghijklmnopqrstuvwxyz01234567890_-";
            var result = value switch
            {
                IEnumerable<string> list => list.All(x => x.Length > 0 && x.All(y => validChars.Contains(y, StringComparison.OrdinalIgnoreCase))),
                _ => value.ToString().All(x => validChars.Contains(x, StringComparison.OrdinalIgnoreCase)),
            };

            return result ? ValidationResult.Success
                : new ValidationResult(string.Format(CultureInfo.InvariantCulture, ValidationMessage.FieldNotUrlPath, validationContext.DisplayName, validChars), new[] { validationContext.MemberName });
        }
    }
}

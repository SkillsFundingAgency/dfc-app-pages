namespace DFC.App.Pages.Data.Common
{
    public static class ValidationMessage
    {
        public const string FieldInvalidGuid = "The field {0} has to be a valid GUID and cannot be an empty GUID.";
        public const string FieldNotLowercase = "The field {0} is not in lowercase.";
        public const string FieldNotUrlPath = "The field {0} does not contains valid characters for a url path. Valid characters are {1}";
    }
}

using System.Diagnostics.CodeAnalysis;

namespace DFC.App.Pages.ViewModels
{
    [ExcludeFromCodeCoverage]
    public class IndexDocumentViewModel
    {
        public bool IsDefaultForPageLocation { get; set; }

        public string? PageLocation { get; set; }

        public string? CanonicalName { get; set; }
    }
}

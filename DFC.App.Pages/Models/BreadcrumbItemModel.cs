using System.Diagnostics.CodeAnalysis;

namespace DFC.App.Pages.Models
{
    [ExcludeFromCodeCoverage]
    public class BreadcrumbItemModel
    {
        public string? CanonicalName { get; set; }

        public string? BreadcrumbTitle { get; set; }
    }
}

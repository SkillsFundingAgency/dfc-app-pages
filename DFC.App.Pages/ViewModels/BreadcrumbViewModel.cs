using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;


namespace DFC.App.Pages.ViewModels
{
    [ExcludeFromCodeCoverage]
    public class BreadcrumbViewModel
    {
        public IList<BreadcrumbPathViewModel>? Paths { get; set; }
    }
}

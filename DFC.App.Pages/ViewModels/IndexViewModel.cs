using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace DFC.App.Pages.ViewModels
{
    [ExcludeFromCodeCoverage]
    public class IndexViewModel
    {
        public string? LocalPath { get; set; }

        public List<IndexDocumentViewModel>? Documents { get; set; }
    }
}

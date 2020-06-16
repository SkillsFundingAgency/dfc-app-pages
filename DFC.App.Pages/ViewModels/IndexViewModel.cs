using System.Collections.Generic;

namespace DFC.App.Pages.ViewModels
{
    public class IndexViewModel
    {
        public string? LocalPath { get; set; }

        public IList<IndexDocumentViewModel>? Documents { get; set; }
    }
}

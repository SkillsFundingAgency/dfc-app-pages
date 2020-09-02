using Microsoft.AspNetCore.Html;
using System.Diagnostics.CodeAnalysis;

namespace DFC.App.Pages.ViewModels
{
    [ExcludeFromCodeCoverage]
    public class HeroBannerViewModel
    {
        public HtmlString? Content { get; set; } = new HtmlString("Unknown content");
    }
}

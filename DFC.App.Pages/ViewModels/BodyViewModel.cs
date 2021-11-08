using Microsoft.AspNetCore.Html;
using System.Diagnostics.CodeAnalysis;

namespace DFC.App.Pages.ViewModels
{
    [ExcludeFromCodeCoverage]
    public class BodyViewModel
    {
        public HtmlString? Content { get; set; } = new HtmlString("Unknown content");

        public bool UseBrowserWidth { get; set; }

        public string WidthContainerClass => UseBrowserWidth ? "govuk-full-width-container" : "govuk-width-container";
    }
}

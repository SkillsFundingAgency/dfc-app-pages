using Microsoft.AspNetCore.Html;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;

namespace DFC.App.Pages.ViewModels
{
    [ExcludeFromCodeCoverage]
    public class HeroBannerViewModel
    {
        [Display(Name = "Show Hero Banner")]
        public bool ShowHeroBanner { get; set; }

        public HtmlString? Content { get; set; } = new HtmlString("Unknown content");
    }
}

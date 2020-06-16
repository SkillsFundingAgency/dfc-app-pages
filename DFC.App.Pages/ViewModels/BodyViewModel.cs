using Microsoft.AspNetCore.Html;

namespace DFC.App.Pages.ViewModels
{
    public class BodyViewModel
    {
        public HtmlString? Content { get; set; } = new HtmlString("Unknown content");
    }
}

using DFC.Content.Pkg.Netcore.Data.Contracts;

namespace DFC.App.Pages.Data.Contracts
{
    public interface ICmsApiMarkupContentItem
    {
        string? Title { get; set; }

        public string? Content { get; set; }

        public string? HtmlBody { get; set; }
    }
}

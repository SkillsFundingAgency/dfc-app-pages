using DFC.App.Pages.Data.Models;
using System.Threading.Tasks;

namespace DFC.App.Pages.Data.Contracts
{
    public interface IPagesControllerHelper
    {
        string GetPageUrl(string location, string article);

        Task<ContentPageModel?> GetContentPageFromSharedAsync(string? location, string? article);
    }
}

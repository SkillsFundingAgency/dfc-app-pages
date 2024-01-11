using DFC.App.Pages.Data.Models;
using System.Threading.Tasks;

namespace DFC.App.Pages.Data.Contracts
{
    public interface IPagesControlerHelpers
    {
        Task<ContentPageModel?> GetContentPageAsync(string? location, string? article);

        Task<ContentPageModel?> GetContentPageFromPageServiceAsync(string? location, string? article);

        Task<ContentPageModel?> GetRedirectedContentPageAsync(string? location, string? article);
    }
}
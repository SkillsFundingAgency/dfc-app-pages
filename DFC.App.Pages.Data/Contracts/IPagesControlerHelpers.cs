using DFC.App.Pages.Data.Models;
using System.Threading.Tasks;

namespace DFC.App.Pages.Data.Contracts
{
    public interface IPagesControlerHelpers
    {
        Task<ContentPageModel?> GetContentPageFromSharedAsync(string? location, string? article);
    }
}
using System.Reflection;

namespace DFC.App.Pages.Cms.Interface
{
    public interface IPageService
    {
        public Task<IList<Model.PageUrl>> GetPageUrls();

        public Task<IList<Model.Page>> GetPage(string path);

        public Task<Model.Item> GetBreadCrumbs(string queryName);
    }
}

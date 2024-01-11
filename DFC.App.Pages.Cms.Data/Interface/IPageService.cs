using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DFC.App.Pages.Cms.Data.Interface
{
    public interface IPageService
    {
        public Task<IList<Model.PageUrl>> GetPageUrls();

        public Task<IList<Model.Page>> GetPage(string path);

        public Task<Model.Item> GetBreadCrumbs(string queryName);
    }
}

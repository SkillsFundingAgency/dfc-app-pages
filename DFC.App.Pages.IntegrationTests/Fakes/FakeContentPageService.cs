using DFC.Compui.Cosmos;
using DFC.Compui.Cosmos.Contracts;
using System;
using System.Threading.Tasks;

namespace DFC.App.Pages.IntegrationTests.Fakes
{
    public class FakeContentPageService<TModel> : DocumentService<TModel>, IContentPageService<TModel>
                                                  where TModel : class, IContentPageModel
    {
        public FakeContentPageService(ICosmosRepository<TModel> repository) : base(repository)
        {
        }

        public Task<TModel?> GetByNameAsync(string? pageLocation, string? canonicalName)
        {
            throw new NotImplementedException();
        }

        public Task<TModel?> GetByNameAsync(string? canonicalName)
        {
            throw new NotImplementedException();
        }

        public Task<TModel?> GetByRedirectLocationAsync(string? redirectLocation)
        {
            throw new NotImplementedException();
        }
    }
}

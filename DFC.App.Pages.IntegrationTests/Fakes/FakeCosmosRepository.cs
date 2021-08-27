using DFC.Compui.Cosmos.Contracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Net;
using System.Threading.Tasks;

namespace DFC.App.Pages.IntegrationTests.Fakes
{

    public class FakeCosmosRepository<TModel> : ICosmosRepository<TModel>
        where TModel : class, IDocumentModel
    {
        public Task<HttpStatusCode> DeleteAsync(Guid id)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<TModel>?> GetAllAsync(string? partitionKeyValue = null)
        {
            IEnumerable<TModel>? result = new List<TModel>();

            return Task.FromResult(result.Any() ? result : default);
        }

        public Task<IEnumerable<TModel>?> GetAsync(Expression<Func<TModel, bool>> where)
        {
            throw new NotImplementedException();
        }

        public Task<TModel?> GetAsync(Expression<Func<TModel, bool>> where, string partitionKeyValue)
        {
            throw new NotImplementedException();
        }

        public Task<TModel?> GetByIdAsync(Guid id, string? partitionKeyValue = null)
        {
            throw new NotImplementedException();
        }

        public Task<bool> PingAsync()
        {
            throw new NotImplementedException();
        }

        public Task<HttpStatusCode> PurgeAsync()
        {
            throw new NotImplementedException();
        }

        public Task<HttpStatusCode> UpsertAsync(TModel model)
        {
            throw new NotImplementedException();
        }
    }
}

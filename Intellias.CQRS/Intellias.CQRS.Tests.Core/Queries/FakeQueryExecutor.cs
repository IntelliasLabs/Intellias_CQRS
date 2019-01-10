using System.Threading.Tasks;
using Intellias.CQRS.Core.Queries;
using Intellias.CQRS.Tests.Core.Fakes;

namespace Intellias.CQRS.Tests.Core.Queries
{
    /// <summary>
    /// Query executor
    /// </summary>
    public class FakeQueryExecutor<TQueryModel> : IReadQueryExecutor<TQueryModel>
        where TQueryModel : class, IQueryModel

    {
        private readonly InProcessQueryStore<TQueryModel> store;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="store"></param>
        public FakeQueryExecutor(InProcessQueryStore<TQueryModel> store)
        {
            this.store = store;
        }

        /// <inheritdoc />
        public Task<TQueryModel> GetByIdAsync(string parentId, string id) =>
            store.GetAsync(parentId, id);

        /// <inheritdoc />
        public Task<TQueryModel> GetByIdAsync(string id) => 
            store.GetAsync(id);

        /// <inheritdoc />
        public Task<CollectionQueryModel<TQueryModel>> GetAllAsync() => 
            store.GetAllAsync();

        /// <inheritdoc />
        public Task<CollectionQueryModel<TQueryModel>> GetAllAsync(string parentId) =>
            store.GetAllAsync(parentId);
    }
}

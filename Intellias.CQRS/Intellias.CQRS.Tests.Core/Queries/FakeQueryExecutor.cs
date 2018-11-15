using System.Threading.Tasks;
using Intellias.CQRS.Core.Queries;
using Intellias.CQRS.Tests.Core.Fakes;

namespace Intellias.CQRS.Core.Tests.Queries
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

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public async Task<CollectionQueryModel<TQueryModel>> GetAllAsync() => await store.GetAllAsync();

        /// <summary>
        /// 
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task<TQueryModel> GetByIdAsync(string id) => await store.GetAsync(id);
    }
}

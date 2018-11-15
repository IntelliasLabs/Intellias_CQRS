using System.Threading.Tasks;
using Intellias.CQRS.Core.Queries;
using Intellias.CQRS.Tests.Core.Fakes;

namespace Intellias.CQRS.Core.Tests.Queries
{
    /// <summary>
    /// Query executor
    /// </summary>
    public class FakeQueryExecutor<TQueryModel> : 
        IQueryExecutor<ReadModelByIdQuery<TQueryModel>, TQueryModel>,
        IQueryExecutor<ReadAllQuery<TQueryModel>, CollectionQueryModel<TQueryModel>>
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
        /// <param name="query"></param>
        /// <returns></returns>
        public async Task<TQueryModel> ExecuteQueryAsync(ReadModelByIdQuery<TQueryModel> query)
        {
            var readModel = await store.GetAsync(query.Id);
            return readModel;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="query"></param>
        /// <returns></returns>
        public async Task<CollectionQueryModel<TQueryModel>> ExecuteQueryAsync(ReadAllQuery<TQueryModel> query)
        {
            var model = await store.GetAllAsync();
            return model;
        }
    }
}

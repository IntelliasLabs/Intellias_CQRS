using System.Threading.Tasks;
using Intellias.CQRS.Core.Queries;
using Intellias.CQRS.Tests.Core.Fakes;
using Intellias.CQRS.Tests.Core.Queries;

namespace Intellias.CQRS.Core.Tests.QueryHandlers
{
    /// <summary>
    /// Query handler
    /// </summary>
    public class DemoQueryExecutor : 
        IQueryExecutor<ReadModelByIdQuery<DemoQueryModel>, DemoQueryModel>,
        IQueryExecutor<ReadAllQuery<DemoQueryModel>, CollectionQueryModel<DemoQueryModel>>
    {
        private readonly DemoReadModelStore store;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="store"></param>
        public DemoQueryExecutor(DemoReadModelStore store)
        {
            this.store = store;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="query"></param>
        /// <returns></returns>
        public async Task<DemoQueryModel> ExecuteQueryAsync(ReadModelByIdQuery<DemoQueryModel> query)
        {
            var readModel = await store.GetAsync(query.Id);
            return readModel;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="query"></param>
        /// <returns></returns>
        public async Task<CollectionQueryModel<DemoQueryModel>> ExecuteQueryAsync(ReadAllQuery<DemoQueryModel> query)
        {
            var model = await store.GetAllAsync();
            return model;
        }
    }
}

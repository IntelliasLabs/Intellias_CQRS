using System.Collections.Generic;
using System.Threading.Tasks;
using Intellias.CQRS.Core.Queries;
using Intellias.CQRS.Tests.Core.Fakes;
using Intellias.CQRS.Tests.Core.Queries;

namespace Intellias.CQRS.Core.Tests.QueryHandlers
{
    /// <summary>
    /// Query handler
    /// </summary>
    public class DemoQueryHandler : IQueryHandler<ReadModelByIdQuery<DemoReadModel>, DemoReadModel>,
        IQueryHandler<ReadAllQuery<DemoReadModel>, IReadOnlyCollection<DemoReadModel>>
    {
        private readonly DemoReadModelStore store;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="store"></param>
        public DemoQueryHandler(DemoReadModelStore store)
        {
            this.store = store;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="query"></param>
        /// <returns></returns>
        public async Task<DemoReadModel> ExecuteQueryAsync(ReadModelByIdQuery<DemoReadModel> query)
        {
            var readModelEnvelope = await store.GetAsync(query.Id);
            return readModelEnvelope.ReadModel;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="query"></param>
        /// <returns></returns>
        public async Task<IReadOnlyCollection<DemoReadModel>> ExecuteQueryAsync(ReadAllQuery<DemoReadModel> query)
        {
            var collectionEnvelope = await store.GetAllAsync();
            return collectionEnvelope.ReadModel;
        }
    }
}

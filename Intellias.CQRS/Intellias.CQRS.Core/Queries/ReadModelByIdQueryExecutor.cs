using System.Threading.Tasks;
using Intellias.CQRS.Core.Storage;

namespace Intellias.CQRS.Core.Queries
{
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="TReadModel"></typeparam>
    /// <typeparam name="TCollectionModel"></typeparam>
    public class ReadModelByIdQueryExecutor<TReadModel, TCollectionModel> : IQueryExecutor<ReadModelByIdQuery<TReadModel>, TReadModel>
        where TReadModel : class, IReadModel
        where TCollectionModel : class, IReadModel
    {
        private readonly IReadModelStore<TReadModel, TCollectionModel> _readStore;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="readStore"></param>
        public ReadModelByIdQueryExecutor(
            IReadModelStore<TReadModel, TCollectionModel> readStore)
        {
            _readStore = readStore;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="query"></param>
        /// <returns></returns>
        public async Task<TReadModel> ExecuteQueryAsync(ReadModelByIdQuery<TReadModel> query)
        {
            var readModel = await _readStore.GetAsync(query.Id);
            return readModel;
        }
    }
}
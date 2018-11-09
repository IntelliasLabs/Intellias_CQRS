using System.Threading.Tasks;
using Intellias.CQRS.Core.Storage;

namespace Intellias.CQRS.Core.Queries
{
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="TReadStore"></typeparam>
    /// <typeparam name="TReadModel"></typeparam>
    /// <typeparam name="TCollectionModel"></typeparam>
    public class ReadModelByIdQueryExecutor<TReadStore, TReadModel, TCollectionModel> : IQueryExecutor<ReadModelByIdQuery<TReadModel>, TReadModel>
        where TReadStore : IReadModelStore<TReadModel, TCollectionModel>
        where TReadModel : class, IReadModel
        where TCollectionModel : class, IReadModel
    {
        private readonly TReadStore _readStore;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="readStore"></param>
        public ReadModelByIdQueryExecutor(
            TReadStore readStore)
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
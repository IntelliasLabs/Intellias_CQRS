using System.Threading.Tasks;
using Intellias.CQRS.Core.Storage;

namespace Intellias.CQRS.Core.Queries
{
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="TReadModel"></typeparam>
    public class ReadModelByIdQueryExecutor<TReadModel> : IQueryExecutor<ReadModelByIdQuery<TReadModel>, TReadModel>
        where TReadModel : class, IReadModel
    {
        private readonly IReadModelStore<TReadModel> _readStore;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="readStore"></param>
        public ReadModelByIdQueryExecutor(
            IReadModelStore<TReadModel> readStore)
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
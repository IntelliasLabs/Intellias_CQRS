using System.Threading.Tasks;
using Intellias.CQRS.Core.Storage;

namespace Intellias.CQRS.Core.Queries
{
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="TQueryModel"></typeparam>
    public class ReadModelByIdQueryExecutor<TQueryModel> : IQueryExecutor<ReadModelByIdQuery<TQueryModel>, TQueryModel>
        where TQueryModel : class, IQueryModel
    {
        private readonly IQueryModelStore<TQueryModel> _readStore;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="readStore"></param>
        public ReadModelByIdQueryExecutor(
            IQueryModelStore<TQueryModel> readStore)
        {
            _readStore = readStore;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="query"></param>
        /// <returns></returns>
        public async Task<TQueryModel> ExecuteQueryAsync(ReadModelByIdQuery<TQueryModel> query)
        {
            var readModel = await _readStore.GetAsync(query.Id);
            return readModel;
        }
    }
}
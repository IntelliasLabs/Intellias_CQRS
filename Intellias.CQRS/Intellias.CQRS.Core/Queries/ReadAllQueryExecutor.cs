using System.Threading.Tasks;
using Intellias.CQRS.Core.Storage;

namespace Intellias.CQRS.Core.Queries
{
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="TQueryModel"></typeparam>
    public class ReadAllQueryExecutor<TQueryModel> : IQueryExecutor<ReadAllQuery<TQueryModel>, CollectionQueryModel<TQueryModel>>
        where TQueryModel : class, IQueryModel
    {
        private readonly IQueryModelStore<TQueryModel> _readStore;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="readStore"></param>
        public ReadAllQueryExecutor(
            IQueryModelStore<TQueryModel> readStore)
        {
            _readStore = readStore;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="query"></param>
        /// <returns></returns>
        public async Task<CollectionQueryModel<TQueryModel>> ExecuteQueryAsync(ReadAllQuery<TQueryModel> query)
        {
            var model = await _readStore.GetAllAsync();
            return model;
        }
    }
}
using System.Threading.Tasks;
using Intellias.CQRS.Core.Storage;

namespace Intellias.CQRS.Core.Queries
{
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="TReadModel"></typeparam>
    public class ReadAllQueryExecutor<TReadModel> : IQueryExecutor<ReadAllQuery<TReadModel>, CollectionReadModel<TReadModel>>
        where TReadModel : class, IQueryModel
    {
        private readonly IReadModelStore<TReadModel> _readStore;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="readStore"></param>
        public ReadAllQueryExecutor(
            IReadModelStore<TReadModel> readStore)
        {
            _readStore = readStore;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="query"></param>
        /// <returns></returns>
        public async Task<CollectionReadModel<TReadModel>> ExecuteQueryAsync(ReadAllQuery<TReadModel> query)
        {
            var model = await _readStore.GetAllAsync();
            return model;
        }
    }
}
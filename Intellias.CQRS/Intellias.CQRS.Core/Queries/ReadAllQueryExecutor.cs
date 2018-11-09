using System.Threading.Tasks;
using Intellias.CQRS.Core.Storage;

namespace Intellias.CQRS.Core.Queries
{
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="TReadModel"></typeparam>
    /// <typeparam name="TCollectionReadModel"></typeparam>
    public class ReadAllQueryExecutor<TReadModel, TCollectionReadModel> : IQueryExecutor<ReadAllQuery<TCollectionReadModel>, TCollectionReadModel>
        where TReadModel : class, IReadModel
        where TCollectionReadModel : class, IReadModel
    {
        private readonly IReadModelStore<TReadModel, TCollectionReadModel> _readStore;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="readStore"></param>
        public ReadAllQueryExecutor(
            IReadModelStore<TReadModel, TCollectionReadModel> readStore)
        {
            _readStore = readStore;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="query"></param>
        /// <returns></returns>
        public async Task<TCollectionReadModel> ExecuteQueryAsync(ReadAllQuery<TCollectionReadModel> query)
        {
            var model = await _readStore.GetAllAsync();
            return model;
        }
    }
}
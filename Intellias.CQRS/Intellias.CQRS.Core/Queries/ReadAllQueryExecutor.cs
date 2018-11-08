using System.Collections.Generic;
using System.Threading.Tasks;
using Intellias.CQRS.Core.Storage;

namespace Intellias.CQRS.Core.Queries
{
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="TReadStore"></typeparam>
    /// <typeparam name="TReadModel"></typeparam>
    public class ReadAllQueryExecutor<TReadStore, TReadModel> : IQueryExecutor<ReadAllQuery<TReadModel>, IReadOnlyCollection<TReadModel>>
        where TReadStore : IReadModelStore<TReadModel>
        where TReadModel : class, IReadModel
    {
        private readonly TReadStore _readStore;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="readStore"></param>
        public ReadAllQueryExecutor(
            TReadStore readStore)
        {
            _readStore = readStore;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="query"></param>
        /// <returns></returns>
        public async Task<IReadOnlyCollection<TReadModel>> ExecuteQueryAsync(ReadAllQuery<TReadModel> query)
        {
            var readModelEnvelope = await _readStore.GetAllAsync();
            return readModelEnvelope.Items;
        }
    }
}
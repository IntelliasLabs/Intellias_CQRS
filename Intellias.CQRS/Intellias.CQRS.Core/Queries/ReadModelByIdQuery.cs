using System;
using System.Threading.Tasks;
using Intellias.CQRS.Core.Storage;

namespace Intellias.CQRS.Core.Queries
{
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="TReadModel"></typeparam>
    public class ReadModelByIdQuery<TReadModel> : IQuery<TReadModel>
        where TReadModel : class, IReadModel
    {
        /// <summary>
        /// 
        /// </summary>
        public string Id { get; }

        /// <summary>
        ///
        /// </summary>
        /// <param name="id"></param>
        public ReadModelByIdQuery(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                throw new ArgumentNullException(nameof(id));
            }

            Id = id;
        }
    }

    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="TReadStore"></typeparam>
    /// <typeparam name="TReadModel"></typeparam>
    public class ReadModelByIdQueryHandler<TReadStore, TReadModel> : IQueryHandler<ReadModelByIdQuery<TReadModel>, TReadModel>
        where TReadStore : IReadModelStore<TReadModel>
        where TReadModel : class, IReadModel
    {
        private readonly TReadStore _readStore;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="readStore"></param>
        public ReadModelByIdQueryHandler(
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
            var readModelEnvelope = await _readStore.GetAsync(query.Id).ConfigureAwait(false);
            return readModelEnvelope.ReadModel;
        }
    }
}

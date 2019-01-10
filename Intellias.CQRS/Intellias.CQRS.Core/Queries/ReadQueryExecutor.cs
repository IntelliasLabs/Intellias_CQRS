using System.Threading.Tasks;
using Intellias.CQRS.Core.Messages;
using Intellias.CQRS.Core.Storage;

namespace Intellias.CQRS.Core.Queries
{
    /// <summary>
    /// reads concrete models by type or by Id
    /// </summary>
    /// <typeparam name="TQueryModel"></typeparam>
    public class ReadQueryExecutor<TQueryModel> : IReadQueryExecutor<TQueryModel>
        where TQueryModel : class, IQueryModel
    {
        private readonly IQueryModelStore<TQueryModel> _readStore;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="readStore"></param>
        public ReadQueryExecutor(
            IQueryModelStore<TQueryModel> readStore)
        {
            _readStore = readStore;
        }


        /// <inheritdoc />
        public Task<TQueryModel> GetByIdAsync(string parentId, string id) =>
            _readStore.GetAsync(parentId, id);

        /// <inheritdoc />
        public Task<TQueryModel> GetByIdAsync(string id) =>
            GetByIdAsync(Unified.Dummy, id);


        /// <inheritdoc />
        public Task<CollectionQueryModel<TQueryModel>> GetAllAsync(string parentId) =>
            _readStore.GetAllAsync(parentId);

        /// <inheritdoc />
        public Task<CollectionQueryModel<TQueryModel>> GetAllAsync() => 
            _readStore.GetAllAsync();
    }
}
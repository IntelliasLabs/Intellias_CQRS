using System;
using System.Threading.Tasks;
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


        /// <summary>
        /// Get all TQueryModels async
        /// </summary>
        /// <returns></returns>
        public async Task<CollectionQueryModel<TQueryModel>> GetAllAsync() => await _readStore.GetAllAsync();

        /// <summary>
        /// Get TQueryModel by Id async
        /// </summary>
        /// <param name="id">TQueryModel Id</param>
        /// <returns></returns>
        public Task<TQueryModel> GetByIdAsync(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                throw new ArgumentNullException(nameof(id));
            }

            return GetByIdInternalAsync(id);
        }

        private async Task<TQueryModel> GetByIdInternalAsync(string id) => await _readStore.GetAsync(id);
    }
}
using System.Threading.Tasks;
using Intellias.CQRS.Core.Queries;
using Intellias.CQRS.Core.Storage;
using Intellias.CQRS.EventStore.AzureTable.Repositories;
using Microsoft.WindowsAzure.Storage.Table;

namespace Intellias.CQRS.EventStore.AzureTable
{
    /// <inheritdoc />
    /// <summary>
    /// Azure Table Read Storage
    /// </summary>
    public class AzureTableQueryStore<TQueryModel> : IReadModelStore<TQueryModel>
        where TQueryModel : class, IQueryModel
    {
        private readonly QueryModelRepository<TQueryModel> repository;

        /// <summary>
        /// AzureTableReadStore
        /// </summary>
        /// <param name="cloudTable">Azure Cloud Table c</param>
        public AzureTableQueryStore(CloudTable cloudTable)
        {
            repository = new QueryModelRepository<TQueryModel>(cloudTable);
        }

        /// <summary>
        /// Get TReadModel by Id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task<TQueryModel> GetAsync(string id) => await repository.GetModelAsync(id);

        /// <summary>
        /// Get collection of read models by type
        /// </summary>
        /// <returns></returns>
        public async Task<CollectionReadModel<TQueryModel>> GetAllAsync() => await repository.GetAllModelsAsync();

        /// <summary>
        /// NOT IMPLEMENTED
        /// </summary>
        /// <param name="newQueryModel"></param>
        /// <returns></returns>
        public Task<TQueryModel> UpdateAsync(TQueryModel newQueryModel) => throw new System.NotImplementedException();

        /// <summary>
        /// NOT IMPLENETED
        /// </summary>
        /// <param name="newCollection"></param>
        /// <returns></returns>
        public Task<CollectionReadModel<TQueryModel>> UpdateAllAsync(CollectionReadModel<TQueryModel> newCollection)
            => throw new System.NotImplementedException();

        /// <summary>
        /// NOT IMPLEMENTED
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public Task DeleteAsync(string id) => throw new System.NotImplementedException();

        /// <summary>
        /// NOT IMPLEMENTED
        /// </summary>
        /// <returns></returns>
        public Task DeleteAllAsync() => throw new System.NotImplementedException();
    }
}

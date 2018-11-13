using System.Collections.Generic;
using System.Threading.Tasks;
using Intellias.CQRS.Core.Queries;
using Intellias.CQRS.Core.Storage;
using Intellias.CQRS.QueryStore.AzureTable.Repositories;
using Microsoft.WindowsAzure.Storage;

namespace Intellias.CQRS.QueryStore.AzureTable
{
    /// <inheritdoc />
    /// <summary>
    /// Azure Table Read Storage
    /// </summary>
    public class AzureTableQueryStore<TQueryModel> : IQueryModelStore<TQueryModel>
        where TQueryModel : class, IQueryModel
    {
        private readonly QueryModelRepository<TQueryModel> repository;

        /// <summary>
        /// AzureTableReadStore
        /// </summary>
        /// <param name="storeConnectionString"></param>
        public AzureTableQueryStore(string storeConnectionString)
        {
            var client = CloudStorageAccount
                .Parse(storeConnectionString)
                .CreateCloudTableClient();

            repository = new QueryModelRepository<TQueryModel>(client);
        }

        /// <summary>
        /// Get TQueryModel by Id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task<TQueryModel> GetAsync(string id) => await repository.GetModelAsync(id);

        /// <summary>
        /// Get collection of query models by type
        /// </summary>
        /// <returns></returns>
        public async Task<CollectionQueryModel<TQueryModel>> GetAllAsync() => await repository.GetAllModelsAsync();

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
        public Task<CollectionQueryModel<TQueryModel>> UpdateAllAsync(IEnumerable<TQueryModel> newCollection)
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

        /// <summary>
        /// NOT IMPLEMENTED
        /// </summary>
        /// <param name="newQueryModel"></param>
        /// <returns></returns>
        public Task<TQueryModel> CreateAsync(TQueryModel newQueryModel)
        {
            throw new System.NotImplementedException();
        }

        /// <summary>
        /// NOT IMPLEMENTED
        /// </summary>
        /// <param name="newCollection"></param>
        /// <returns></returns>
        public Task<CollectionQueryModel<TQueryModel>> CreateAllAsync(IEnumerable<TQueryModel> newCollection)
        {
            throw new System.NotImplementedException();
        }
    }
}

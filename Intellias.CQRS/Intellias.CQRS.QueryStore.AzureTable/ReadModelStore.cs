using System.Threading.Tasks;
using Intellias.CQRS.Core.Queries;
using Intellias.CQRS.Core.Storage;
using Intellias.CQRS.EventStore.AzureTable.Repositories;
using Microsoft.WindowsAzure.Storage;

namespace Intellias.CQRS.EventStore.AzureTable
{
    /// <inheritdoc />
    /// <summary>
    /// Azure Table Storage event store
    /// </summary>
    public class AzureTableEventStore<TReadModel> : IReadModelStore<TReadModel>
        where TReadModel : class, IReadModel
    {
        private readonly ReadModelRepository<TReadModel> repository;

        /// <summary>
        /// AzureTableEventStore
        /// </summary>
        /// <param name="storeConnectionString">Azure Table Storage connection string</param>
        /// <param name="eventStoreName">name of the Azure Table Storage in Azure</param>
        public AzureTableEventStore(string storeConnectionString, string eventStoreName)
        {
            var client = CloudStorageAccount
                .Parse(storeConnectionString)
                .CreateCloudTableClient();

            repository = new ReadModelRepository<TReadModel>(client, eventStoreName);
        }

        // WARNING! I'm not sure that we need to imlement such methods
        /// <summary>
        /// NOT IMPLEMENTED
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public Task DeleteAsync(string id)
        {
            throw new System.NotImplementedException();
        }

        /// <summary>
        /// NOT IMPLEMENTED
        /// </summary>
        /// <returns></returns>
        public Task DeleteAllAsync()
        {
            throw new System.NotImplementedException();
        }

        /// <summary>
        /// Get TReadModel by Id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task<TReadModel> GetAsync(string id) => await repository.GetModelAsync(id);

        /// <summary>
        /// Get collection of read models by type
        /// </summary>
        /// <returns></returns>
        public async Task<CollectionReadModel<TReadModel>> GetAllAsync() => await repository.GetAllModelsAsync();
    }
}

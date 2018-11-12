using System.Linq;
using System.Threading.Tasks;
using Intellias.CQRS.Core.Queries;
using Intellias.CQRS.EventStore.AzureTable.Documents;
using Microsoft.WindowsAzure.Storage.Table;
using Newtonsoft.Json;

namespace Intellias.CQRS.EventStore.AzureTable.Repositories
{
    /// <summary>
    /// ReadModelRepository
    /// </summary>
    public class ReadModelRepository<TReadModel> where TReadModel : class, IReadModel
    {
        private readonly CloudTable readModelTable;

        /// <summary>
        /// ReadModelRepository init
        /// </summary>
        /// <param name="client">CloudTableClient to Azure</param>
        /// <param name="tableName">name of the table of Read Models</param>
        public ReadModelRepository(CloudTableClient client, string tableName)
        {
            readModelTable = client.GetTableReference(tableName);

            // Create the CloudTable if it does not exist
            readModelTable.CreateIfNotExistsAsync().Wait();
        }

        /// <summary>
        /// GetModel By Id
        /// </summary>
        /// <param name="readModelId">Id of read model</param>
        /// <returns></returns>
        public async Task<TReadModel> GetModelAsync(string readModelId)
        {
            var query = new TableQuery<ReadModelTableEntity>()
                .Where(TableQuery.GenerateFilterCondition("RowKey",
                    QueryComparisons.Equal, readModelId));

            var queryResult = await readModelTable.ExecuteQuerySegmentedAsync(query, null);
            var tableEntity = queryResult.Results.Single();

            return (TReadModel)JsonConvert.DeserializeObject(tableEntity.Data);
            
        }

        /// <summary>
        /// Get CollectionReadModel by Read Model type
        /// </summary>
        /// <returns></returns>
        public async Task<CollectionReadModel<TReadModel>> GetAllModelsAsync()
        {
            var query = new TableQuery<ReadModelTableEntity>()
                .Where(TableQuery.GenerateFilterCondition("PartitionKey",
                    QueryComparisons.Equal, typeof(TReadModel).ToString()));

            var queryResult = await readModelTable.ExecuteQuerySegmentedAsync(query, null);

            var list = queryResult.Results.Select(item => (TReadModel)JsonConvert.DeserializeObject(item.Data)).ToList();

            return new CollectionReadModel<TReadModel>
            {
                Items = list,
                Total = list.Count
            };
        }
    }
}

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
    public class QueryModelRepository<TQueryModel> where TQueryModel : class, IQueryModel
    {
        private readonly CloudTable queryModelTable;

        /// <summary>
        /// ReadModelRepository init
        /// </summary>
        /// <param name="table">CloudTable</param>
        public QueryModelRepository(CloudTable table)
        {
            queryModelTable = table;

            // Create the CloudTable if it does not exist
            queryModelTable.CreateIfNotExistsAsync().Wait();
        }

        /// <summary>
        /// GetModel By Id
        /// </summary>
        /// <param name="readModelId">Id of read model</param>
        /// <returns></returns>
        public async Task<TQueryModel> GetModelAsync(string readModelId)
        {
            var query = new TableQuery<QueryModelTableEntity>()
                .Where(TableQuery.GenerateFilterCondition("RowKey",
                    QueryComparisons.Equal, readModelId));

            var queryResult = await queryModelTable.ExecuteQuerySegmentedAsync(query, null);
            var tableEntity = queryResult.Results.Single();

            return (TQueryModel)JsonConvert.DeserializeObject(tableEntity.Data);
            
        }

        /// <summary>
        /// Get CollectionReadModel by Read Model type
        /// </summary>
        /// <returns></returns>
        public async Task<CollectionQueryModel<TQueryModel>> GetAllModelsAsync()
        {
            var query = new TableQuery<QueryModelTableEntity>()
                .Where(TableQuery.GenerateFilterCondition("PartitionKey",
                    QueryComparisons.Equal, typeof(TQueryModel).ToString()));

            var queryResult = await queryModelTable.ExecuteQuerySegmentedAsync(query, null);

            var list = queryResult.Results.Select(item => (TQueryModel)JsonConvert.DeserializeObject(item.Data)).ToList();

            return new CollectionQueryModel<TQueryModel>
            {
                Items = list,
                Total = list.Count
            };
        }
    }
}

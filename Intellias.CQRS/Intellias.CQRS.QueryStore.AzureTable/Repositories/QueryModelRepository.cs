using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Intellias.CQRS.Core.Config;
using Intellias.CQRS.Core.Queries;
using Intellias.CQRS.QueryStore.AzureTable.Documents;
using Intellias.CQRS.QueryStore.AzureTable.Extensions;
using Microsoft.WindowsAzure.Storage.Table;
using Newtonsoft.Json;

namespace Intellias.CQRS.QueryStore.AzureTable.Repositories
{
    /// <summary>
    /// ReadModelRepository
    /// </summary>
    public class QueryModelRepository<TQueryModel> where TQueryModel : class, IQueryModel
    {
        private readonly CloudTable queryTable;

        /// <summary>
        /// ReadModelRepository init
        /// </summary>
        /// <param name="client"></param>
        public QueryModelRepository(CloudTableClient client)
        {
            queryTable = client.GetTableReference(typeof(TQueryModel).Name);

            // Create the CloudTable if it does not exist
            queryTable.CreateIfNotExistsAsync().Wait();
        }

        /// <summary>
        /// GetModel By Id
        /// </summary>
        /// <param name="parentId"></param>
        /// <param name="id">Id of read model</param>
        /// <returns></returns>
        public async Task<TQueryModel> GetModelAsync(string parentId, string id)
        {
            var operation = 
                TableOperation.Retrieve<QueryModelTableEntity>(parentId, id);

            var queryResult = 
                await queryTable.ExecuteAsync(operation);

            var modelJson = ((QueryModelTableEntity)queryResult.Result).Data;
            var model = JsonConvert.DeserializeObject<TQueryModel>(modelJson, CqrsSettings.JsonConfig());

            return model ?? throw new KeyNotFoundException();
        }

        /// <summary>
        /// Get CollectionReadModel by Read Model type
        /// </summary>
        /// <param name="parentId"></param>
        /// <returns></returns>
        public async Task<CollectionQueryModel<TQueryModel>> GetAllModelsAsync(string parentId)
        {
            var query = new TableQuery<QueryModelTableEntity>()
                .Where(TableQuery.GenerateFilterCondition("PartitionKey",
                    QueryComparisons.Equal, parentId));

            var results = new List<QueryModelTableEntity>();
            TableContinuationToken continuationToken = null;

            do
            {
                var queryResults =
                    await queryTable.ExecuteQuerySegmentedAsync(query, continuationToken);

                continuationToken = queryResults.ContinuationToken;
                results.AddRange(queryResults.Results);

            } while (continuationToken != null);

            var list = results
                .Select(item => JsonConvert.DeserializeObject<TQueryModel>(item.Data, CqrsSettings.JsonConfig()))
                .ToList();

            return new CollectionQueryModel<TQueryModel>
            {
                Items = list,
                Total = list.Count
            };
        }

        /// <summary>
        /// Insert query model to table
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public async Task<TQueryModel> InsertModelAsync(TQueryModel model)
        {
            var operation = TableOperation.Insert(model.ToStoreEntity());
            await queryTable.ExecuteAsync(operation);
            return model;
        }

        /// <summary>
        /// UpdateModelAsync
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public async Task<TQueryModel> UpdateModelAsync(TQueryModel model)
        {
            // Getting entity
            var record = await RetrieveRecord(model.ParentId, model.Id);

            record.Data = JsonConvert.SerializeObject(model, CqrsSettings.JsonConfig());
            record.Timestamp = DateTime.UtcNow;

            var updateOperation = TableOperation.Replace(record);
            await queryTable.ExecuteAsync(updateOperation);
            return model;
        }


        /// <summary>
        /// DeleteModel
        /// </summary>
        /// <param name="parentId"></param>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task DeleteModelAsync(string parentId, string id)
        {
            // Getting entity
            var record = await RetrieveRecord(parentId, id);

            // Removing
            var deleteOperation = TableOperation.Delete(record);
            await queryTable.ExecuteAsync(deleteOperation);
        }


        private async Task<QueryModelTableEntity> RetrieveRecord(string parentId, string id)
        {
            var readOperation =
                TableOperation.Retrieve<QueryModelTableEntity>(parentId, id);
            var queryResult = await queryTable.ExecuteAsync(readOperation);

            var entity = (QueryModelTableEntity)queryResult.Result;
            if (entity == null) { throw new KeyNotFoundException(); }

            return entity;
        }
    }
}

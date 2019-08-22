using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using Intellias.CQRS.Core.Queries.Immutable;
using Intellias.CQRS.QueryStore.AzureTable.Common;
using Intellias.CQRS.QueryStore.AzureTable.Options;
using Microsoft.Extensions.Options;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Table;
using Newtonsoft.Json;

namespace Intellias.CQRS.QueryStore.AzureTable.Immutable
{
    /// <summary>
    /// Azure Storage Account Table storage for <see cref="IImmutableQueryModel"/>.
    /// </summary>
    /// <typeparam name="TQueryModel">Type of the query model.</typeparam>
    public class ImmutableQueryModelTableStorage<TQueryModel> :
        IImmutableQueryModelReader<TQueryModel>,
        IImmutableQueryModelWriter<TQueryModel>
        where TQueryModel : class, IImmutableQueryModel, new()
    {
        private readonly CloudTableProxy tableProxy;

        public ImmutableQueryModelTableStorage(IOptionsMonitor<TableStorageOptions> options)
        {
            var client = CloudStorageAccount
                .Parse(options.CurrentValue.ConnectionString)
                .CreateCloudTableClient();

            var tableName = options.CurrentValue.TableNamePrefix + typeof(TQueryModel).Name;
            var tableReference = client.GetTableReference(tableName);

            tableProxy = new CloudTableProxy(tableReference);
        }

        /// <inheritdoc />
        public async Task<TQueryModel?> FindAsync(string id, int version)
        {
            var operation = TableOperation.Retrieve<ImmutableTableEntity>(id, GetRowKey(version));
            var result = await tableProxy.ExecuteAsync(operation);
            var entity = (ImmutableTableEntity)result.Result;

            return entity?.DeserializeQueryModel();
        }

        /// <inheritdoc />
        public async Task<TQueryModel> GetAsync(string id, int version)
        {
            var queryModel = await FindAsync(id, version);
            if (queryModel == null)
            {
                throw new KeyNotFoundException($"No query model '{typeof(TQueryModel)}' with id '{id}' and version '{version}' is found.");
            }

            return queryModel;
        }

        /// <inheritdoc />
        public async Task<TQueryModel> GetLatestAsync(string id)
        {
            var filter = TableQuery.GenerateFilterCondition(nameof(TableEntity.PartitionKey), QueryComparisons.Equal, id);
            var query = new TableQuery<ImmutableTableEntity>()
                .Where(filter)
                .Take(1);

            var querySegment = await tableProxy.ExecuteQuerySegmentedAsync(query, null);
            var queryModel = querySegment.Results.Select(entity => entity.DeserializeQueryModel()).FirstOrDefault();

            return queryModel;
        }

        /// <inheritdoc />
        public async Task<TQueryModel> CreateAsync(TQueryModel model)
        {
            var entity = new ImmutableTableEntity(model);
            var operation = TableOperation.Insert(entity);

            var result = await tableProxy.ExecuteAsync(operation);

            return ((ImmutableTableEntity)result.Result).DeserializeQueryModel();
        }

        private static string GetRowKey(int version)
        {
            // Build from Created row key that stores data in reverse order.
            return string.Format(CultureInfo.InvariantCulture, "{0:D20}", int.MaxValue - version);
        }

        private class ImmutableTableEntity : TableEntity
        {
            public ImmutableTableEntity()
            {
                JsonQueryModel = string.Empty;
            }

            public ImmutableTableEntity(TQueryModel queryModel)
            {
                PartitionKey = queryModel.Id;
                RowKey = GetRowKey(queryModel.Version);
                JsonQueryModel = JsonConvert.SerializeObject(queryModel);
            }

            public string JsonQueryModel { get; set; }

            public TQueryModel DeserializeQueryModel()
            {
                if (string.IsNullOrWhiteSpace(JsonQueryModel))
                {
                    throw new InvalidOperationException($"Unable to deserialize entity '{RowKey}' from empty json.");
                }

                var queryModel = JsonConvert.DeserializeObject<TQueryModel>(JsonQueryModel);

                queryModel.Timestamp = Timestamp;

                return queryModel;
            }
        }
    }
}
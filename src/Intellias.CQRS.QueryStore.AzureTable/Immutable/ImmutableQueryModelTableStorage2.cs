using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using Intellias.CQRS.Core.Queries.Immutable;
using Intellias.CQRS.Persistence.AzureStorage.Common;
using Microsoft.Azure.Cosmos.Table;

namespace Intellias.CQRS.QueryStore.AzureTable.Immutable
{
    /// <summary>
    /// Azure Storage Account Table storage for <see cref="IImmutableQueryModel"/>.
    /// </summary>
    /// <typeparam name="TQueryModel">Type of the query model.</typeparam>
    public class ImmutableQueryModelTableStorage2<TQueryModel> : BaseTableStorage2,
        IImmutableQueryModelReader<TQueryModel>,
        IImmutableQueryModelWriter<TQueryModel>
        where TQueryModel : class, IImmutableQueryModel, new()
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ImmutableQueryModelTableStorage2{TQueryModel}"/> class.
        /// </summary>
        /// <param name="options">Table storage options.</param>
        public ImmutableQueryModelTableStorage2(ITableStorageOptions options)
            : base(options, typeof(TQueryModel).Name)
        {
        }

        /// <inheritdoc />
        public async Task<TQueryModel> FindAsync(string id, int version)
        {
            var result = await FindAsync(id, GetRowKey(version));
            return result == null ? null : Deserialize(result);
        }

        /// <inheritdoc />
        public async Task<TQueryModel> FindLatestAsync(string id)
        {
            var result = (await QueryFirstAsync(id, 1)).FirstOrDefault();
            return result == null ? null : Deserialize(result);
        }

        /// <inheritdoc />
        public async Task<TQueryModel> FindEqualOrLessAsync(string id, int fromVersion)
        {
            var filterByPartition = TableQuery.GenerateFilterCondition(nameof(TableEntity.PartitionKey), QueryComparisons.Equal, id);
            var filterByRowKey = TableQuery.GenerateFilterCondition(nameof(TableEntity.RowKey), QueryComparisons.GreaterThanOrEqual, GetRowKey(fromVersion));
            var filter = TableQuery.CombineFilters(filterByPartition, TableOperators.And, filterByRowKey);

            var query = new TableQuery<DynamicTableEntity>()
                .Where(filter)
                .Take(1);

            var result = (await TableProxy.ExecuteQuerySegmentedAsync(query, null)).FirstOrDefault();
            return result == null ? null : Deserialize(result);
        }

        /// <inheritdoc />
        public async Task<TQueryModel> GetAsync(string id, int version)
        {
            var result = await FindAsync(id, version);
            return result ?? throw new KeyNotFoundException($"No query model '{typeof(TQueryModel)}' with id '{id}' and version '{version}' is found.");
        }

        /// <inheritdoc />
        public async Task<TQueryModel> GetLatestAsync(string id)
        {
            var result = await FindLatestAsync(id);
            return result ?? throw new KeyNotFoundException($"No query model '{typeof(TQueryModel)}' with id '{id}' is found.");
        }

        /// <inheritdoc />
        public async Task<TQueryModel> GetEqualOrLessAsync(string id, int fromVersion)
        {
            var queryModel = await FindEqualOrLessAsync(id, fromVersion);
            return queryModel ?? throw new KeyNotFoundException($"No query model '{typeof(TQueryModel)}' with id '{id}' and version the same or smaller than '{fromVersion}' is found.");
        }

        /// <inheritdoc />
        public async Task<TQueryModel> CreateAsync(TQueryModel model)
        {
            var entity = Serialize(model);
            var result = await InsertAsync(entity);

            return Deserialize(result);
        }

        private static string GetRowKey(int version)
        {
            // Build from Created row key that stores data in reverse order.
            return string.Format(CultureInfo.InvariantCulture, "{0:D20}", int.MaxValue - version);
        }

        private static TQueryModel Deserialize(DynamicTableEntity entity)
        {
            var queryModel = AzureTableSerializer.Deserialize<TQueryModel>(entity);
            queryModel.Timestamp = entity.Timestamp;
            return queryModel;
        }

        private DynamicTableEntity Serialize(TQueryModel queryModel)
        {
            return new DynamicTableEntity(queryModel.Id, GetRowKey(queryModel.Version))
            {
                Timestamp = queryModel.Timestamp,
                Properties = AzureTableSerializer.Serialize(queryModel)
            };
        }
    }
}
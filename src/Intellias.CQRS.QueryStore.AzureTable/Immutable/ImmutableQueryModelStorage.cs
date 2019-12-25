using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using Intellias.CQRS.Core.Queries.Immutable;
using Intellias.CQRS.QueryStore.AzureTable.Common;
using Intellias.CQRS.QueryStore.AzureTable.Options;
using Microsoft.Extensions.Options;
using Microsoft.WindowsAzure.Storage.Table;

namespace Intellias.CQRS.QueryStore.AzureTable.Immutable
{
    /// <summary>
    /// Azure Storage Account Table storage for <see cref="IImmutableQueryModel"/>.
    /// </summary>
    /// <typeparam name="TQueryModel">Type of the query model.</typeparam>
    public class ImmutableQueryModelStorage<TQueryModel> :
        BaseTableStorage<ImmutableTableEntity<TQueryModel>>,
        IImmutableQueryModelReader<TQueryModel>,
        IImmutableQueryModelWriter<TQueryModel>
        where TQueryModel : class, IImmutableQueryModel, new()
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ImmutableQueryModelStorage{TQueryModel}"/> class.
        /// </summary>
        /// <param name="options">Table storage options.</param>
        public ImmutableQueryModelStorage(IOptionsMonitor<TableStorageOptions> options)
            : base(options, typeof(TQueryModel).Name)
        {
        }

        /// <inheritdoc />
        public async Task<TQueryModel> FindAsync(string id, int version)
        {
            var result = await FindAsync(id, GetRowKey(version));
            return result?.DeserializeData();
        }

        /// <inheritdoc />
        public async Task<TQueryModel> FindLatestAsync(string id)
        {
            var entity = (await QueryFirstAsync(id, 1)).FirstOrDefault();
            return entity?.DeserializeData();
        }

        /// <inheritdoc />
        public async Task<TQueryModel> FindEqualOrLessAsync(string id, int fromVersion)
        {
            var filterByPartition = TableQuery.GenerateFilterCondition(nameof(TableEntity.PartitionKey), QueryComparisons.Equal, id);
            var filterByRowKey = TableQuery.GenerateFilterCondition(nameof(TableEntity.RowKey), QueryComparisons.GreaterThanOrEqual, GetRowKey(fromVersion));
            var filter = TableQuery.CombineFilters(filterByPartition, TableOperators.And, filterByRowKey);

            var query = new TableQuery<ImmutableTableEntity<TQueryModel>>()
                .Where(filter)
                .Take(1);

            var result = await TableProxy.ExecuteQuerySegmentedAsync(query, null);
            return result.FirstOrDefault()?.DeserializeData();
        }

        /// <inheritdoc />
        public async Task<TQueryModel> GetAsync(string id, int version)
        {
            var result = await FindAsync(id, version);
            if (result == null)
            {
                throw new KeyNotFoundException($"No query model '{typeof(TQueryModel)}' with id '{id}' and version '{version}' is found.");
            }

            return result;
        }

        /// <inheritdoc />
        public async Task<TQueryModel> GetLatestAsync(string id)
        {
            var result = await FindLatestAsync(id);
            return result ?? throw new KeyNotFoundException($"No query model '{typeof(TQueryModel)}' with id '{id}' is found.");
        }

        /// <inheritdoc />
        public async Task<TQueryModel> CreateAsync(TQueryModel model)
        {
            var entity = new ImmutableTableEntity<TQueryModel>(model);
            var result = await InsertAsync(entity);

            return result.DeserializeData();
        }

        /// <inheritdoc />
        public async Task<TQueryModel> GetEqualOrLessAsync(string id, int fromVersion)
        {
            var queryModel = await FindEqualOrLessAsync(id, fromVersion);
            return queryModel ?? throw new KeyNotFoundException($"No query model '{typeof(TQueryModel)}' with id '{id}' and version the same or smaller than '{fromVersion}' is found.");
        }

        private static string GetRowKey(int version)
        {
            // Build from Created row key that stores data in reverse order.
            return string.Format(CultureInfo.InvariantCulture, "{0:D20}", int.MaxValue - version);
        }
    }
}
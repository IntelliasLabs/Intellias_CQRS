using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Intellias.CQRS.Core.Queries.Mutable;
using Intellias.CQRS.Persistence.AzureStorage.Common;
using Microsoft.Azure.Cosmos.Table;

namespace Intellias.CQRS.QueryStore.AzureTable.Mutable
{
    /// <summary>
    /// Azure Storage Account Table storage for <see cref="IMutableQueryModel"/>.
    /// </summary>
    /// <typeparam name="TQueryModel">Type of the query model.</typeparam>
    public class MutableQueryModelTableStorage2<TQueryModel> : BaseTableStorage2,
        IMutableQueryModelReader<TQueryModel>,
        IMutableQueryModelWriter<TQueryModel>
        where TQueryModel : class, IMutableQueryModel, new()
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="MutableQueryModelTableStorage2{TQueryModel}"/> class.
        /// </summary>
        /// <param name="options">Table storage options.</param>
        public MutableQueryModelTableStorage2(ITableStorageOptions options)
            : base(options, typeof(TQueryModel).Name)
        {
        }

        /// <inheritdoc />
        public async Task<TQueryModel> FindAsync(string id)
        {
            var result = await FindAsync(Options.GetPartitionKey(id), id);
            return result == null ? null : Deserialize(result);
        }

        /// <inheritdoc />
        public async Task<TQueryModel> GetAsync(string id)
        {
            var queryModel = await FindAsync(id);
            if (queryModel == null)
            {
                throw new KeyNotFoundException($"No query model '{typeof(TQueryModel)}' with id '{id}' is found.");
            }

            return queryModel;
        }

        /// <inheritdoc />
        public async Task<IReadOnlyCollection<TQueryModel>> GetAllAsync()
        {
            var results = await QueryAllSegmentedAsync(new TableQuery<DynamicTableEntity>());
            return results.Select(Deserialize).ToArray();
        }

        /// <inheritdoc />
        public async Task<IReadOnlyCollection<TQueryModel>> GetAllAsync(IReadOnlyCollection<string> ids)
        {
            var keys = ids
                .Select(id => (PartitionKey: Options.GetPartitionKey(id), RowKey: id))
                .ToArray();

            var queries = CreateQueryByKeys<DynamicTableEntity>(keys);
            var queryResults = await QueryAllSegmentedAsync(queries);

            return queryResults.Select(Deserialize).ToArray();
        }

        /// <inheritdoc />
        public async Task<TQueryModel> CreateAsync(TQueryModel model)
        {
            var entity = Serialize(model);
            var result = await InsertAsync(entity);

            return Deserialize(result);
        }

        /// <inheritdoc />
        public async Task<TQueryModel> ReplaceAsync(TQueryModel model)
        {
            var entity = Serialize(model);
            var result = await ReplaceAsync(entity);

            return Deserialize(result);
        }

        /// <inheritdoc />
        public Task DeleteAsync(string id)
        {
            return DeleteAsync(Options.GetPartitionKey(id), id);
        }

        private static TQueryModel Deserialize(DynamicTableEntity entity)
        {
            var queryModel = AzureTableSerializer.Deserialize<TQueryModel>(entity);
            queryModel.ETag = entity.ETag;
            queryModel.Timestamp = entity.Timestamp;
            return queryModel;
        }

        private DynamicTableEntity Serialize(TQueryModel queryModel, string etag = null)
        {
            return new DynamicTableEntity(Options.GetPartitionKey(queryModel.Id), queryModel.Id)
            {
                ETag = etag ?? queryModel.ETag,
                Timestamp = queryModel.Timestamp,
                Properties = AzureTableSerializer.Serialize(queryModel)
            };
        }
    }
}
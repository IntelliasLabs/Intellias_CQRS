using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Intellias.CQRS.Core.Queries.Mutable;
using Intellias.CQRS.QueryStore.AzureTable.Common;
using Intellias.CQRS.QueryStore.AzureTable.Options;
using Microsoft.Azure.Cosmos.Table;
using Microsoft.Extensions.Options;

namespace Intellias.CQRS.QueryStore.AzureTable.Mutable
{
    /// <summary>
    /// Azure Storage Account Table storage for <see cref="IMutableQueryModel"/>.
    /// </summary>
    /// <typeparam name="TQueryModel">Type of the query model.</typeparam>
    public class MutableQueryModelTableStorage<TQueryModel> :
        BaseTableStorage<MutableTableEntity<TQueryModel>>,
        IMutableQueryModelReader<TQueryModel>,
        IMutableQueryModelWriter<TQueryModel>
        where TQueryModel : class, IMutableQueryModel, new()
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="MutableQueryModelTableStorage{TQueryModel}"/> class.
        /// </summary>
        /// <param name="options">Table storage options.</param>
        public MutableQueryModelTableStorage(IOptionsMonitor<TableStorageOptions> options)
            : base(options, typeof(TQueryModel).Name)
        {
        }

        /// <inheritdoc />
        public async Task<TQueryModel> FindAsync(string id)
        {
            var result = await FindAsync(typeof(TQueryModel).Name, id);
            return result?.DeserializeData();
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
            var results = await QueryAllSegmentedAsync(new TableQuery<MutableTableEntity<TQueryModel>>());
            return results.Select(r => r.DeserializeData()).ToArray();
        }

        /// <inheritdoc />
        public async Task<IReadOnlyCollection<TQueryModel>> GetAllAsync(IReadOnlyCollection<string> ids)
        {
            var results = await QuerySegmentedAsync(typeof(TQueryModel).Name, ids);
            return results.Select(r => r.DeserializeData()).ToArray();
        }

        /// <inheritdoc />
        public async Task<TQueryModel> CreateAsync(TQueryModel model)
        {
            var entity = new MutableTableEntity<TQueryModel>(model);
            var result = await InsertAsync(entity);

            return result.DeserializeData();
        }

        /// <inheritdoc />
        public async Task<TQueryModel> ReplaceAsync(TQueryModel model)
        {
            var entity = new MutableTableEntity<TQueryModel>(model);
            var result = await ReplaceAsync(entity);

            return result.DeserializeData();
        }

        /// <inheritdoc />
        public Task DeleteAsync(string id)
        {
            return DeleteAsync(typeof(TQueryModel).Name, id);
        }
    }
}
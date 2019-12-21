using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using Intellias.CQRS.Core.Queries.Immutable;
using Intellias.CQRS.QueryStore.AzureTable.Common;
using Intellias.CQRS.QueryStore.AzureTable.Options;
using Microsoft.Extensions.Options;

namespace Intellias.CQRS.QueryStore.AzureTable.Immutable
{
    /// <summary>
    /// Azure Storage Account Table storage for <see cref="IImmutableQueryModel"/>.
    /// </summary>
    /// <typeparam name="TQueryModel">Type of the query model.</typeparam>
    public class ImmutableQueryModelTableStorage<TQueryModel> :
        BaseTableStorage<ImmutableTableEntity<TQueryModel>>,
        Core.Queries.Immutable.Interfaces.IImmutableQueryModelReader<TQueryModel>,
        Core.Queries.Immutable.Interfaces.IImmutableQueryModelWriter<TQueryModel>
        where TQueryModel : class, IImmutableQueryModel, new()
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ImmutableQueryModelTableStorage{TQueryModel}"/> class.
        /// </summary>
        /// <param name="options">Table storage options.</param>
        public ImmutableQueryModelTableStorage(IOptionsMonitor<TableStorageOptions> options)
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
            var queryModel = await FindLatestAsync(id);
            return queryModel ?? throw new KeyNotFoundException($"No query model '{typeof(TQueryModel)}' with id '{id}' is found.");
        }

        /// <inheritdoc />
        public async Task<TQueryModel> CreateAsync(TQueryModel model)
        {
            var entity = new ImmutableTableEntity<TQueryModel>(model);
            var result = await InsertAsync(entity);

            return result.DeserializeData();
        }

        private static string GetRowKey(int version)
        {
            // Build from Created row key that stores data in reverse order.
            return string.Format(CultureInfo.InvariantCulture, "{0:D20}", int.MaxValue - version);
        }
    }
}
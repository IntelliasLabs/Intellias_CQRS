using Intellias.CQRS.Core.Messages;
using Intellias.CQRS.Core.Storage;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Auth;
using Microsoft.WindowsAzure.Storage.Table;
using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace Intellias.CQRS.Storage.Azure
{
    /// <inheritdoc />
    public class TableStorage<T> : IStorage<T> where T : AbstractMessage, new()
    {
        private readonly CloudTable table;

        /// <inheritdoc />
        public TableStorage(StorageCredentials creds)
        {
            var account = new CloudStorageAccount(creds, true);
            var client = account.CreateCloudTableClient();
            table = client.GetTableReference(typeof(T).Name);

            // Create the CloudTable if it does not exist
            table.CreateIfNotExistsAsync().Wait();
        }

        private T GetResult(TableResult response)
        {
            if (response == null || response.Result == null)
            {
                return null;
            }
            return ((StorageEntity) response.Result).GetValue<T>();
        }

        /// <inheritdoc />
        public async Task<T> CreateAsync(T entity)
        {
            if (entity == null)
            {
                throw new ArgumentNullException(nameof(entity));
            }

            if (entity.Id == null)
            {
                throw new InvalidConstraintException(nameof(entity.Id));
            }

            var op = TableOperation.Insert(new StorageEntity(entity));

            // Execute the insert operation.
            var response = await table.ExecuteAsync(op).ConfigureAwait(false);

            return GetResult(response);
        }

        /// <inheritdoc />
        public async Task<T> DeleteAsync(string id)
        {
            if (string.IsNullOrWhiteSpace(id))
            {
                throw new ArgumentNullException(nameof(id));
            }

            var entity = await OneAsync(id.ToUpperInvariant()).ConfigureAwait(false);

            var op = TableOperation.Delete(new StorageEntity(entity));

            var response = await table.ExecuteAsync(op).ConfigureAwait(false);

            return GetResult(response);
        }

        /// <inheritdoc />
        public void Dispose()
        {

        }

        /// <inheritdoc />
        public async Task<T> OneAsync(string id)
        {
            if (string.IsNullOrWhiteSpace(id))
            {
                throw new ArgumentNullException(nameof(id));
            }

            var op = TableOperation.Retrieve<StorageEntity>(id.ToUpperInvariant().First().ToString(CultureInfo.InvariantCulture), id.ToUpperInvariant());

            // Execute the retrieve operation.
            var response = await table.ExecuteAsync(op).ConfigureAwait(false);

            return GetResult(response);
        }

        /// <inheritdoc />
        public async Task<IReadOnlyCollection<T>> QueryAsync(Expression<Func<T, bool>> predicate = null)
        {
            // Construct the query operation for all customer entities where PartitionKey="Smith".
            var query = new TableQuery<StorageEntity>();

            TableContinuationToken token = null;
            var items = new List<T>();

            do
            {
                var segment = await table.ExecuteQuerySegmentedAsync(query, token).ConfigureAwait(false);
                token = segment.ContinuationToken;

                items.AddRange(segment.Results.Select(x => x.GetValue<T>()));
            } while (token != null);

            return items;
        }

        /// <inheritdoc />
        public async Task<T> UpdateAsync(T entity)
        {
            if (entity == null)
            {
                throw new ArgumentNullException(nameof(entity));
            }

            if (string.IsNullOrWhiteSpace(entity.Id))
            {
                throw new InvalidConstraintException(nameof(entity.Id));
            }

            // Execute the insert operation.
            var response = await table.ExecuteAsync(TableOperation.Replace(new StorageEntity(entity))).ConfigureAwait(false);

            return GetResult(response);
        }
    }
}

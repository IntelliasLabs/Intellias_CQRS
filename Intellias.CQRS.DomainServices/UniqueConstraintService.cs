using System;
using System.Diagnostics.CodeAnalysis;
using System.Text;
using System.Threading.Tasks;
using Intellias.CQRS.Core.Messages;
using Intellias.CQRS.Core.Results;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Shared.Protocol;
using Microsoft.WindowsAzure.Storage.Table;
using Microsoft.WindowsAzure.Storage.Table.Protocol;

namespace Intellias.CQRS.DomainServices
{
    /// <summary>
    /// Unique-constraint service.
    /// </summary>
    [ExcludeFromCodeCoverage]
    public class UniqueConstraintService : IUniqueConstraintService
    {
        private readonly CloudTable table;

        /// <summary>
        /// Initializes a new instance of the <see cref="UniqueConstraintService"/> class.
        /// </summary>
        /// <param name="account">Storage account.</param>
        public UniqueConstraintService(CloudStorageAccount account)
        {
            var client = account.CreateCloudTableClient();
            table = client.GetTableReference(typeof(UniqueConstraintService).Name);

            if (!table.ExistsAsync().GetAwaiter().GetResult())
            {
                table.CreateIfNotExistsAsync().Wait();
            }
        }

        /// <inheritdoc />
        public async Task<IExecutionResult> RemoveConstraintAsync(string indexName, string value)
        {
            try
            {
                await table.ExecuteAsync(TableOperation.Delete(new UniqueTableEntity(indexName, value)));
            }
            catch (StorageException e)
            {
                var errorCode = e.RequestInformation.ExtendedErrorInformation.ErrorCode;
                if (StorageErrorCodeStrings.ResourceNotFound.Equals(errorCode, StringComparison.InvariantCultureIgnoreCase))
                {
                    return new FailedResult($"The name '{value}' is not in use. Please enter another one.");
                }

                return new FailedResult("Delete operation failed.");
            }

            return new SuccessfulResult();
        }

        /// <inheritdoc />
        public async Task<IExecutionResult> ReserveConstraintAsync(string indexName, string value)
        {
            try
            {
                await table.ExecuteAsync(TableOperation.Insert(new UniqueTableEntity(indexName, value)));
            }
            catch (StorageException e)
            {
                var errorCode = e.RequestInformation.ExtendedErrorInformation.ErrorCode;
                if (TableErrorCodeStrings.EntityAlreadyExists.Equals(errorCode, StringComparison.InvariantCultureIgnoreCase))
                {
                    return new FailedResult($"The name '{value}' is already in use. Please enter another one.");
                }

                return new FailedResult("Reserve operation failed.");
            }

            return new SuccessfulResult();
        }

        /// <inheritdoc />
        public async Task<IExecutionResult> UpdateConstraintAsync(string indexName, string oldValue, string newValue)
        {
            var updateOperation = new TableBatchOperation();

            updateOperation.Delete(new UniqueTableEntity(indexName, oldValue));
            updateOperation.Insert(new UniqueTableEntity(indexName, newValue));

            try
            {
                await table.ExecuteBatchAsync(updateOperation);
            }
            catch (StorageException e)
            {
                switch (e.RequestInformation.ExtendedErrorInformation.ErrorCode)
                {
                    case string code when code == TableErrorCodeStrings.EntityAlreadyExists:
                        return new FailedResult($"The name '{newValue}' is already in use. Please enter another one.");
                    case string code when code == StorageErrorCodeStrings.ResourceNotFound:
                        return new FailedResult($"The name '{oldValue}' is not in use. Please enter another one.");
                    case "InvalidDuplicateRow": // occures when new value duplicates old value then no error is needed
                        return new SuccessfulResult();
                    default:
                        return new FailedResult("Update operation failed.");
                }
            }

            return new SuccessfulResult();
        }

        private class UniqueTableEntity : TableEntity
        {
            public UniqueTableEntity(string partition, string key)
            {
                PartitionKey = partition;
                RowKey = Encode(key);
                Timestamp = DateTimeOffset.UtcNow;
                ETag = "*";
                Source = key;
            }

            public string Source { get; set; }

            private static string Encode(string key)
            {
                return Unified.NewCode(Unified.NewHash(Encoding.UTF8.GetBytes(key)));
            }
        }
    }
}

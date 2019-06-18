using System;
using System.Threading.Tasks;
using Intellias.CQRS.Core.Exceptions;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Shared.Protocol;
using Microsoft.WindowsAzure.Storage.Table;
using Microsoft.WindowsAzure.Storage.Table.Protocol;

namespace Intellias.CQRS.DomainServices
{
    /// <summary>
    /// Unique-constraint service
    /// </summary>
    public class UniqueConstraintService : IUniqueConstraintService
    {
        private readonly CloudTable table;

        /// <summary>
        /// ctor
        /// </summary>
        /// <param name="account"></param>
        public UniqueConstraintService(CloudStorageAccount account)
        {
            var client = account.CreateCloudTableClient();
            table = client.GetTableReference(typeof(UniqueConstraintService).Name);

            // Create the CloudTable if it does not exist
            table.CreateIfNotExistsAsync().Wait();
        }

        /// <inheritdoc />
        public async Task RemoveConstraintAsync(string indexName, string value)
        {
            try
            {
                await table.ExecuteAsync(TableOperation.Delete(new TableEntity
                {
                    PartitionKey = indexName,
                    RowKey = value,
                    Timestamp = DateTimeOffset.UtcNow,
                    ETag = "*"
                }));
            }
            catch (StorageException e)
            {
                var errorCode = e.RequestInformation.ExtendedErrorInformation.ErrorCode;
                if (StorageErrorCodeStrings.ResourceNotFound.Equals(errorCode, StringComparison.InvariantCultureIgnoreCase))
                {
                    throw new BusinessRuleValidationException($"The name '{value}' is not in use. Please enter another one.", e);
                }
                else
                {
                    throw new InvalidOperationException("Delete operation failed.", e);
                }
            }
        }


        /// <inheritdoc />
        public async Task ReserveConstraintAsync(string indexName, string value)
        {
            try
            {
                await table.ExecuteAsync(TableOperation.Insert(new TableEntity
                {
                    PartitionKey = indexName,
                    RowKey = value,
                    Timestamp = DateTimeOffset.UtcNow,
                    ETag = "*"
                }));
            }
            catch (StorageException e)
            {
                var errorCode = e.RequestInformation.ExtendedErrorInformation.ErrorCode;
                if (TableErrorCodeStrings.EntityAlreadyExists.Equals(errorCode, StringComparison.InvariantCultureIgnoreCase))
                {
                    throw new BusinessRuleValidationException($"The name '{value}' is already in use. Please enter another one.", e);
                }
                else
                {
                    throw new InvalidOperationException("Reserve operation failed.", e);
                }
            }
        }

        /// <inheritdoc />
        public async Task UpdateConstraintAsync(string indexName, string oldValue, string newValue)
        {
            var updateOperation = new TableBatchOperation();

            updateOperation.Delete(new TableEntity
            {
                PartitionKey = indexName,
                RowKey = oldValue,
                Timestamp = DateTimeOffset.UtcNow,
                ETag = "*"
            });
            updateOperation.Insert(new TableEntity
            {
                PartitionKey = indexName,
                RowKey = newValue,
                Timestamp = DateTimeOffset.UtcNow,
                ETag = "*"
            });

            try
            {
                await table.ExecuteBatchAsync(updateOperation);
            }
            catch (StorageException e)
            {
                var errorCode = e.RequestInformation.ExtendedErrorInformation.ErrorCode;
                if (TableErrorCodeStrings.EntityAlreadyExists.Equals(errorCode, StringComparison.InvariantCultureIgnoreCase))
                {
                    throw new BusinessRuleValidationException($"The name '{newValue}' is already in use. Please enter another one.", e);
                }
                else if (StorageErrorCodeStrings.ResourceNotFound.Equals(errorCode, StringComparison.InvariantCultureIgnoreCase))
                {
                    throw new BusinessRuleValidationException($"The name '{oldValue}' is not in use. Please enter another one.", e);
                }
                else
                {
                    throw new InvalidOperationException("Update operation failed.", e);
                }
            }
        }
    }
}

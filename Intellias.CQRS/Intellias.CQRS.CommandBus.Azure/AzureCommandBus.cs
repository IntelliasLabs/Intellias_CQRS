using System.Threading.Tasks;
using Intellias.CQRS.Core.Commands;
using Intellias.CQRS.Core.Messages;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Queue;
using Microsoft.WindowsAzure.Storage.Table;
using Newtonsoft.Json;

namespace Intellias.CQRS.CommandBus.Azure
{
    /// <inheritdoc />
    public class AzureCommandBus : ICommandBus
    {
        private readonly CloudStorageAccount storageAccount;
        private readonly CloudTableClient tableClient;
        private readonly CloudQueueClient queueClient;

        /// <summary>
        /// Creates azure command bus for cloud storage account
        /// </summary>
        /// <param name="account"></param>
        public AzureCommandBus(CloudStorageAccount account)
        {
            storageAccount = account;
            tableClient = account.CreateCloudTableClient();
            queueClient = account.CreateCloudQueueClient();
        }

        /// <inheritdoc />
        public async Task<ICommandResult> PublishAsync(ICommand msg)
        {
            try
            {
                var agreegateType = msg.Metadata[MetadataKey.AgreegateType];
                var commandContent = JsonConvert.SerializeObject(msg, Formatting.Indented);

                // Create table
                var table = tableClient.GetTableReference(agreegateType);
                await table.CreateIfNotExistsAsync();

                // Create queue
                var queue = queueClient.GetQueueReference(agreegateType);
                await queue.CreateIfNotExistsAsync();

                // Save to table for history
                var operation = TableOperation.Insert(new CommandTableEntity(msg, commandContent));
                var result = await table.ExecuteAsync(operation);

                await queue.AddMessageAsync(new CloudQueueMessage(commandContent));
            }
            catch(StorageException ex)
            {
                return await Task.FromResult(CommandResult.Fail(ex.Message));
            }

            return await Task.FromResult(CommandResult.Success);
        }
    }
}

using System.Threading.Tasks;
using Intellias.CQRS.CommandStore.AzureTable.Extensions;
using Intellias.CQRS.Core.Commands;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Table;

namespace Intellias.CQRS.CommandStore.AzureTable
{
    /// <inheritdoc />
    public class AzureTableCommandStore : ICommandStore
    {
        private readonly CloudTable commandTable;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="storeConnectionString"></param>
        public AzureTableCommandStore(string storeConnectionString)
        {
            var client = CloudStorageAccount
                .Parse(storeConnectionString)
                .CreateCloudTableClient();

            commandTable = client.GetTableReference(nameof(CommandStore));
            // Create the CloudTable if it does not exist
            commandTable.CreateIfNotExistsAsync().Wait();
        }

        /// <summary>
        /// InsertCommand
        /// </summary>
        /// <param name="command"></param>
        /// <returns></returns>
        public Task SaveAsync(ICommand command)
        {
            var operation = TableOperation.Insert(command.ToStoreCommand());
            return commandTable.ExecuteAsync(operation);
        }
    }
}

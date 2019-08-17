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
        /// Initializes a new instance of the <see cref="AzureTableCommandStore"/> class.
        /// </summary>
        /// <param name="storeConnectionString">Storage Connection string.</param>
        public AzureTableCommandStore(string storeConnectionString)
        {
            var client = CloudStorageAccount
                .Parse(storeConnectionString)
                .CreateCloudTableClient();

            commandTable = client.GetTableReference(nameof(CommandStore));

            if (!commandTable.ExistsAsync().GetAwaiter().GetResult())
            {
                commandTable.CreateIfNotExistsAsync().Wait();
            }
        }

        /// <summary>
        /// InsertCommand.
        /// </summary>
        /// <param name="command">Command to Save.</param>
        /// <returns>Simple Task.</returns>
        public Task SaveAsync(ICommand command)
        {
            var operation = TableOperation.Insert(command.ToStoreCommand());
            return commandTable.ExecuteAsync(operation);
        }
    }
}

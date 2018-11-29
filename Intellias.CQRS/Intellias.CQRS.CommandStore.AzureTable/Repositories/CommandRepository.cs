using System.Threading.Tasks;
using Intellias.CQRS.CommandStore.AzureTable.Extensions;
using Intellias.CQRS.Core.Commands;
using Microsoft.WindowsAzure.Storage.Table;

namespace Intellias.CQRS.CommandStore.AzureTable.Repositories
{
    /// <summary>
    /// CommandRepository
    /// </summary>
    public class CommandRepository
    {
        private readonly CloudTable commandTable;

        /// <summary>
        /// Init ctor
        /// </summary>
        /// <param name="client"></param>
        public CommandRepository(CloudTableClient client)
        {
            commandTable = client.GetTableReference("#CommandStore");
            // Create the CloudTable if it does not exist
            commandTable.CreateIfNotExistsAsync().Wait();
        }

        /// <summary>
        /// InsertCommand
        /// </summary>
        /// <param name="command"></param>
        /// <returns></returns>
        public Task InsertCommandAsync(ICommand command)
        {
            var operation = TableOperation.Insert(command.ToStoreCommand());
            return commandTable.ExecuteAsync(operation);
        }
    }
}

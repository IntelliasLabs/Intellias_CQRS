using System.Diagnostics.CodeAnalysis;
using System.Threading.Tasks;
using Intellias.CQRS.CommandStore.AzureTable.Extensions;
using Intellias.CQRS.Core.Commands;
using Intellias.CQRS.Persistence.AzureStorage.Common;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Table;

namespace Intellias.CQRS.CommandStore.AzureTable
{
    /// <inheritdoc />
    [ExcludeFromCodeCoverage]
    public class AzureTableCommandStore : ICommandStore
    {
        private readonly CloudTableProxy tableProxy;

        /// <summary>
        /// Initializes a new instance of the <see cref="AzureTableCommandStore"/> class.
        /// </summary>
        /// <param name="storeConnectionString">Storage Connection string.</param>
        public AzureTableCommandStore(string storeConnectionString)
        {
            var client = CloudStorageAccount
                .Parse(storeConnectionString)
                .CreateCloudTableClient();

            var commandTableReference = client.GetTableReference(nameof(CommandStore));

            tableProxy = new CloudTableProxy(commandTableReference, ensureTableExists: true);
        }

        /// <summary>
        /// InsertCommand.
        /// </summary>
        /// <param name="command">Command to Save.</param>
        /// <returns>Simple Task.</returns>
        public Task SaveAsync(ICommand command)
        {
            var operation = TableOperation.Insert(command.ToStoreCommand());
            return tableProxy.ExecuteAsync(operation);
        }
    }
}

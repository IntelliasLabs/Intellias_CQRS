using System.Threading.Tasks;
using Intellias.CQRS.CommandStore.AzureTable.Repositories;
using Intellias.CQRS.Core.Commands;
using Microsoft.WindowsAzure.Storage;

namespace Intellias.CQRS.CommandStore.AzureTable
{
    /// <inheritdoc />
    public class AzureTableCommandStore : ICommandStore
    {
        private readonly CommandRepository commandRepository;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="storeConnectionString"></param>
        public AzureTableCommandStore(string storeConnectionString)
        {
            var client = CloudStorageAccount
                .Parse(storeConnectionString)
                .CreateCloudTableClient();

            commandRepository = new CommandRepository(client);
        }

        /// <inheritdoc />
        public Task SaveAsync(ICommand command) => 
            commandRepository.InsertCommandAsync(command);
    }
}

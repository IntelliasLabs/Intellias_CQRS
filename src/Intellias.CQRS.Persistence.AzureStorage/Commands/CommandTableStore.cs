using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Intellias.CQRS.Core.Commands;
using Intellias.CQRS.Persistence.AzureStorage.Common;
using Microsoft.Azure.Cosmos.Table;

namespace Intellias.CQRS.Persistence.AzureStorage.Commands
{
    /// <summary>
    /// Command store implementation based on Azure Storage Account Tables.
    /// </summary>
    public class CommandTableStore : BaseTableStorage2, ICommandStore
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="CommandTableStore"/> class.
        /// </summary>
        /// <param name="options">Table storage options.</param>
        public CommandTableStore(ITableStorageOptions options)
            : base(options, "CommandStore")
        {
        }

        /// <inheritdoc />
        public async Task SaveAsync(ICommand command)
        {
            var entity = Serialize(command);
            await this.InsertAsync(entity);
        }

        /// <summary>
        /// Gets all commands.
        /// </summary>
        /// <returns>Collection of commands.</returns>
        public async Task<IReadOnlyCollection<ICommand>> GetAllAsync()
        {
            var entities = await QueryAllSegmentedAsync(new TableQuery<DynamicTableEntity>());
            return entities.Select(Deserialize).ToArray();
        }

        private static ICommand Deserialize(DynamicTableEntity entity)
        {
            var command = (ICommand)AzureTableSerializer.Deserialize(entity);
            return command;
        }

        private DynamicTableEntity Serialize(ICommand command)
        {
            return new DynamicTableEntity(command.AggregateRootId, command.Id)
            {
                Properties = AzureTableSerializer.Serialize(command, persistType: true)
            };
        }
    }
}
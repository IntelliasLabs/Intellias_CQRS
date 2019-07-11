using System;
using Intellias.CQRS.CommandStore.AzureTable.Documents;
using Intellias.CQRS.Core;
using Intellias.CQRS.Core.Commands;

namespace Intellias.CQRS.CommandStore.AzureTable.Extensions
{
    /// <summary>
    /// CommandStoreItemExtensions
    /// </summary>
    public static class CommandStoreItemExtensions
    {
        /// <summary>
        /// ToStoreCommandS
        /// </summary>
        /// <param name="command"></param>
        /// <returns></returns>
        public static CommandTableEntity ToStoreCommand(this ICommand command) =>
        new CommandTableEntity
        {
            PartitionKey = command.AggregateRootId,
            RowKey = command.Id,
            TypeName = command.TypeName,
            ExpectedVersion = command.ExpectedVersion,
            Data = command.ToJson(),
            ETag = "*",
            Timestamp = DateTimeOffset.UtcNow
        };
    }
}

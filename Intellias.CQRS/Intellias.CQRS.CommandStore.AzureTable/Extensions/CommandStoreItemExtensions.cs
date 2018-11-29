using System;
using Intellias.CQRS.CommandStore.AzureTable.Documents;
using Intellias.CQRS.Core.Commands;
using Intellias.CQRS.Core.Config;
using Newtonsoft.Json;

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
            CommandType = command.GetType().Name,
            Version = command.ExpectedVersion,
            Data = JsonConvert.SerializeObject(command, CqrsSettings.JsonConfig()),
            ETag = "*",
            Timestamp = DateTimeOffset.UtcNow
        };
    }
}

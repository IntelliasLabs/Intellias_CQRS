using System;
using System.Diagnostics.CodeAnalysis;
using Intellias.CQRS.Core.Commands;
using Intellias.CQRS.Core.Messages;

namespace Intellias.CQRS.Messaging.AzureServiceBus.Commands
{
    /// <summary>
    /// Default Command Bus options.
    /// </summary>
    [ExcludeFromCodeCoverage]
    public class DefaultCommandBusOptions : ICommandBusOptions
    {
        /// <inheritdoc />
        public string ConnectionString { get; set; } = string.Empty;

        /// <inheritdoc />
        public string Name { get; set; } = string.Empty;

        /// <inheritdoc />
        public Func<ICommand, string> GetPartition { get; set; } = _ => AbstractMessage.GlobalSessionId;
    }
}
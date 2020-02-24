using System;

namespace Intellias.CQRS.Core.Commands
{
    /// <summary>
    /// Configures <see cref="ICommandBus{TCommandBusOptions}"/>.
    /// </summary>
    public interface ICommandBusOptions
    {
        /// <summary>
        /// Connection string to Command Bus.
        /// </summary>
        string ConnectionString { get; }

        /// <summary>
        /// Command Bus name.
        /// </summary>
        string Name { get; }

        /// <summary>
        /// Provides partition key from Command.
        /// </summary>
        Func<ICommand, string> GetPartition { get; }
    }
}
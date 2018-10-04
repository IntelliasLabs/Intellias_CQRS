using Intellias.CQRS.Core.Messages;

namespace Intellias.CQRS.Core.Commands
{
    /// <inheritdoc cref="ICommand" />
    public abstract class Command : AbstractMessage, ICommand
    {
        /// <inheritdoc />
        public int ExpectedVersion { get; set; }

        /// <inheritdoc />
        public string AggregateRootId { get; set; }
    }
}

namespace Intellias.CQRS.Core.Commands
{
    /// <inheritdoc />
    public abstract class Command : ICommand
    {
        /// <inheritdoc />
        public int ExpectedVersion { get; set; }

        /// <inheritdoc />
        public string AggregateRootId { get; set; }
    }
}

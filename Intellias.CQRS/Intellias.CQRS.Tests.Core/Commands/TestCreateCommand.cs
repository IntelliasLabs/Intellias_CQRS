using Intellias.CQRS.Core.Commands;

namespace Intellias.CQRS.Tests.Core.Commands
{
    /// <inheritdoc />
    public class TestCreateCommand : Command
    {
        /// <inheritdoc />
        public TestCreateCommand(string aggregatedRootId)
        {
            AggregateRootId = aggregatedRootId;
        }
    }
}

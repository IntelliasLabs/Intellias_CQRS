using Intellias.CQRS.Core.Commands;

namespace Intellias.CQRS.Tests.Core.Commands
{
    /// <inheritdoc />
    public class TestUpdateCommand : Command
    {
        /// <inheritdoc />
        public TestUpdateCommand(string aggregatedRootId)
        {
            AggregateRootId = aggregatedRootId;
        }
    }
}

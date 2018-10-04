using Intellias.CQRS.Core.Commands;

namespace Intellias.CQRS.EventStore.AzureTable.Tests.Core
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

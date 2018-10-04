using Intellias.CQRS.Core.Commands;

namespace Intellias.CQRS.EventStore.AzureTable.Tests.Core
{
    /// <inheritdoc />
    public class TestDeleteCommand : Command
    {
        /// <inheritdoc />
        public TestDeleteCommand(string aggregatedRootId)
        {
            AggregateRootId = aggregatedRootId;
        }
    }
}

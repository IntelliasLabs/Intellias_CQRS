using Intellias.CQRS.Core.Commands;

namespace Intellias.CQRS.EventStore.AzureTable.Tests.Core
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

using System.Linq;
using Intellias.CQRS.Core.Commands;
using Intellias.CQRS.Core.Events;
using Intellias.CQRS.Core.Storage;
using Intellias.CQRS.Tests.CommandHandlers;
using Intellias.CQRS.Tests.Core.Commands;
using Intellias.CQRS.Tests.Core.Events;
using Intellias.CQRS.Tests.Core.Fakes;
using Intellias.CQRS.Tests.Core.Queries;
using Intellias.CQRS.Tests.EventHandlers;
using Xunit;

namespace Intellias.CQRS.Tests
{
    /// <summary>
    /// Full CQRS scenario test
    /// </summary>
    public class FullScenarioTests
    {
        /// <summary>
        /// Write Side Command Bus
        /// </summary>
        private readonly ICommandBus commandBus;

        /// <summary>
        /// Read Side Query Executor
        /// </summary>
        private readonly IQueryModelReader<TestQueryModel> queryReader;

        /// <summary>
        /// Setup test infrastructure
        /// </summary>
        public FullScenarioTests()
        {
            // Prepare query storage
            var queryStore = new InProcessQueryStore<TestQueryModel>();

            // Attach event handlers to query store
            var eventHandlers = new DemoEventHandlers(queryStore);

            // Create event bus and subscribe handlers to it
            var eventBus = new InProcessEventBus();
            eventBus.AddHandler<TestCreatedEvent>(eventHandlers);
            eventBus.AddHandler<TestUpdatedEvent>(eventHandlers);
            eventBus.AddHandler<TestDeletedEvent>(eventHandlers);

            // Register event store to populate events into event bus
            IEventStore eventStore = new InProcessEventStore(eventBus);

            // Attach event store to command handlers
            var commandHandlers = new DemoCommandHandlers(eventStore);

            // Create command bus and subscribe command handlers
            var inProcCommandBus = new InProcessCommandBus();
            inProcCommandBus.AddHandler<TestCreateCommand>(commandHandlers);
            inProcCommandBus.AddHandler<TestUpdateCommand>(commandHandlers);
            inProcCommandBus.AddHandler<TestDeleteCommand>(commandHandlers);

            // Set command bus instance
            commandBus = inProcCommandBus;
            queryReader = queryStore;
        }

        /// <summary>
        /// Full demo test
        /// </summary>
        [Fact]
        public void DemoTest()
        {
            // Send command to create aggregate
            var createCommand = new TestCreateCommand { TestData = "Test data" };
            var createResult = commandBus.PublishAsync(createCommand).Result;
            Assert.NotNull(createResult);

            // Get read query
            var queryResult = queryReader.GetAllAsync().Result;
            var firstItem = queryResult.First();
            Assert.Equal(1, queryResult.Count);
            Assert.Equal(createCommand.TestData, firstItem.TestData);

            // Update aggregate using previous query
            var updateCommand = new TestUpdateCommand
            {
                TestData = "Test data updated",
                AggregateRootId = firstItem.Id,
                ExpectedVersion = firstItem.Version
            };
            var updateResult = commandBus.PublishAsync(updateCommand).Result;
            Assert.NotNull(updateResult);

            // Check if item was updated
            var updatedQueryResult = queryReader.GetAllAsync().Result;
            var updatedFirstItem = updatedQueryResult.First();
            Assert.Equal(updateCommand.TestData, updatedFirstItem.TestData);

            // Deactivate aggregate using previous query
            var deactivateCommand = new TestDeleteCommand
            {
                AggregateRootId = updatedFirstItem.Id,
                ExpectedVersion = updatedFirstItem.Version
            };
            var deactivateResult = commandBus.PublishAsync(deactivateCommand).Result;
            Assert.NotNull(deactivateResult);

            // Check if there is no elements in read query
            var queryRemovedResult = queryReader.GetAllAsync().Result;
            Assert.Equal(0, queryRemovedResult.Count);
        }
    }
}

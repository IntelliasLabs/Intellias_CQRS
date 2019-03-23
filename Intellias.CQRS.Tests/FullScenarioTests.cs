using System;
using System.Collections.Generic;
using System.Linq;
using Intellias.CQRS.Core.Extensions;
using Intellias.CQRS.Core.Messages;
using Intellias.CQRS.Core.Storage;
using Intellias.CQRS.Tests.Core.CommandHandlers;
using Intellias.CQRS.Tests.Core.Commands;
using Intellias.CQRS.Tests.Core.EventHandlers;
using Intellias.CQRS.Tests.Core.Events;
using Intellias.CQRS.Tests.Core.Fakes;
using Intellias.CQRS.Tests.Core.Queries;
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
        private readonly InProcessCommandBus commandBus;

        /// <summary>
        /// Read Side Query Executor
        /// </summary>
        private readonly IQueryModelReader<TestQueryModel> queryReader;

        /// <summary>
        /// Setup test infrastructure
        /// </summary>
        public FullScenarioTests()
        {
            var tables = new Dictionary<Type, Dictionary<string, object>>();

            // Prepare query storage
            var queryStore = new InProcessQueryStore<TestQueryModel>(tables);

            // Clear storage to reproduce stable test results
            queryStore.ClearAsync().Wait();

            // Attach event handlers to query store
            var eventHandlers = new DemoEventHandlers(queryStore);

            // Create event bus and subscribe handlers to it
            var eventBus = new InProcessEventBus();
            eventBus.AddHandler<TestCreatedEvent>(eventHandlers);
            eventBus.AddHandler<TestUpdatedEvent>(eventHandlers);
            eventBus.AddHandler<TestDeletedEvent>(eventHandlers);

            // Register event store to populate events into event bus
            var eventStore = new InProcessEventStore(eventBus);

            // Attach event store to command handlers
            var commandHandlers = new DemoCommandHandlers(eventStore);

            // Create command bus and subscribe command handlers
            commandBus = new InProcessCommandBus();
            commandBus.AddHandler<TestCreateCommand>(commandHandlers);
            commandBus.AddHandler<TestUpdateCommand>(commandHandlers);
            commandBus.AddHandler<TestDeleteCommand>(commandHandlers);

            queryReader = queryStore;
        }

        /// <summary>
        /// Full demo test
        /// </summary>
        [Fact]
        public void DemoTest()
        {
            // Send command to create aggregate
            var createCommand = new TestCreateCommand
            {
                TestData = "Test data",
                AggregateRootId = Unified.NewCode()
            };
            createCommand.Wrap();
            createCommand.Validate();
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
            updateCommand.Wrap();
            updateCommand.Validate();
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
            deactivateCommand.Wrap();
            deactivateCommand.Validate();
            var deactivateResult = commandBus.PublishAsync(deactivateCommand).Result;
            Assert.NotNull(deactivateResult);

            // Check if there is no elements in read query
            var queryRemovedResult = queryReader.GetAllAsync().Result;
            Assert.Equal(0, queryRemovedResult.Count);
        }
    }
}

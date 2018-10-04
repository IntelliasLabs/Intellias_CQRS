using Intellias.CQRS.Core.Commands;
using Intellias.CQRS.Core.Events;
using Intellias.CQRS.Core.Storage;
using Intellias.CQRS.Core.Tests.CommandHandlers;
using Intellias.CQRS.Core.Tests.Commands;
using Intellias.CQRS.Core.Tests.Domain;
using Intellias.CQRS.Core.Tests.EventHandlers;
using Intellias.CQRS.Core.Tests.Events;
using Intellias.CQRS.Core.Tests.Fakes;
using Xunit;

namespace Intellias.CQRS.Core.Tests
{
    /// <summary>
    /// Full CQRS scenario test
    /// </summary>
    public class FullScenarioTests
    {
        /// <summary>
        /// Full demo test
        /// </summary>
        [Fact]
        public void DemoTest()
        {
            var demoCommand = new DemoCreateCommand { Name = "Test data" };

            IEventBus eventBus = new InProcessEventBus<DemoCreatedEvent>(new DemoEventHandlers());
            IEventStore eventStore = new InProcessEventStore(eventBus);

            IAggregateStorage<DemoRoot> rootStorage = new InProcessAggregateStorage<DemoRoot>();
            ICommandBus commandBus = new InProcessCommandBus<DemoCreateCommand>(new DemoCommandHandlers(rootStorage));

            var result = commandBus.PublishAsync(demoCommand);

            Assert.NotNull(result);
        }
    }
}

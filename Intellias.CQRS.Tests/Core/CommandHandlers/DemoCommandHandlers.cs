using System.Linq;
using System.Threading.Tasks;
using Intellias.CQRS.Core.Commands;
using Intellias.CQRS.Core.Events;
using Intellias.CQRS.Core.Results;
using Intellias.CQRS.Tests.Core.Commands;
using Intellias.CQRS.Tests.Core.Domain;

namespace Intellias.CQRS.Tests.Core.CommandHandlers
{
    /// <summary>
    /// Demo command handlers
    /// </summary>
    public class DemoCommandHandlers : AbstractCommandHandler,
        ICommandHandler<TestCreateCommand>,
        ICommandHandler<TestUpdateCommand>,
        ICommandHandler<TestDeleteCommand>
    {
        /// <summary>
        /// Creates an instance of demo command handler
        /// </summary>
        /// <param name="store">Event store</param>
        /// <param name="bus">Event bus</param>
        public DemoCommandHandlers(IEventStore store, IEventBus bus) : base(store, bus)
        { }

        /// <summary>
        /// Handels create command
        /// </summary>
        /// <param name="command"></param>
        /// <returns></returns>
        public async Task<IExecutionResult> HandleAsync(TestCreateCommand command)
        {
            var ar = new TestRoot(command);

            var processedEvents = await store.SaveAsync(ar);
            await Task.WhenAll(processedEvents.Select(bus.PublishAsync));

            return await Task.FromResult(ExecutionResult.Successful);
        }

        /// <summary>
        /// Handle Test Update Command
        /// </summary>
        /// <param name="command">command</param>
        /// <returns>result</returns>
        public async Task<IExecutionResult> HandleAsync(TestUpdateCommand command)
        {
            var ar = await GetAggregateAsync<TestRoot, TestState>(command.AggregateRootId);
            var result = ar.Update(command);

            var processedEvents = await store.SaveAsync(ar);
            await Task.WhenAll(processedEvents.Select(bus.PublishAsync));

            return await Task.FromResult(result);
        }

        /// <summary>
        /// Handle Test Delete Command
        /// </summary>
        /// <param name="command">command</param>
        /// <returns>result</returns>
        public async Task<IExecutionResult> HandleAsync(TestDeleteCommand command)
        {
            var ar = await GetAggregateAsync<TestRoot, TestState>(command.AggregateRootId);
            var result = ar.Deactivate();

            var processedEvents = await store.SaveAsync(ar);
            await Task.WhenAll(processedEvents.Select(bus.PublishAsync));

            return await Task.FromResult(result);
        }
    }
}

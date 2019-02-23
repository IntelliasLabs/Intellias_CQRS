using System.Threading.Tasks;
using Intellias.CQRS.Core.Commands;
using Intellias.CQRS.Core.Events;
using Intellias.CQRS.Core.Messages;
using Intellias.CQRS.Tests.Core.Commands;
using Intellias.CQRS.Tests.Core.Domain;

namespace Intellias.CQRS.Tests.CommandHandlers
{
    /// <summary>
    /// Demo command handlers
    /// </summary>
    public class DemoCommandHandlers : ICommandHandler<TestCreateCommand>,
        ICommandHandler<TestUpdateCommand>,
        ICommandHandler<TestDeleteCommand>
    {
        private readonly IEventStore store;

        /// <summary>
        /// Creates an instance of demo command handler
        /// </summary>
        /// <param name="store"></param>
        public DemoCommandHandlers(IEventStore store)
        {
            this.store = store;
        }

        /// <summary>
        /// Handels create command
        /// </summary>
        /// <param name="command"></param>
        /// <returns></returns>
        public async Task<IExecutionResult> HandleAsync(TestCreateCommand command)
        {
            command.AggregateRootId = Unified.NewCode();

            var ar = new TestRoot(command);

            await store.SaveAsync(ar);

            return await Task.FromResult(ExecutionResult.Success);
        }

        /// <summary>
        /// Handle Test Update Command
        /// </summary>
        /// <param name="command">command</param>
        /// <returns>result</returns>
        public async Task<IExecutionResult> HandleAsync(TestUpdateCommand command)
        {
            var events = await store.GetAsync(command.AggregateRootId, 0);
            var ar = new TestRoot(command.AggregateRootId);
            ar.LoadFromHistory(events);

            var result = ar.Update(command);

            await store.SaveAsync(ar);

            return await Task.FromResult(result);
        }

        /// <summary>
        /// Handle Test Delete Command
        /// </summary>
        /// <param name="command">command</param>
        /// <returns>result</returns>
        public async Task<IExecutionResult> HandleAsync(TestDeleteCommand command)
        {
            var events = await store.GetAsync(command.AggregateRootId, 0);
            var ar = new TestRoot(command.AggregateRootId);
            ar.LoadFromHistory(events);

            var result = ar.Deactivate();

            await store.SaveAsync(ar);

            return await Task.FromResult(result);
        }
    }
}

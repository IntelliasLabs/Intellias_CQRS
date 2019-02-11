using System.Threading.Tasks;
using Intellias.CQRS.Core.Events;
using Intellias.CQRS.Core.Messages;
using Intellias.CQRS.Core.Storage;
using Intellias.CQRS.Tests.Core.Events;
using Intellias.CQRS.Tests.Core.Queries;

namespace Intellias.CQRS.Core.Tests.EventHandlers
{
    /// <summary>
    /// Demo command handlers
    /// </summary>
    public class DemoEventHandlers : 
        IEventHandler<TestCreatedEvent>,
        IEventHandler<TestUpdatedEvent>,
        IEventHandler<TestDeletedEvent>
    {
        private readonly IQueryModelStore<TestQueryModel> store;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="store"></param>
        public DemoEventHandlers(IQueryModelStore<TestQueryModel> store)
        {
            this.store = store;
        }

        /// <summary>
        /// Applies create event
        /// </summary>
        /// <param name="event">Event</param>
        /// <returns>Result</returns>
        public async Task<IExecutionResult> HandleAsync(TestCreatedEvent @event)
        {
            await store.CreateAsync(new TestQueryModel
            {
                Id = @event.AggregateRootId,
                ParentId = Unified.Dummy,
                TestData = @event.TestData,
                Version = @event.Version
            });

            return ExecutionResult.Success;
        }

        /// <summary>
        /// Applies deleted event
        /// </summary>
        /// <param name="event">Event</param>
        /// <returns>Result</returns>
        public async Task<IExecutionResult> HandleAsync(TestDeletedEvent @event)
        {
            await store.DeleteAsync(@event.AggregateRootId);

            return ExecutionResult.Success;
        }

        /// <summary>
        /// Applies updated event
        /// </summary>
        /// <param name="event">Event</param>
        /// <returns>Result</returns>
        public async Task<IExecutionResult> HandleAsync(TestUpdatedEvent @event)
        {
            await store.UpdateAsync(new TestQueryModel
            {
                Id = @event.AggregateRootId,
                ParentId = Unified.Dummy,
                TestData = @event.TestData,
                Version = @event.Version
            });

            return ExecutionResult.Success;
        }
    }
}

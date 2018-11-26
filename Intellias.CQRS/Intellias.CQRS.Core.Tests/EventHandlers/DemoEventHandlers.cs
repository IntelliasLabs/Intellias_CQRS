using System.Threading.Tasks;
using Intellias.CQRS.Core.Events;
using Intellias.CQRS.Core.Storage;
using Intellias.CQRS.Core.Tests.Queries;
using Intellias.CQRS.Tests.Core.Events;

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
        private readonly IQueryModelStore<DemoQueryModel> store;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="store"></param>
        public DemoEventHandlers(IQueryModelStore<DemoQueryModel> store)
        {
            this.store = store;
        }

        /// <summary>
        /// Applies create event
        /// </summary>
        /// <param name="event">Event</param>
        /// <returns>Result</returns>
        public Task HandleAsync(TestCreatedEvent @event)
        {
            return store.CreateAsync(new DemoQueryModel
            {
                Id = @event.AggregateRootId,
                TestData = @event.TestData
            });
        }

        /// <summary>
        /// Applies deleted event
        /// </summary>
        /// <param name="event">Event</param>
        /// <returns>Result</returns>
        public Task HandleAsync(TestDeletedEvent @event)
        {
            return store.DeleteAsync(@event.AggregateRootId);
        }

        /// <summary>
        /// Applies updated event
        /// </summary>
        /// <param name="event">Event</param>
        /// <returns>Result</returns>
        public Task HandleAsync(TestUpdatedEvent @event)
        {
            return store.UpdateAsync(new DemoQueryModel
            {
                Id = @event.AggregateRootId,
                TestData = @event.TestData
            });
        }
    }
}

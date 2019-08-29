using System.Threading.Tasks;
using Intellias.CQRS.Core.Events;
using Intellias.CQRS.Core.Messages;
using Intellias.CQRS.Core.Storage;
using Intellias.CQRS.Tests.Core.Events;
using Intellias.CQRS.Tests.Core.Queries;

namespace Intellias.CQRS.Tests.Core.EventHandlers
{
    /// <summary>
    /// Demo command handlers
    /// </summary>
    public class DemoEventHandlers :
        IEventHandler<TestCreatedEvent>,
        IEventHandler<TestUpdatedEvent>,
        IEventHandler<TestDeletedEvent>
    {
        private readonly IQueryModelWriter<TestQueryModel> store;

        /// <summary>
        /// DemoEventHandlers
        /// </summary>
        /// <param name="store"></param>
        public DemoEventHandlers(IQueryModelWriter<TestQueryModel> store)
        {
            this.store = store;
        }

        /// <summary>
        /// Applies create event
        /// </summary>
        /// <param name="event">Event</param>
        /// <returns>Result</returns>
        public async Task HandleAsync(TestCreatedEvent @event)
        {
            await store.CreateAsync(new TestQueryModel
            {
                Id = @event.AggregateRootId,
                ParentId = Unified.Dummy,
                TestData = @event.TestData,
                Version = @event.Version
            });
        }

        /// <summary>
        /// Applies deleted event
        /// </summary>
        /// <param name="event">Event</param>
        /// <returns>Result</returns>
        public async Task HandleAsync(TestDeletedEvent @event)
        {
            await store.DeleteAsync(@event.AggregateRootId);
        }

        /// <summary>
        /// Applies updated event
        /// </summary>
        /// <param name="event">Event</param>
        /// <returns>Result</returns>
        public async Task HandleAsync(TestUpdatedEvent @event)
        {
            await store.UpdateAsync(@event.AggregateRootId, model =>
            {
                model.ParentId = Unified.Dummy;
                model.TestData = @event.TestData;
                model.Version = @event.Version;
            });
        }
    }
}

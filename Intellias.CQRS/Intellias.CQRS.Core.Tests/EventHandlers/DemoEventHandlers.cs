using System.Collections.Generic;
using System.Threading.Tasks;
using Intellias.CQRS.Core.Events;
using Intellias.CQRS.Tests.Core.Events;
using Intellias.CQRS.Tests.Core.Queries;

namespace Intellias.CQRS.Core.Tests.EventHandlers
{
    /// <summary>
    /// Demo command handlers
    /// </summary>
    public class DemoEventHandlers : IEventHandler<TestCreatedEvent>,
        IEventHandler<TestUpdatedEvent>,
        IEventHandler<TestDeletedEvent>
    {
        private readonly Dictionary<string, DemoReadModel> store;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="store"></param>
        public DemoEventHandlers(Dictionary<string, DemoReadModel> store)
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
            store.Add(@event.AggregateRootId, new DemoReadModel
            {
                Id = @event.AggregateRootId,
                TestData = @event.TestData
            });
            return Task.CompletedTask;
        }

        /// <summary>
        /// Applies deleted event
        /// </summary>
        /// <param name="event">Event</param>
        /// <returns>Result</returns>
        public Task HandleAsync(TestDeletedEvent @event)
        {
            store.Remove(@event.AggregateRootId);
            return Task.CompletedTask;
        }

        /// <summary>
        /// Applies updated event
        /// </summary>
        /// <param name="event">Event</param>
        /// <returns>Result</returns>
        public Task HandleAsync(TestUpdatedEvent @event)
        {
            store[@event.AggregateRootId].TestData = @event.TestData;
            return Task.CompletedTask;
        }
    }
}

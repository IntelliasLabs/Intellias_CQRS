using System.Threading.Tasks;
using Intellias.CQRS.Core.Events;
using Intellias.CQRS.Tests.Core.Events;

namespace Intellias.CQRS.Core.Tests.EventHandlers
{
    /// <summary>
    /// Demo command handlers
    /// </summary>
    public class DemoEventHandlers : IEventHandler<TestCreatedEvent>,
        IEventHandler<TestUpdatedEvent>,
        IEventHandler<TestDeletedEvent>
    {
        /// <summary>
        /// Applies create event
        /// </summary>
        /// <param name="message">Event</param>
        /// <returns>Result</returns>
        public async Task<IEventResult> HandleAsync(TestCreatedEvent message)
        {
            return await Task.FromResult(EventResult.Success);
        }

        /// <summary>
        /// Applies deleted event
        /// </summary>
        /// <param name="message">Event</param>
        /// <returns>Result</returns>
        public async Task<IEventResult> HandleAsync(TestDeletedEvent message)
        {
            return await Task.FromResult(EventResult.Success);
        }

        /// <summary>
        /// Applies updated event
        /// </summary>
        /// <param name="message">Event</param>
        /// <returns>Result</returns>
        public async Task<IEventResult> HandleAsync(TestUpdatedEvent message)
        {
            return await Task.FromResult(EventResult.Success);
        }
    }
}

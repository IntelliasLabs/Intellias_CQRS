using System.Threading.Tasks;
using Intellias.CQRS.Core.Events;
using Intellias.CQRS.Tests.Core.Events;

namespace Intellias.CQRS.Core.Tests.EventHandlers
{
    /// <summary>
    /// Demo command handlers
    /// </summary>
    public class DemoEventHandlers : IEventHandler<TestCreatedEvent>
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
    }
}

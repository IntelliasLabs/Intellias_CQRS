using System.Threading.Tasks;
using Intellias.CQRS.Core.Events;
using Intellias.CQRS.Tests.Core.Events;

namespace Intellias.CQRS.Tests.Core.EventHandlers
{
    /// <summary>
    /// Test event handler with only 1 wrapped event.
    /// </summary>
    public class WrappedEventHandler :
        IEventHandler<WrappedTestCreatedEvent>
    {
        /// <inheritdoc />
        public Task HandleAsync(WrappedTestCreatedEvent @event) => Task.CompletedTask;
    }
}

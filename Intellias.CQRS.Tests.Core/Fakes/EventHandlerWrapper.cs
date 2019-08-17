using System.Threading.Tasks;
using Intellias.CQRS.Core.Events;

namespace Intellias.CQRS.Tests.Core.Fakes
{
    /// <inheritdoc />
    public class EventHandlerWrapper<T> : IEventHandler<IEvent>
        where T : IEvent
    {
        private readonly IEventHandler<T> handler;

        /// <summary>
        /// Initializes a new instance of the <see cref="EventHandlerWrapper{T}"/> class.
        /// </summary>
        /// <param name="handler">Event Handler.</param>
        public EventHandlerWrapper(IEventHandler<T> handler)
        {
            this.handler = handler;
        }

        /// <inheritdoc />
        public Task HandleAsync(IEvent @event) =>
            handler.HandleAsync((T)@event);
    }
}

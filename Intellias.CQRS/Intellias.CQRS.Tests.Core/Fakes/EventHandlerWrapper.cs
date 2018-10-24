using System.Threading.Tasks;
using Intellias.CQRS.Core.Events;

namespace Intellias.CQRS.Tests.Core.Fakes
{
    /// <inheritdoc />
    public class EventHandlerWrapper<T> : IEventHandler<IEvent> where T : IEvent
    {
        private readonly IEventHandler<T> handler;

        /// <summary>
        /// Constructs handler wrapper from func
        /// </summary>
        /// <param name="handler"></param>
        public EventHandlerWrapper(IEventHandler<T> handler)
        {
            this.handler = handler;
        }

        /// <inheritdoc />
        public Task HandleAsync(IEvent @event)
        {
            return handler.HandleAsync((T)@event);
        }
    }
}

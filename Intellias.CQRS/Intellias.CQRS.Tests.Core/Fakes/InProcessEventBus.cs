using System.Linq;
using System.Threading.Tasks;
using Intellias.CQRS.Core.Events;
using Intellias.CQRS.Core.Messages;

namespace Intellias.CQRS.Tests.Core.Fakes
{
    /// <inheritdoc />
    public class InProcessEventBus<T> : IEventBus
        where T : IEvent, new()
    {
        private readonly IMessageBus<IMessage, IExecutionResult> bus;

        /// <summary>
        /// Creates event bus
        /// </summary>
        /// <param name="handlers">event handlers</param>
        public InProcessEventBus(params IEventHandler<T>[] handlers)
        {
            bus = new InProcessBus(handlers.Select(h => (IHandler<IMessage, IExecutionResult>)new HandlerWrapper(async msg => await h.HandleAsync((T)msg).ConfigureAwait(false))).ToArray());
        }

        /// <inheritdoc />
        public async Task<IEventResult> PublishAsync(IEvent msg)
        {
            var result = await bus.PublishAsync(msg).ConfigureAwait(false);
            return await Task.FromResult((IEventResult)result).ConfigureAwait(false);
        }
    }
}

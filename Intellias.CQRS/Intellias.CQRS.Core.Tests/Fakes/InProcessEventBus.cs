using System.Linq;
using System.Threading.Tasks;
using Intellias.CQRS.Core.Events;
using Intellias.CQRS.Core.Messages;

namespace Intellias.CQRS.Core.Tests.Fakes
{
    internal class InProcessEventBus<T> : IEventBus
        where T : IEvent, new()
    {
        private readonly IMessageBus<IMessage, IExecutionResult> bus;

        public InProcessEventBus(params IEventHandler<T>[] handlers)
        {
            bus = new InProcessBus(handlers.Select(h => (IHandler<IMessage, IExecutionResult>)new HandlerWrapper(async msg => await h.HandleAsync((T)msg))).ToArray());
        }

        public async Task<IEventResult> PublishAsync(IEvent msg)
        {
            var result = await bus.PublishAsync(msg);
            return await Task.FromResult((IEventResult)result);
        }
    }
}

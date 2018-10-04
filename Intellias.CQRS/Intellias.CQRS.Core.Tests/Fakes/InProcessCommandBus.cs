using System.Linq;
using System.Threading.Tasks;
using Intellias.CQRS.Core.Commands;
using Intellias.CQRS.Core.Messages;

namespace Intellias.CQRS.Core.Tests.Fakes
{
    internal class InProcessCommandBus<T> : ICommandBus
        where T : ICommand,  new()
    {
        private readonly IMessageBus<IMessage, IExecutionResult> bus;

        public InProcessCommandBus(params ICommandHandler<T>[] handlers)
        {
            bus = new InProcessBus(handlers.Select(h => (IHandler<IMessage, IExecutionResult>)new HandlerWrapper(async msg => await h.HandleAsync((T)msg))).ToArray());
        }

        public async Task<ICommandResult> PublishAsync(ICommand msg)
        {
            var result = await bus.PublishAsync(msg);
            return await Task.FromResult((ICommandResult)result);
        }
    }
}

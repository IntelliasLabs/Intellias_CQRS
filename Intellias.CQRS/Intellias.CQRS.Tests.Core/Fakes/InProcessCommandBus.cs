using System.Linq;
using System.Threading.Tasks;
using Intellias.CQRS.Core.Commands;
using Intellias.CQRS.Core.Messages;

namespace Intellias.CQRS.Tests.Core.Fakes
{
    /// <inheritdoc />
    public class InProcessCommandBus<T> : ICommandBus
        where T : ICommand,  new()
    {
        private readonly IMessageBus<IMessage, IExecutionResult> bus;

        /// <summary>
        /// Creates command bus
        /// </summary>
        /// <param name="handlers">command handlers</param>
        public InProcessCommandBus(params ICommandHandler<T>[] handlers)
        {
            bus = new InProcessBus(handlers.Select(h => (IHandler<IMessage, IExecutionResult>)new HandlerWrapper(async msg => await h.HandleAsync((T)msg).ConfigureAwait(false))).ToArray());
        }

        /// <inheritdoc />
        public async Task<ICommandResult> PublishAsync(ICommand msg)
        {
            var result = await bus.PublishAsync(msg).ConfigureAwait(false);
            return await Task.FromResult((ICommandResult)result).ConfigureAwait(false);
        }
    }
}

using System.Threading.Tasks;
using Intellias.CQRS.Core.Commands;
using Intellias.CQRS.Core.Messages;

namespace Intellias.CQRS.Tests.Core.Fakes
{
    /// <inheritdoc />
    public class InProcessCommandBus : ICommandBus
    {
        private readonly InProcessBus bus;

        /// <summary>
        /// Creates command bus
        /// </summary>
        public InProcessCommandBus()
        {
            bus = new InProcessBus();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        public void AddHandler<T>(IHandler<T, ICommandResult> handler) where T : ICommand
        {
            bus.AddHandler(handler);
        }

        /// <inheritdoc />
        public async Task<ICommandResult> PublishAsync(ICommand msg)
        {
            var result = await bus.PublishAsync(msg);
            return await Task.FromResult((ICommandResult)result);
        }
    }
}

using System.Threading.Tasks;
using Intellias.CQRS.Core.Events;
using Intellias.CQRS.Core.Messages;

namespace Intellias.CQRS.Tests.Core.Fakes
{
    /// <inheritdoc />
    public class InProcessEventBus : IEventBus
    {
        private readonly InProcessBus bus;

        /// <summary>
        /// Creates event bus
        /// </summary>
        public InProcessEventBus()
        {
            bus = new InProcessBus();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        public void AddHandler<T>(IHandler<T, IEventResult> handler) where T : IEvent
        {
            bus.AddHandler(handler);
        }

        /// <inheritdoc />
        public async Task<IEventResult> PublishAsync(IEvent msg)
        {
            var result = await bus.PublishAsync(msg);
            return await Task.FromResult((IEventResult)result);
        }
    }
}

using Intellias.CQRS.Core.Messages;

namespace Intellias.CQRS.Core.Events
{
    /// <summary>
    /// Abstraction of Event Bus
    /// </summary>
    public interface IEventBus : IMessageBus<IEvent, IExecutionResult>
    {
    }
}

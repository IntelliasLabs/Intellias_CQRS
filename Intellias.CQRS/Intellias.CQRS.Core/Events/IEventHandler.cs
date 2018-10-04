using Intellias.CQRS.Core.Messages;

namespace Intellias.CQRS.Core.Events
{
    /// <inheritdoc />
    /// <summary>
    /// Event handler abstraction
    /// </summary>
    /// <typeparam name="T">Type of event</typeparam>
    public interface IEventHandler<in T> : IHandler<T, IEventResult> 
        where T : IEvent
    {
    }
}

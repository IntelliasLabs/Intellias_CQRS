using Intellias.CQRS.Core.Events;

namespace Intellias.CQRS.Core.Domain
{
    /// <summary>
    /// Defines a event applier for agreegate root.
    /// </summary>
    /// <typeparam name="T">Event type being applied.</typeparam>
    public interface IEventApplier<in T>
        where T : IEvent
    {
        /// <summary>
        /// Applies event for Agreegate root.
        /// </summary>
        /// <param name="event">Event instance.</param>
        void Apply(T @event);
    }
}

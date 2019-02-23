using Intellias.CQRS.Core.Messages;

namespace Intellias.CQRS.Core.Events
{
    /// <summary>
    /// Domain event interface
    /// </summary>
    public interface IEvent : IMessage
    {
        /// <summary>
        /// Gets the identifier of the source originating the event.
        /// </summary>
        string SourceId { get; }

        /// <summary>
        /// Version of AR that generated an event
        /// </summary>
        int Version { get; set; }
    }
}

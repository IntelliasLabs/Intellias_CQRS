using Intellias.CQRS.Core.Events;

namespace Intellias.CQRS.Core.Tests.Events
{
    /// <summary>
    /// Demo event class
    /// </summary>
    public class DemoCreatedEvent : Event
    {
        /// <summary>
        /// New demo name
        /// </summary>
        public string NewName { get; set; }
    }
}

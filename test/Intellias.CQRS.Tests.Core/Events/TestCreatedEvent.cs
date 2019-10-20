using Intellias.CQRS.Core.Events;

namespace Intellias.CQRS.Tests.Core.Events
{
    /// <summary>
    /// Test created event.
    /// </summary>
    public class TestCreatedEvent : Event
    {
        /// <summary>
        /// TestData.
        /// </summary>
        public string TestData { get; set; } = string.Empty;
    }
}

using Intellias.CQRS.Core.Events;

namespace Intellias.CQRS.Tests.Core.Events
{
    /// <inheritdoc />
    public class TestCreatedEvent : Event
    {
        /// <summary>
        /// TestData
        /// </summary>
        public string TestData { get; set; } = string.Empty;
    }
}

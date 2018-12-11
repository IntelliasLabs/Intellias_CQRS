using Intellias.CQRS.Core.Domain;
using Intellias.CQRS.Tests.Core.Events;

namespace Intellias.CQRS.Core.Tests.Domain
{
    /// <summary>
    /// Demo State
    /// </summary>
    public class DemoState : AggregateState,
        IEventApplier<TestCreatedEvent>,
        IEventApplier<TestUpdatedEvent>,
        IEventApplier<TestDeletedEvent>
    {
        /// <summary>
        /// TestData
        /// </summary>
        public string TestData { get; private set; }

        /// <summary>
        /// Demo State
        /// </summary>
        public DemoState()
        {
            Handles<TestCreatedEvent>(Apply);
            Handles<TestUpdatedEvent>(Apply);
            Handles<TestDeletedEvent>(Apply);
        }

        /// <inheritdoc />
        public void Apply(TestCreatedEvent @event)
        {
            TestData = @event.TestData;
        }

        /// <inheritdoc />
        public void Apply(TestUpdatedEvent @event)
        {
            TestData = @event.TestData;
        }

        /// <inheritdoc />
        public void Apply(TestDeletedEvent @event)
        {
        }
    }
}

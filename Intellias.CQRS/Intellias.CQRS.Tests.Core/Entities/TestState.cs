using Intellias.CQRS.Core.Domain;
using Intellias.CQRS.Tests.Core.Events;

namespace Intellias.CQRS.Tests.Core.Entities
{
    /// <summary>
    /// Test State
    /// </summary>
    public class TestState : AggregateState,
        IEventApplier<TestCreatedEvent>,
        IEventApplier<TestUpdatedEvent>,
        IEventApplier<TestDeletedEvent>
    {
        /// <summary>
        /// Test Data state
        /// </summary>
        public string TestData { private set; get; }

        /// <summary>
        /// 
        /// </summary>
        public TestState()
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
            // Not changing state
        }
    }
}

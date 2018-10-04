using Intellias.CQRS.Core.Domain;
using Intellias.CQRS.Tests.Core.Commands;
using Intellias.CQRS.Tests.Core.Events;

namespace Intellias.CQRS.Tests.Core.Entities
{
    /// <summary>
    /// Test aggregate root
    /// </summary>
    public class TestEntity : AggregateRoot
    {
        /// <summary>
        /// Do not allow to create objects without Id
        /// </summary>
        protected TestEntity() { }

        /// <summary>
        /// Main constructor of the class
        /// </summary>
        /// <param name="id"></param>
        public TestEntity(string id) : base(id)
        {
            Handles<TestCreatedEvent>(OnTestCreated);
            Handles<TestUpdatedEvent>(OnTestUpdated);
            Handles<TestDeletedEvent>(OnTestDeleted);
        }

        /// <summary>
        /// Creates an instance
        /// </summary>
        public void Create(TestCreateCommand command)
        {
            ApplyChange(new TestCreatedEvent
            {
                AggregateRootId = Id,
                TestData = command.TestData
            });
        }
        /// <summary>
        /// Updates the data
        /// </summary>
        /// <param name="command"></param>
        public void Update(TestUpdateCommand command)
        {
            ApplyChange(new TestUpdatedEvent
            {
                AggregateRootId = Id,
                TestData = command.TestData
            });
        }

        /// <summary>
        /// Deactivates an instance
        /// </summary>
        public void Deactivate()
        {
            ApplyChange(new TestDeletedEvent
            {
                AggregateRootId = Id
            });
        }




        private void OnTestCreated(TestCreatedEvent e)
        {
        }
        private void OnTestUpdated(TestUpdatedEvent e)
        {
        }
        private void OnTestDeleted(TestDeletedEvent e)
        {
        }
    }
}

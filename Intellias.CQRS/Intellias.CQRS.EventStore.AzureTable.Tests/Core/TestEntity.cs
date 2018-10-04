using Intellias.CQRS.Core.Domain;

namespace Intellias.CQRS.EventStore.AzureTable.Tests.Core
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

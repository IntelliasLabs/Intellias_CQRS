using Intellias.CQRS.Core.Domain;
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
        public void Create()
        {
            ApplyChange(new TestCreatedEvent
            {
                AggregateRootId = Id
            });
        }

        ///// <summary>
        ///// Updates the data
        ///// </summary>
        ///// <param name="command"></param>
        //public void Update(TestUpdateCommand command)
        //{
        //    ApplyChange(new TestUpdatedEvent(Id));
        //}

        ///// <summary>
        ///// Deactivates an instance
        ///// </summary>
        ///// <param name="command"></param>
        //public void Deactivate(TestDeleteCommand command)
        //{
        //    ApplyChange(new TestDeletedEvent(Id));
        //}




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

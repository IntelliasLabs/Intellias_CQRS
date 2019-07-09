using Intellias.CQRS.Core.Domain;
using Intellias.CQRS.Core.Results;
using Intellias.CQRS.Tests.Core.Commands;
using Intellias.CQRS.Tests.Core.Events;

namespace Intellias.CQRS.Tests.Core.Domain
{
    /// <inheritdoc cref="IAggregateRoot"/>
    public class TestRoot : AggregateRoot<TestState>
    {
        /// <summary>
        /// TestRoot
        /// </summary>
        public TestRoot(string id) : base(id)
        {
        }

        /// <summary>
        /// TestRoot
        /// </summary>
        public TestRoot(TestCreateCommand command) : base(command.AggregateRootId)
        {
            PublishEvent(new TestCreatedEvent
            {
                TestData = command.TestData
            });
        }

        /// <summary>
        /// Update
        /// </summary>
        /// <param name="command">TestUpdateCommand</param>
        public IExecutionResult Update(TestUpdateCommand command)
        {
            if (command.TestData.Length < 10)
            {
                return ValidationFailed("text too small");
            }

            PublishEvent(new TestUpdatedEvent
            {
                TestData = command.TestData,
            });

            return Success();
        }

        /// <summary>
        /// Deactivate
        /// </summary>
        public IExecutionResult Deactivate()
        {
            PublishEvent(new TestDeletedEvent());
            return Success();
        }
    }
}

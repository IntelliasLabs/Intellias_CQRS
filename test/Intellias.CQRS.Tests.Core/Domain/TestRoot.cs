using Intellias.CQRS.Core.Domain;
using Intellias.CQRS.Core.Results;
using Intellias.CQRS.Core.Results.Errors;
using Intellias.CQRS.Tests.Core.Commands;
using Intellias.CQRS.Tests.Core.Events;

namespace Intellias.CQRS.Tests.Core.Domain
{
    /// <inheritdoc cref="IAggregateRoot"/>
    public class TestRoot : AggregateRoot<TestState>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="TestRoot"/> class.
        /// </summary>
        /// <param name="id">Id of Aggeregate Root.</param>
        public TestRoot(string id)
            : base(id)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="TestRoot"/> class.
        /// </summary>
        /// <param name="command">Create command.</param>
        public TestRoot(TestCreateCommand command)
            : base(command.AggregateRootId)
        {
            PublishEvent(new TestCreatedEvent
            {
                TestData = command.TestData
            });
        }

        /// <summary>
        /// Update.
        /// </summary>
        /// <param name="command">TestUpdateCommand.</param>
        /// <returns>Execution Result.</returns>
        public IExecutionResult Update(TestUpdateCommand command)
        {
            if (command.TestData.Length < 10)
            {
                return ValidationFailedWithCode(CoreErrorCodes.ValidationFailed, "Text is too small.");
            }

            PublishEvent(new TestUpdatedEvent
            {
                TestData = command.TestData,
            });

            return Success();
        }

        /// <summary>
        /// Deactivate.
        /// </summary>
        /// <returns>Execution Result.</returns>
        public IExecutionResult Deactivate()
        {
            PublishEvent(new TestDeletedEvent());
            return Success();
        }
    }
}

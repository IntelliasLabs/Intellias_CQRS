using Intellias.CQRS.Core.Domain;
using Intellias.CQRS.Core.Messages;
using Intellias.CQRS.Tests.Core.Commands;
using Intellias.CQRS.Tests.Core.Events;

namespace Intellias.CQRS.Core.Tests.Domain
{
    /// <inheritdoc cref="AggregateRoot"/>
    public class DemoRoot : AggregateRoot,
        IEventApplier<TestCreatedEvent>,
        IEventApplier<TestUpdatedEvent>,
        IEventApplier<TestDeletedEvent>
    {
        /// <summary>
        /// TestData
        /// </summary>
        public string TestData { get; private set; }

        /// <summary>
        /// Creates demo root
        /// </summary>
        public DemoRoot()
        {
            Handles<TestCreatedEvent>(e => Apply(e));
            Handles<TestUpdatedEvent>(e => Apply(e));
            Handles<TestDeletedEvent>(e => Apply(e));
        }

        /// <summary>
        /// 
        /// </summary>
        public DemoRoot(TestCreateCommand command) : this()
        {
            Id = Unified.NewCode();
            PublishEvent(new TestCreatedEvent
            {
                TestData = command.TestData
            });
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="command"></param>
        public IExecutionResult Update(TestUpdateCommand command)
        {
            if (command.TestData.Length < 10)
            {
                return ExecutionResult.Fail("text too small");
            }

            PublishEvent(new TestUpdatedEvent
            {
                TestData = command.TestData,
            });

            return ExecutionResult.Success;
        }

        /// <summary>
        /// 
        /// </summary>
        public IExecutionResult Deactivate()
        {
            PublishEvent(new TestDeletedEvent());
            return ExecutionResult.Success;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="event"></param>
        /// <returns></returns>
        public void Apply(TestCreatedEvent @event)
        {
            TestData = @event.TestData;
            Id = @event.AggregateRootId;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="event"></param>
        /// <returns></returns>
        public void Apply(TestUpdatedEvent @event)
        {
            TestData = @event.TestData;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="event"></param>
        /// <returns></returns>
        public void Apply(TestDeletedEvent @event)
        {
        }
    }
}

using Intellias.CQRS.Core.Commands;
using Intellias.CQRS.Core.Domain;
using Intellias.CQRS.Core.Messages;
using Intellias.CQRS.Tests.Core.Commands;
using Intellias.CQRS.Tests.Core.Events;

namespace Intellias.CQRS.Core.Tests.Domain
{
    /// <inheritdoc />
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
            PublishEvent(new TestCreatedEvent
            {
                TestData = command.TestData,
                AggregateRootId = Unified.NewCode(),
                Version = 1
            });
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="command"></param>
        public ICommandResult Update(TestUpdateCommand command)
        {
            if (command.TestData.Length < 10)
            {
                return CommandResult.Fail("text too small");
            }

            PublishEvent(new TestUpdatedEvent
            {
                TestData = command.TestData,
            });

            return CommandResult.Success;
        }

        /// <summary>
        /// 
        /// </summary>
        public ICommandResult Deactivate()
        {
            PublishEvent(new TestDeletedEvent());
            return CommandResult.Success;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="event"></param>
        /// <returns></returns>
        public void Apply(TestCreatedEvent @event)
        {
            this.TestData = @event.TestData;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="event"></param>
        /// <returns></returns>
        public void Apply(TestUpdatedEvent @event)
        {
            this.TestData = @event.TestData;
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

using System.Threading.Tasks;
using Intellias.CQRS.Core.Commands;
using Intellias.CQRS.Core.Domain;
using Intellias.CQRS.Core.Events;
using Intellias.CQRS.Core.Messages;
using Intellias.CQRS.Tests.Core.Commands;
using Intellias.CQRS.Tests.Core.Events;

namespace Intellias.CQRS.Core.Tests.Domain
{
    /// <inheritdoc />
    public class DemoRoot : AggregateRoot,
        IEventHandler<TestCreatedEvent>,
        IEventHandler<TestUpdatedEvent>,
        IEventHandler<TestDeletedEvent>
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
            Handles<TestCreatedEvent>(async x => await HandleAsync(x));
            Handles<TestUpdatedEvent>(async x => await HandleAsync(x));
            Handles<TestDeletedEvent>(async x => await HandleAsync(x));
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
        /// <param name="message"></param>
        /// <returns></returns>
        public Task<IEventResult> HandleAsync(TestCreatedEvent message)
        {
            this.TestData = message.TestData;
            return Task.FromResult((IEventResult)EventResult.Success);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="message"></param>
        /// <returns></returns>
        public Task<IEventResult> HandleAsync(TestUpdatedEvent message)
        {
            this.TestData = message.TestData;
            return Task.FromResult((IEventResult)EventResult.Success);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="message"></param>
        /// <returns></returns>
        public Task<IEventResult> HandleAsync(TestDeletedEvent message)
        {
            return Task.FromResult((IEventResult)EventResult.Success);
        }
    }
}

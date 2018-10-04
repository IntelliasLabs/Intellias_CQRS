using Intellias.CQRS.Core.Domain;
using Intellias.CQRS.Core.Messages;
using Intellias.CQRS.Tests.Core.Commands;
using Intellias.CQRS.Tests.Core.Events;

namespace Intellias.CQRS.Core.Tests.Domain
{
    /// <inheritdoc />
    public class DemoRoot : AggregateRoot
    {
        /// <summary>
        /// Creates demo root
        /// </summary>
        public DemoRoot()
        {
        }

        /// <summary>
        /// 
        /// </summary>
        public DemoRoot(TestCreateCommand command)
        {
            ApplyChange(new TestCreatedEvent
            {
                TestData = command.TestData,
                AggregateRootId = Unified.NewCode(),
                Version = 1
            });
        }
    }
}

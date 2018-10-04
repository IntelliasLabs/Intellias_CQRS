using Intellias.CQRS.Core.Domain;
using Intellias.CQRS.Core.Messages;
using Intellias.CQRS.Core.Tests.Commands;
using Intellias.CQRS.Core.Tests.Events;

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
        public DemoRoot(DemoCreateCommand command)
        {
            ApplyChange(new DemoCreatedEvent
            {
                NewName = command.Name,
                AggregateRootId = Unified.NewCode(),
                Version = 1
            });
        }
    }
}

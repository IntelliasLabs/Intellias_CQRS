using Intellias.CQRS.Core.Commands;

namespace Intellias.CQRS.Tests.Core.Fakes
{
    public class InProcessCommandBus<TCommandBusOptions> : InProcessCommandBus, ICommandBus<TCommandBusOptions>
        where TCommandBusOptions : ICommandBusOptions
    {
    }
}
using Intellias.CQRS.Core.Messages;

namespace Intellias.CQRS.Core.Commands
{
    /// <summary>
    /// Publishes commands.
    /// </summary>
    /// <typeparam name="TCommandBusOptions">Command bus options type.</typeparam>
    public interface ICommandBus<TCommandBusOptions> : IMessageBus<ICommand>
        where TCommandBusOptions : ICommandBusOptions
    {
    }
}
using Intellias.CQRS.Core.Messages;

namespace Intellias.CQRS.Core.Commands
{
    /// <summary>
    /// Publishes commands.
    /// </summary>
    /// <typeparam name="TCommandBusOptions">Command bus options type.</typeparam>
    // ReSharper disable once UnusedTypeParameter, parameter is used to have multiple instances of Command Bus with the different options.
    public interface ICommandBus<TCommandBusOptions> : IMessageBus<ICommand>
        where TCommandBusOptions : ICommandBusOptions
    {
    }
}
using Intellias.CQRS.Core.Messages;

namespace Intellias.CQRS.Core.Commands
{
    /// <summary>
    /// Command bus.
    /// </summary>
    public interface ICommandBus : IMessageBus<ICommand>
    {
    }
}

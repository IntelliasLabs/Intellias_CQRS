using Intellias.CQRS.Core.Messages;

namespace Intellias.CQRS.Core.Commands
{
    /// <summary>
    /// Command handler abstraction
    /// </summary>
    /// <typeparam name="T">Type of command</typeparam>
    public interface ICommandHandler<T> : IHandler<T, ICommandResult>
        where T : ICommand
    {}
}

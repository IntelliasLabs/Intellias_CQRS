using System.Threading.Tasks;
using Intellias.CQRS.Core.Messages;

namespace Intellias.CQRS.Core.Commands
{
    /// <summary>
    /// Command handler abstraction
    /// </summary>
    /// <typeparam name="T">Type of command</typeparam>
    public interface ICommandHandler<T> : IHandler<T, ICommandResult>
        where T : ICommand
    {
        /// <summary>
        /// Handle message.
        /// </summary>
        /// <param name="message">abstract message.</param>
        /// <returns>async task awaiter.</returns>
        new Task<ICommandResult> Handle(T message);
    }
}

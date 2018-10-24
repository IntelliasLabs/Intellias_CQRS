using System.Threading.Tasks;

namespace Intellias.CQRS.Core.Commands
{
    /// <inheritdoc />
    /// <summary>
    /// Command handler abstraction
    /// </summary>
    /// <typeparam name="T">Type of command</typeparam>
    public interface ICommandHandler<in T> where T : ICommand
    {
        /// <summary>
        ///  Handles a command
        /// </summary>
        /// <param name="command">Command being handled</param>
        /// <returns>Task that represents handling of message</returns>
        Task<ICommandResult> HandleAsync(T command);
    }
}

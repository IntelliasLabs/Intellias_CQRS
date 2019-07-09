using System.Threading.Tasks;
using Intellias.CQRS.Core.Results;

namespace Intellias.CQRS.Core.Commands
{
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
        Task<IExecutionResult> HandleAsync(T command);
    }
}

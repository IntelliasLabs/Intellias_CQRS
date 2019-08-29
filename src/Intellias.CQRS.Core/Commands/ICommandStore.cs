using System.Threading.Tasks;

namespace Intellias.CQRS.Core.Commands
{
    /// <summary>
    /// Command storage abstraction.
    /// </summary>
    public interface ICommandStore
    {
        /// <summary>
        /// Store command.
        /// </summary>
        /// <param name="command">Command.</param>
        /// <returns>Awaiter.</returns>
        Task SaveAsync(ICommand command);
    }
}

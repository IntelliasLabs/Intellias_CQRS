using System.Threading.Tasks;

namespace Intellias.CQRS.Core.Commands
{
    /// <summary>
    /// Command bus
    /// </summary>
    public interface ICommandBus
    {
        /// <summary>
        /// Send a command to command bus
        /// </summary>
        /// <typeparam name="T">Type of command</typeparam>
        /// <param name="command">Command instance</param>
        /// <returns>Result of command</returns>
        Task<CommandResult> SendAsync<T>(T command) where T : ICommand;
    }
}

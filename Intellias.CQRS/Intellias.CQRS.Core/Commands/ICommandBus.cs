using System.Threading.Tasks;

namespace Product.Domain.Core.Commands
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
        Task<CommandResult> Send<T>(T command) where T : ICommand;
    }
}

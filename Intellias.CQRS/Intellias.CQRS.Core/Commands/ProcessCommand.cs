using Intellias.CQRS.Core.Processes;

namespace Intellias.CQRS.Core.Commands
{
    /// <summary>
    /// Command used to keep process state
    /// </summary>
    public abstract class ProcessCommand : Command
    {
        /// <summary>
        /// Process-manager's state
        /// </summary>
        public IProcess Process { get; set; }
    }
}

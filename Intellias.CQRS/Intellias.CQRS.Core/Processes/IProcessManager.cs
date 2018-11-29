using System.Collections.Generic;
using Intellias.CQRS.Core.Commands;

namespace Intellias.CQRS.Core.Processes
{
    /// <summary>
    /// IProcessManager
    /// </summary>
    public interface IProcessManager
    {
        /// <summary>
        /// Gets the process manager identifier.
        /// </summary>
        string Id { get; }

        /// <summary>
        /// Gets a value indicating whether the process manager workflow is completed and the state can be archived.
        /// </summary>
        bool Completed { get; }

        /// <summary>
        /// Gets a collection of commands that need to be sent when the state of the process manager is persisted.
        /// </summary>
        IEnumerable<ICommand> Commands { get; }
    }
}

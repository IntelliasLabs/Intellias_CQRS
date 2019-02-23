using System.Collections.Generic;
using Intellias.CQRS.Core.Commands;
using Intellias.CQRS.Core.Events;

namespace Intellias.CQRS.Core.Processes
{
    /// <summary>
    /// IProcess
    /// </summary>
    public interface IProcess
    {
        /// <summary>
        /// Gets the process manager identifier.
        /// </summary>
        string OperationId { get; }

        /// <summary>
        /// Gets a value indicating whether the process manager workflow is completed and the state can be archived.
        /// </summary>
        bool Completed { get; }

        /// <summary>
        /// Gets a collection of commands that need to be sent when the state of the process manager is persisted.
        /// </summary>
        IEnumerable<ICommand> Commands { get; }

        /// <summary>
        /// Gets a collection of events that need to be sent when the state of the process manager is persisted.
        /// </summary>
        IEnumerable<IEvent> Events { get; }

        /// <summary>
        /// Gets a collection of events that need to be sent to outside world.
        /// </summary>
        IEnumerable<IEvent> Reports { get; }
    }
}

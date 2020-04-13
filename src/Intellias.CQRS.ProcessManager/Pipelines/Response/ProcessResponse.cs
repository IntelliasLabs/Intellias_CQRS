using System;
using System.Collections.Generic;

namespace Intellias.CQRS.ProcessManager.Pipelines.Response
{
    /// <summary>
    /// Process response.
    /// </summary>
    public class ProcessResponse
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ProcessResponse"/> class.
        /// </summary>
        /// <param name="id">Id.</param>
        /// <param name="processMessages">Process messages.</param>
        /// <param name="isPersisted">Is persisted.</param>
        public ProcessResponse(string id, IReadOnlyCollection<ProcessMessage> processMessages = null, bool isPersisted = false)
        {
            Id = id;
            ProcessMessages = processMessages ?? Array.Empty<ProcessMessage>();
            IsPersisted = isPersisted;
        }

        /// <summary>
        /// Id.
        /// </summary>
        public string Id { get; }

        /// <summary>
        /// Process messages.
        /// </summary>
        public IReadOnlyCollection<ProcessMessage> ProcessMessages { get; }

        /// <summary>
        /// Is persisted.
        /// </summary>
        public bool IsPersisted { get; }
    }
}
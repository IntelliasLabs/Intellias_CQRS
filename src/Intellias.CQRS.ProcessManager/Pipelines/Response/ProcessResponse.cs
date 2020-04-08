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
        public ProcessResponse(string id, IReadOnlyCollection<ProcessMessage> processMessages = null)
        {
            Id = id;
            ProcessMessages = processMessages ?? Array.Empty<ProcessMessage>();
        }

        /// <summary>
        /// Id.
        /// </summary>
        public string Id { get; }

        /// <summary>
        /// Process messages.
        /// </summary>
        public IReadOnlyCollection<ProcessMessage> ProcessMessages { get; }
    }
}
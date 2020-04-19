using System;
using Intellias.CQRS.Core.Domain;

namespace Intellias.CQRS.ProcessManager.Pipelines.Requests
{
    /// <summary>
    /// Process request.
    /// </summary>
    /// <typeparam name="TState">State request.</typeparam>
    public class ProcessRequest<TState>
        where TState : class
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ProcessRequest{TState}"/> class.
        /// </summary>
        /// <param name="queryModel">Query model.</param>
        /// <param name="getId">Get query model snapshot id.</param>
        public ProcessRequest(TState queryModel, Func<TState, SnapshotId> getId)
        {
            var snapshotId = getId(queryModel);
            Id = $"{snapshotId.EntryId}-{snapshotId.EntryVersion}";

            State = queryModel;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ProcessRequest{TState}"/> class.
        /// </summary>
        /// <param name="state">Input state.</param>
        /// <param name="getId">Get query model snapshot id.</param>
        public ProcessRequest(TState state, Func<TState, string> getId)
        {
            Id = getId(state);
            State = state;
        }

        /// <summary>
        /// Request id.
        /// </summary>
        public string Id { get; }

        /// <summary>
        /// State.
        /// </summary>
        public TState State { get; }
    }
}
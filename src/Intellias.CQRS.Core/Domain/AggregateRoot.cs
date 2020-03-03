using System;
using System.Collections.Generic;
using System.Linq;
using Intellias.CQRS.Core.Events;
using Intellias.CQRS.Core.Results;
using Intellias.CQRS.Core.Results.Errors;

namespace Intellias.CQRS.Core.Domain
{
    /// <inheritdoc />
    public class AggregateRoot<T> : IAggregateRoot
        where T : AggregateState, new()
    {
        private readonly List<IEvent> pendingEvents = new List<IEvent>();

        /// <summary>
        /// Initializes a new instance of the <see cref="AggregateRoot{T}"/> class.
        /// </summary>
        /// <param name="id">Id of Aggregate Root.</param>
        protected AggregateRoot(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                throw new ArgumentNullException(nameof(id));
            }

            Id = id;
        }

        /// <summary>
        /// Current State of Aggregate.
        /// </summary>
        public T State { get; } = new T();

        /// <inheritdoc />
        public string Id { get; protected set; }

        /// <inheritdoc />
        public IReadOnlyCollection<IEvent> Events => pendingEvents.AsReadOnly();

        /// <inheritdoc />
        public int Version => State.Version;

        /// <inheritdoc />
        public void LoadFromHistory(IEnumerable<IEvent> history)
        {
            foreach (var e in history.OrderBy(e => e.Version))
            {
                // Event should always equal next state version
                if (e.Version != State.Version + 1)
                {
                    throw new DataMisalignedException($"Misaligned event version {e.Version} of {e.AggregateRootId} aggregate");
                }

                State.ApplyEvent(e);
            }
        }

        /// <summary>
        /// Successful result.
        /// </summary>
        /// <returns>Execution Result.</returns>
        public IExecutionResult Success()
        {
            return new SuccessfulResult();
        }

        /// <summary>
        /// Acess Denied helper.
        /// </summary>
        /// <param name="internalCodeInfo">Error code info.</param>
        /// <returns>Failed Result.</returns>
        protected static FailedResult AccessDenied(ErrorCodeInfo internalCodeInfo)
        {
            return FailedResult.Create(CoreErrorCodes.AccessDenied, internalCodeInfo);
        }

        /// <summary>
        /// Validation Failed helper.
        /// </summary>
        /// <param name="internalCodeInfo">Error code info.</param>
        /// <param name="customMessage">Custom error message..</param>
        /// <returns>Failed Result.</returns>
        protected static FailedResult ValidationFailed(ErrorCodeInfo internalCodeInfo, string customMessage)
        {
            return FailedResult.Create(CoreErrorCodes.ValidationFailed, internalCodeInfo, customMessage);
        }

        /// <summary>
        /// Validation Failed helper.
        /// </summary>
        /// <param name="internalCodeInfo">Error code info.</param>
        /// <returns>Failed Result.</returns>
        protected static FailedResult ValidationFailed(ErrorCodeInfo internalCodeInfo)
        {
            return FailedResult.Create(CoreErrorCodes.ValidationFailed, internalCodeInfo);
        }

        /// <summary>
        /// Validation Failed helper with internal execution errors.
        /// </summary>
        /// <param name="internalErrors">Internal validation errors.</param>
        /// <returns>Failed Result.</returns>
        protected static FailedResult ValidationFailed(IReadOnlyCollection<ExecutionError> internalErrors)
        {
            return FailedResult.ValidationFailed(internalErrors);
        }

        /// <summary>
        /// Apply an event.
        /// </summary>
        /// <param name="event">Event.</param>
        protected void PublishEvent(IEvent @event)
        {
            if (@event == null)
            {
                throw new ArgumentNullException(nameof(@event));
            }

            if (string.IsNullOrWhiteSpace(@event.AggregateRootId))
            {
                throw new NullReferenceException($"{nameof(@event.AggregateRootId)} is null or empty.");
            }

            State.ApplyEvent(@event);
            pendingEvents.Add(@event);
        }
    }
}

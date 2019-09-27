using System;
using System.Collections.Generic;
using System.Linq;
using Intellias.CQRS.Core.Domain;
using Intellias.CQRS.Core.Events;
using Intellias.CQRS.Core.Results;
using Intellias.CQRS.Pipelines.CommandHandlers.Behaviors;

namespace Intellias.CQRS.Pipelines.CommandHandlers
{
    /// <summary>
    /// Base class for command handlers.
    /// </summary>
    public abstract class BaseCommandHandler
    {
        /// <summary>
        /// Creates validation failed result.
        /// </summary>
        /// <param name="errorMessage">Error message.</param>
        /// <returns>Execution result.</returns>
        protected IExecutionResult ValidationFailed(string errorMessage)
        {
            return new FailedResult(ErrorCodes.ValidationFailed, errorMessage);
        }

        /// <summary>
        /// Creates validation failed result.
        /// </summary>
        /// <param name="source">Source of the error.</param>
        /// <param name="errorMessage">Error message.</param>
        /// <returns>Execution result.</returns>
        protected IExecutionResult ValidationFailed(string source, string errorMessage)
        {
            return new FailedResult(ErrorCodes.ValidationFailed, source, errorMessage);
        }

        /// <summary>
        /// Creates validation failed result.
        /// </summary>
        /// <param name="errors">Validation errors.</param>
        /// <returns>Execution result.</returns>
        protected IExecutionResult ValidationFailed(IReadOnlyCollection<ExecutionError> errors)
        {
            // First error contains the most generic description on issue.
            var topError = errors.First();
            return new FailedResult(topError.Code, topError.Source, topError.Message);
        }

        /// <summary>
        /// Creates aggregate not found result.
        /// </summary>
        /// <param name="aggregateId">Aggregate id.</param>
        /// <typeparam name="TAggregateRoot">Type of the aggregate.</typeparam>
        /// <returns>Execution result.</returns>
        protected IExecutionResult AggregateNotFound<TAggregateRoot>(string aggregateId)
            where TAggregateRoot : IAggregateRoot
        {
            return new FailedResult($"Aggregate of type '{typeof(TAggregateRoot)}' with id '{aggregateId}' is not found.");
        }

        /// <summary>
        /// Creates aggregate not found result.
        /// </summary>
        /// <param name="aggregateId">Aggregate id.</param>
        /// <param name="version">Aggregate version.</param>
        /// <typeparam name="TAggregateRoot">Type of the aggregate.</typeparam>
        /// <returns>Execution result.</returns>
        protected IExecutionResult AggregateNotFound<TAggregateRoot>(string aggregateId, int version)
            where TAggregateRoot : IAggregateRoot
        {
            return new FailedResult($"Aggregate of type '{typeof(TAggregateRoot)}' with id '{aggregateId}' and version '{version}' is not found.");
        }

        /// <summary>
        /// Creates <see cref="IntegrationEventExecutionResult"/>.
        /// </summary>
        /// <param name="context">Execution context.</param>
        /// <param name="setup">Configures integration event.</param>
        /// <typeparam name="TIntegrationEvent">Type of integration event.</typeparam>
        /// <returns>Execution result.</returns>
        protected IExecutionResult IntegrationEvent<TIntegrationEvent>(AggregateExecutionContext context, Action<TIntegrationEvent> setup)
            where TIntegrationEvent : IntegrationEvent, new()
        {
            var @event = context.CreateIntegrationEvent<TIntegrationEvent>();

            setup(@event);

            return new IntegrationEventExecutionResult(@event);
        }
    }
}
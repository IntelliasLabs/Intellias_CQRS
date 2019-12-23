using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using Intellias.CQRS.Core.Commands;
using Intellias.CQRS.Core.Domain;
using Intellias.CQRS.Core.Events;
using Intellias.CQRS.Core.Results;
using Intellias.CQRS.Pipelines.CommandHandlers.Behaviors;

namespace Intellias.CQRS.Tests.Core.Pipelines.CommandHandlers
{
    /// <summary>
    /// Base test flow execution context.
    /// </summary>
    public abstract class TestFlowExecutionContextBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="TestFlowExecutionContextBase"/> class.
        /// </summary>
        /// <param name="host">Value for <see cref="Host"/>.</param>
        protected TestFlowExecutionContextBase(SubdomainTestHost host)
        {
            Host = host;
        }

        protected TestFlowExecutionContextBase(
            SubdomainTestHost host,
            ImmutableDictionary<string, int> aggregateVersions,
            ImmutableList<Command> commands,
            ImmutableList<IntegrationEvent> expectedEvents,
            ImmutableList<IExecutionResult> results)
        {
            Host = host;
            AggregateVersions = AggregateVersions.AddRange(aggregateVersions);
            ExecutedCommands = ExecutedCommands.AddRange(commands);
            ExpectedEvents = ExpectedEvents.AddRange(expectedEvents);
            ExecutionResults = ExecutionResults.AddRange(results);
        }

        /// <summary>
        /// Subdomain test host.
        /// </summary>
        public SubdomainTestHost Host { get; }

        /// <summary>
        /// Aggregates versions.
        /// </summary>
        public ImmutableDictionary<string, int> AggregateVersions { get; private set; } = ImmutableDictionary<string, int>.Empty;

        /// <summary>
        /// Commands executed during running test flow.
        /// </summary>
        public ImmutableList<Command> ExecutedCommands { get; } = ImmutableList<Command>.Empty;

        /// <summary>
        /// Expected <see cref="IIntegrationEvent"/> fired as result of running commands.
        /// </summary>
        public ImmutableList<IntegrationEvent> ExpectedEvents { get; } = ImmutableList<IntegrationEvent>.Empty;

        /// <summary>
        /// Execution results of the commands.
        /// </summary>
        public ImmutableList<IExecutionResult> ExecutionResults { get; } = ImmutableList<IExecutionResult>.Empty;

        /// <summary>
        /// Returns all <see cref="ExpectedEvents"/> of specified type.
        /// </summary>
        /// <typeparam name="TIntegrationEvent">Type of the integration event.</typeparam>
        /// <returns>Found integration events.</returns>
        public IEnumerable<TIntegrationEvent> GetIntegrationEvents<TIntegrationEvent>()
        {
            return ExecutionResults.OfType<IntegrationEventExecutionResult>().Select(e => e.Event).OfType<TIntegrationEvent>();
        }

        /// <summary>
        /// Updates aggregate version for aggregate with <see cref="aggregateId"/>.
        /// </summary>
        /// <param name="aggregateId">Aggregate id.</param>
        /// <param name="aggregateVersionIncrement">Number to increase aggregate version.</param>
        /// <returns>Aggregate snapshot having current aggregate version.</returns>
        public SnapshotId UpdateAggregateVersion(string aggregateId, int aggregateVersionIncrement)
        {
            var currentVersion = AggregateVersions.TryGetValue(aggregateId, out var aggregateVersion)
                ? aggregateVersion + aggregateVersionIncrement
                : aggregateVersionIncrement;

            AggregateVersions = AggregateVersions.SetItem(aggregateId, currentVersion);

            return new SnapshotId(aggregateId, currentVersion);
        }
    }
}
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
    /// Test flow execution context.
    /// </summary>
    public class TestFlowExecutionContext
    {
        private readonly Dictionary<string, int> aggregateVersions = new Dictionary<string, int>();

        /// <summary>
        /// Initializes a new instance of the <see cref="TestFlowExecutionContext"/> class.
        /// </summary>
        /// <param name="host">Value for <see cref="Host"/>.</param>
        public TestFlowExecutionContext(SubdomainTestHost host)
        {
            Host = host;
        }

        private TestFlowExecutionContext(
            SubdomainTestHost host,
            Dictionary<string, int> aggregateVersions,
            ImmutableList<Command> commands,
            ImmutableList<IntegrationEvent> expectedEvents,
            ImmutableList<IExecutionResult> results)
        {
            Host = host;
            this.aggregateVersions = aggregateVersions;
            ExecutedCommands = ExecutedCommands.AddRange(commands);
            ExpectedEvents = ExpectedEvents.AddRange(expectedEvents);
            ExecutionResults = ExecutionResults.AddRange(results);
        }

        /// <summary>
        /// Subdomain test host.
        /// </summary>
        public SubdomainTestHost Host { get; }

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
        /// Deconstructs context.
        /// </summary>
        /// <param name="executionResult">Last execution result.</param>
        /// <param name="executionContext">Current execution context.</param>
        public void Deconstruct(out IExecutionResult executionResult, out TestFlowExecutionContext executionContext)
        {
            executionResult = ExecutionResults.LastOrDefault();
            executionContext = this;
        }

        /// <summary>
        /// Adds results of command execution to context.
        /// </summary>
        /// <param name="command">Executed command.</param>
        /// <param name="integrationEvent">Expected event.</param>
        /// <param name="result">Execution result.</param>
        /// <returns>Updated execution context.</returns>
        public TestFlowExecutionContext Update(Command command, IntegrationEvent integrationEvent, IExecutionResult result)
        {
            return new TestFlowExecutionContext(
                Host,
                aggregateVersions,
                ExecutedCommands.Add(command),
                ExpectedEvents.Add(integrationEvent),
                ExecutionResults.Add(result));
        }

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
            var currentVersion = aggregateVersions.TryGetValue(aggregateId, out var aggregateVersion)
                ? aggregateVersion + aggregateVersionIncrement
                : aggregateVersionIncrement;

            aggregateVersions[aggregateId] = currentVersion;

            return new SnapshotId(aggregateId, currentVersion);
        }
    }
}
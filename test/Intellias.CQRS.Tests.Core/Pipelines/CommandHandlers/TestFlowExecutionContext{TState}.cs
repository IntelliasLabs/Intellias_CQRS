using System.Collections.Immutable;
using System.Linq;
using Intellias.CQRS.Core.Commands;
using Intellias.CQRS.Core.Events;
using Intellias.CQRS.Core.Results;

namespace Intellias.CQRS.Tests.Core.Pipelines.CommandHandlers
{
    /// <summary>
    /// State based test flow execution context.
    /// </summary>
    /// <typeparam name="TState">Test flow execution state.</typeparam>
    public class TestFlowExecutionContext<TState> : TestFlowExecutionContextBase
        where TState : class, new()
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="TestFlowExecutionContext{TState}"/> class.
        /// </summary>
        /// <param name="host">Value for <see cref="TestFlowExecutionContextBase.Host"/>.</param>
        public TestFlowExecutionContext(SubdomainTestHost host)
            : base(host)
        {
            State = new TState();
        }

        private TestFlowExecutionContext(
            SubdomainTestHost host,
            TState state,
            ImmutableDictionary<string, int> aggregateVersions,
            ImmutableList<Command> commands,
            ImmutableList<IntegrationEvent> expectedEvents,
            ImmutableList<IExecutionResult> results)
            : base(host, aggregateVersions, commands, expectedEvents, results)
        {
            State = state;
        }

        /// <summary>
        /// Test flow execution state.
        /// </summary>
        public TState State { get; }

        /// <summary>
        /// Deconstructs context.
        /// </summary>
        /// <param name="executionResult">Last execution result.</param>
        /// <param name="executionContext">Current execution context.</param>
        public void Deconstruct(out IExecutionResult executionResult, out TestFlowExecutionContext<TState> executionContext)
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
        public TestFlowExecutionContext<TState> Update(Command command, IntegrationEvent integrationEvent, IExecutionResult result)
        {
            return new TestFlowExecutionContext<TState>(
                Host,
                State,
                AggregateVersions,
                ExecutedCommands.Add(command),
                ExpectedEvents.Add(integrationEvent),
                ExecutionResults.Add(result));
        }
    }
}
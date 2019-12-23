using System.Collections.Generic;
using System.Threading.Tasks;

namespace Intellias.CQRS.Tests.Core.Pipelines.CommandHandlers
{
    public class SubdomainTestFlow<TState>
        where TState : class, new()
    {
        private readonly SubdomainTestHost host;
        private readonly List<ITestFlowStep<TState>> steps = new List<ITestFlowStep<TState>>();

        /// <summary>
        /// Initializes a new instance of the <see cref="SubdomainTestFlow{TState}"/> class.
        /// </summary>
        /// <param name="host">Subdomain host.</param>
        public SubdomainTestFlow(SubdomainTestHost host)
        {
            this.host = host;
        }

        /// <summary>
        /// Adds step to test flow.
        /// </summary>
        /// <param name="step">Test step.</param>
        /// <returns>Test flow with added step.</returns>
        public SubdomainTestFlow<TState> With(ITestFlowStep<TState> step)
        {
            steps.Add(step);
            return this;
        }

        /// <summary>
        /// Runs test flow.
        /// </summary>
        /// <returns>Execution context of the run flow.</returns>
        public async Task<TestFlowExecutionContext<TState>> RunAsync()
        {
            var context = new TestFlowExecutionContext<TState>(host);

            foreach (var step in steps)
            {
                var stepResult = step.Execute(context);
                if (!(stepResult is CommandStepResult commandResult))
                {
                    continue;
                }

                var (command, integrationEvent) = commandResult;
                var result = await host.SendAsync(command);

                context = context.Update(command, integrationEvent, result);

                if (!result.Success)
                {
                    return context;
                }
            }

            return context;
        }
    }
}
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Intellias.CQRS.Tests.Core.Pipelines.CommandHandlers
{
    /// <summary>
    /// Subdomain test flow.
    /// </summary>
    public class SubdomainTestFlow
    {
        private readonly SubdomainTestHost host;
        private readonly List<ITestFlowStep> steps = new List<ITestFlowStep>();

        /// <summary>
        /// Initializes a new instance of the <see cref="SubdomainTestFlow"/> class.
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
        public SubdomainTestFlow With(ITestFlowStep step)
        {
            steps.Add(step);
            return this;
        }

        /// <summary>
        /// Runs test flow.
        /// </summary>
        /// <returns>Execution context of the run flow.</returns>
        public async Task<TestFlowExecutionContext> RunAsync()
        {
            var context = new TestFlowExecutionContext(host);

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
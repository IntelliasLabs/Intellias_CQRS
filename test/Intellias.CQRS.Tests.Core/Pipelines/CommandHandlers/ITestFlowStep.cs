namespace Intellias.CQRS.Tests.Core.Pipelines.CommandHandlers
{
    /// <summary>
    /// Test flow step.
    /// </summary>
    public interface ITestFlowStep
    {
        /// <summary>
        /// Executes test flow step.
        /// </summary>
        /// <param name="context">Test flow execution context.</param>
        /// <returns>Result of the test flow step execution.</returns>
        ITestFlowStepResult Execute(TestFlowExecutionContext context);
    }
}
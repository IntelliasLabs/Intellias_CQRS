namespace Intellias.CQRS.Tests.Core.Pipelines.CommandHandlers
{
    /// <summary>
    /// Test flow step.
    /// </summary>
    /// <typeparam name="TState">Test flow state.</typeparam>
    public interface ITestFlowStep<TState>
        where TState : class, new()
    {
        /// <summary>
        /// Executes test flow step.
        /// </summary>
        /// <param name="context">Test flow execution context.</param>
        /// <returns>Result of the test flow step execution.</returns>
        ITestFlowStepResult Execute(TestFlowExecutionContext<TState> context);
    }
}
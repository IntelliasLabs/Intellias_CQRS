using Intellias.CQRS.Core.Commands;
using Intellias.CQRS.Core.Events;

namespace Intellias.CQRS.Tests.Core.Pipelines.CommandHandlers
{
    /// <summary>
    /// Test flow result of command execution.
    /// </summary>
    public class CommandStepResult : ITestFlowStepResult
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="CommandStepResult"/> class.
        /// </summary>
        /// <param name="command">Value for <see cref="Command"/>.</param>
        /// <param name="expectedEvent">Value for <see cref="IntegrationEvent"/>.</param>
        public CommandStepResult(Command command, IntegrationEvent expectedEvent)
        {
            Command = command;
            ExpectedEvent = expectedEvent;
        }

        /// <summary>
        /// Executed command.
        /// </summary>
        public Command Command { get; }

        /// <summary>
        /// Expected integration event.
        /// </summary>
        public IntegrationEvent ExpectedEvent { get; }

        /// <summary>
        /// Deconstructs result.
        /// </summary>
        /// <param name="command">Value of <see cref="Command"/>.</param>
        /// <param name="expectedEvent">Value of <see cref="IntegrationEvent"/>.</param>
        public void Deconstruct(out Command command, out IntegrationEvent expectedEvent)
        {
            command = Command;
            expectedEvent = ExpectedEvent;
        }
    }
}
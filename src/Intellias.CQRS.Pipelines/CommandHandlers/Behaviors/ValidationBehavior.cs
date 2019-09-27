using System.Threading;
using System.Threading.Tasks;
using Intellias.CQRS.Core.Commands;
using Intellias.CQRS.Core.Results;
using MediatR;

namespace Intellias.CQRS.Pipelines.CommandHandlers.Behaviors
{
    /// <summary>
    /// Validates incoming command.
    /// </summary>
    /// <typeparam name="TCommand">Type of the command to be handled.</typeparam>
    public class ValidationBehavior<TCommand> : IPipelineBehavior<CommandRequest<TCommand>, IExecutionResult>
        where TCommand : Command
    {
        /// <inheritdoc />
        public Task<IExecutionResult> Handle(
            CommandRequest<TCommand> request,
            CancellationToken cancellationToken,
            RequestHandlerDelegate<IExecutionResult> next)
        {
            var result = request.Command.Validate();

            return result is SuccessfulResult
                ? next()
                : Task.FromResult(result);
        }
    }
}
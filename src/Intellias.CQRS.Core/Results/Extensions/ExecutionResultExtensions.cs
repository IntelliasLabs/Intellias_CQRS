using System;
using System.Linq;
using System.Linq.Expressions;
using Intellias.CQRS.Core.Commands;
using Intellias.CQRS.Core.Results.Errors;

namespace Intellias.CQRS.Core.Results.Extensions
{
    /// <summary>
    /// Extension for <see cref="FailedResult"/>.
    /// </summary>
    public static class ExecutionResultExtensions
    {
        /// <summary>
        /// Extension that fills sources for general error + internal error if exist.
        /// </summary>
        /// <typeparam name="TCommand">Type of command.</typeparam>
        /// <param name="executionResult">Failed Execution Result.</param>
        /// <param name="getProprty">Get property expression.</param>
        /// <returns>Filed FailedResult with correct Source.</returns>
        public static IExecutionResult ForCommand<TCommand>(this IExecutionResult executionResult, Expression<Func<TCommand, object>> getProprty)
            where TCommand : ICommand
        {
            if (!(executionResult is FailedResult failedResult))
            {
                return executionResult;
            }

            if (failedResult.Details.Count == 0)
            {
                var source = SourceBuilder.BuildErrorSource(getProprty);
                return new FailedResult(failedResult.CodeInfo, source, failedResult.Message);
            }

            var result = new FailedResult(failedResult.CodeInfo, typeof(TCommand).Name, failedResult.Message);

            var internalError = failedResult.Details.First();
            var internalSource = SourceBuilder.BuildErrorSource(getProprty);
            var newInternalError = new ExecutionError(internalError.CodeInfo, internalSource, internalError.Message);

            result.AddError(newInternalError);

            return result;
        }

        /// <summary>
        /// Extension that filters needed internal errors and fills sources if needed.
        /// </summary>
        /// <typeparam name="TCommand">Type of command.</typeparam>
        /// <param name="executionResult">Failed Execution Result.</param>
        /// <returns>Filed FailedResult with correct Source.</returns>
        public static IExecutionResult ForCommand<TCommand>(this IExecutionResult executionResult)
            where TCommand : ICommand
        {
            if (!(executionResult is FailedResult failedResult))
            {
                return executionResult;
            }

            var commandName = typeof(TCommand).Name;
            var result = new FailedResult(failedResult.CodeInfo, commandName, failedResult.Message);

            // Don't take internal errors if source used for another command.
            foreach (var error in failedResult.Details)
            {
                if (!(string.IsNullOrEmpty(error.Source) || error.Source.Contains(commandName)))
                {
                    // We don't need to take internal error used for another command.
                    continue;
                }

                var internalSource = string.IsNullOrEmpty(error.Source)
                    ? typeof(TCommand).Name
                    : error.Source;

                var newInternalError = new ExecutionError(error.CodeInfo, internalSource, error.Message);

                result.AddError(newInternalError);
            }

            return result;
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using Intellias.CQRS.Core.Commands;
using Intellias.CQRS.Core.Results;
using Intellias.CQRS.Core.Results.Errors;

namespace Intellias.CQRS.Core.Domain.CreationResult
{
    /// <summary>
    /// Extensions for working with <see cref="CreationResult{T}"/>.
    /// </summary>
    public static class CreationResult
    {
        /// <summary>
        /// Converts collection of result into result of collection.
        /// </summary>
        /// <param name="collection">Source collection of results.</param>
        /// <typeparam name="T">Type of the entry.</typeparam>
        /// <returns>Result of collection.</returns>
        public static CreationResult<IReadOnlyCollection<T>> AsCreationResult<T>(this IEnumerable<CreationResult<T>> collection)
            where T : class
        {
            var errors = new List<ExecutionError>();
            var entities = new List<T>();

            foreach (var element in collection)
            {
                if (element.Entry == null)
                {
                    errors.AddRange(element.Errors);
                }
                else
                {
                    entities.Add(element.Entry);
                }
            }

            return errors.Count > 0
                ? new CreationResult<IReadOnlyCollection<T>>(errors)
                : new CreationResult<IReadOnlyCollection<T>>(entities);
        }

        /// <summary>
        /// Creates successful result with an entry.
        /// </summary>
        /// <param name="entry">Created entry.</param>
        /// <typeparam name="T">Type of the entry.</typeparam>
        /// <returns>Successful result.</returns>
        public static CreationResult<T> Succeeded<T>(T entry)
            where T : class
        {
            return new CreationResult<T>(entry);
        }

        /// <summary>
        /// Creates failed CreationResult using default error code.
        /// </summary>
        /// <param name="errorCodeInfo">Error code info.</param>
        /// <typeparam name="TResult">Type of the entry.</typeparam>
        /// <returns>Failed result.</returns>
        public static CreationResult<TResult> Failed<TResult>(ErrorCodeInfo errorCodeInfo)
            where TResult : class
        {
            var executionError = new ExecutionError(errorCodeInfo);

            return new CreationResult<TResult>(executionError);
        }

        /// <summary>
        /// Creates failed CreationResult using error code and custom error message.
        /// </summary>
        /// <param name="errorCodeInfo">Error code info.</param>
        /// <param name="message">Custom Error message.</param>
        /// <typeparam name="TResult">Type of the entry.</typeparam>
        /// <returns>Failed result.</returns>
        public static CreationResult<TResult> Failed<TResult>(ErrorCodeInfo errorCodeInfo, string message)
            where TResult : class
        {
            var executionError = new ExecutionError(errorCodeInfo, null, message);

            return new CreationResult<TResult>(executionError);
        }

        /// <summary>
        /// Converts Creation Result to Creation Result with tracking errors containing correct source in fail case.
        /// </summary>
        /// <typeparam name="TResult">Type of entity to be created.</typeparam>
        /// <typeparam name="TCommand">Command type.</typeparam>
        /// <param name="creationResult">Creation Result.</param>
        /// <param name="getProperty">Property of command to fill as source.</param>
        /// <returns>Modified creation result.</returns>
        public static CreationResult<TResult> ForCommand<TResult, TCommand>(
            this CreationResult<TResult> creationResult,
            Expression<Func<TCommand, object>> getProperty)
            where TCommand : ICommand
            where TResult : class
        {
            if (creationResult.Entry != null)
            {
                return creationResult;
            }

            var source = SourceBuilder.BuildErrorSource(getProperty);
            var firstExecutionError = creationResult.Errors.First();

            // It's the first error and we should fill correct source path for this error.
            if (string.IsNullOrEmpty(firstExecutionError.Source))
            {
                var updatedError = new ExecutionError(firstExecutionError.CodeInfo, source, firstExecutionError.Message);
                return new CreationResult<TResult>(updatedError);
            }

            // There are error with filled source => that we need to create copy of error with another source
            var newError = new ExecutionError(firstExecutionError.CodeInfo, source, firstExecutionError.Message);

            var allErrors = creationResult.Errors.ToList();
            allErrors.Add(newError);

            return new CreationResult<TResult>(allErrors);
        }
    }
}
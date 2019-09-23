using System.Collections.Generic;
using Intellias.CQRS.Core.Results;

namespace Intellias.CQRS.Core.Domain.Validation
{
    /// <summary>
    /// Entry creation result.
    /// </summary>
    public static class CreationResult
    {
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
        /// Creates failed result with an errors.
        /// </summary>
        /// <param name="errorMessage">Error message.</param>
        /// <param name="innerErrors">Details for current error.</param>
        /// <typeparam name="T">Type of the entry.</typeparam>
        /// <returns>Failed result.</returns>
        public static CreationResult<T> Failed<T>(string errorMessage, params ExecutionError[] innerErrors)
            where T : class
        {
            return Failed<T>(errorMessage, (IReadOnlyCollection<ExecutionError>)innerErrors);
        }

        /// <summary>
        /// Creates failed result with an errors.
        /// </summary>
        /// <param name="errorMessage">Error message.</param>
        /// <param name="innerErrors">Details for current error.</param>
        /// <typeparam name="T">Type of the entry.</typeparam>
        /// <returns>Failed result.</returns>
        public static CreationResult<T> Failed<T>(string errorMessage, IReadOnlyCollection<ExecutionError> innerErrors)
            where T : class
        {
            var currentType = typeof(T).Name;
            var resultErrors = new List<ExecutionError>
            {
                new ExecutionError(ErrorCodes.ValidationFailed, currentType, errorMessage)
            };

            foreach (var error in innerErrors)
            {
                var source = $"{currentType}.{error.Source}";
                resultErrors.Add(new ExecutionError(error.Code, source, error.Message));
            }

            return new CreationResult<T>(resultErrors);
        }
    }
}
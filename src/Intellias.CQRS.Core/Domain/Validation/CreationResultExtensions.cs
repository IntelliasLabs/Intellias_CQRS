using System.Collections.Generic;
using Intellias.CQRS.Core.Results;

namespace Intellias.CQRS.Core.Domain.Validation
{
    /// <summary>
    /// Extensions for working with <see cref="CreationResult{T}"/>.
    /// </summary>
    public static class CreationResultExtensions
    {
        /// <summary>
        /// Converts collection of result into result of collection.
        /// </summary>
        /// <param name="source">Source collection of results.</param>
        /// <typeparam name="T">Type of the entry.</typeparam>
        /// <returns>Result of collection.</returns>
        public static CreationResult<IReadOnlyCollection<T>> AsCreationResult<T>(this IEnumerable<CreationResult<T>> source)
        {
            var errors = new List<ExecutionError>();
            var entries = new List<T>();

            foreach (var element in source)
            {
                if (element.Succeeded)
                {
                    entries.Add(element.Entry);
                }
                else
                {
                    errors.AddRange(element.Errors);
                }
            }

            return errors.Count > 0
                ? new CreationResult<IReadOnlyCollection<T>>(errors)
                : new CreationResult<IReadOnlyCollection<T>>(entries);
        }
    }
}
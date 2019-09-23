using System;
using System.Collections.Generic;
using Intellias.CQRS.Core.Results;

namespace Intellias.CQRS.Core.Domain.Validation
{
    /// <summary>
    /// Entry creation result.
    /// </summary>
    /// <typeparam name="T">Type of the entry.</typeparam>
    public class CreationResult<T>
        where T : class
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="CreationResult{T}"/> class.
        /// </summary>
        /// <param name="entry">Value for <see cref="Entry"/>.</param>
        public CreationResult(T entry)
        {
            Entry = entry;
            Errors = Array.Empty<ExecutionError>();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="CreationResult{T}"/> class.
        /// </summary>
        /// <param name="errors">Value for <see cref="Errors"/>.</param>
        public CreationResult(params ExecutionError[] errors)
            : this((IReadOnlyList<ExecutionError>)errors)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="CreationResult{T}"/> class.
        /// </summary>
        /// <param name="errors">Value for <see cref="Errors"/>.</param>
        public CreationResult(IReadOnlyCollection<ExecutionError> errors)
        {
            Errors = errors;
        }

        /// <summary>
        /// Created entry.
        /// </summary>
        public T? Entry { get; }

        /// <summary>
        /// Creation errors.
        /// </summary>
        public IReadOnlyCollection<ExecutionError> Errors { get; }

        /// <summary>
        /// Deconstructs result into entry and errors.
        /// </summary>
        /// <param name="errors">Value of <see cref="Errors"/>.</param>
        /// <param name="entry">Value of <see cref="Entry"/>.</param>
        public void Deconstruct(out IReadOnlyCollection<ExecutionError> errors, out T? entry)
        {
            errors = Errors;
            entry = Entry;
        }
    }
}
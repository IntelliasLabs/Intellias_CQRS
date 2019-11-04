using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Intellias.CQRS.Core.Results;

namespace Intellias.CQRS.Core.DataAnnotations.Validators
{
    /// <summary>
    /// <see cref="IValidatableObject"/> that produces errors with codes.
    /// </summary>
    public interface ICoreValidatableObject
    {
        /// <summary>
        /// Validates current instance.
        /// </summary>
        /// <param name="validationContext">Validation context.</param>
        /// <returns>Found errors.</returns>
        IEnumerable<ExecutionError> Validate(ValidationContext validationContext);
    }
}
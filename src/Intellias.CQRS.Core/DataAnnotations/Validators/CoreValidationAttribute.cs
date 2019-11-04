using System;
using System.ComponentModel.DataAnnotations;
using Intellias.CQRS.Core.Results.Errors;

namespace Intellias.CQRS.Core.DataAnnotations.Validators
{
    /// <summary>
    /// Validation attribute that provides <see cref="ErrorCodeInfo"/>.
    /// </summary>
    public abstract class CoreValidationAttribute : ValidationAttribute
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="CoreValidationAttribute"/> class.
        /// </summary>
        protected CoreValidationAttribute()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="CoreValidationAttribute"/> class.
        /// </summary>
        /// <param name="errorMessageAccessor">Provides error message.</param>
        protected CoreValidationAttribute(Func<string> errorMessageAccessor)
            : base(errorMessageAccessor)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="CoreValidationAttribute"/> class.
        /// </summary>
        /// <param name="errorMessage">Error message.</param>
        protected CoreValidationAttribute(string errorMessage)
            : base(errorMessage)
        {
        }

        /// <summary>
        /// Error code that attribute produces.
        /// </summary>
        public abstract ErrorCodeInfo ErrorCode { get; }
    }
}
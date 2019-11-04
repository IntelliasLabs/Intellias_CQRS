using System;
using System.Collections;
using System.Linq;
using Intellias.CQRS.Core.DataAnnotations.Validators;
using Intellias.CQRS.Core.Results.Errors;

namespace Intellias.CQRS.Core.DataAnnotations
{
    /// <summary>
    /// Validation attribute to indicate that a property field or parameter is not empty.
    /// For strings ensures that string is not empty or white space.
    /// For collections ensures that collection length is non-zero.
    /// </summary>
    public sealed class NotEmptyAttribute : CoreValidationAttribute
    {
        /// <summary>
        /// Template of the error message for <see cref="NotEmptyAttribute"/>.
        /// </summary>
        public const string ErrorMessageTemplate = "The field {0} can't have empty value.";

        /// <summary>
        /// Initializes a new instance of the <see cref="NotEmptyAttribute"/> class.
        /// </summary>
        public NotEmptyAttribute()
            : base(ErrorMessageTemplate)
        {
        }

        /// <inheritdoc />
        public override ErrorCodeInfo ErrorCode => CoreErrorCodes.CantBeEmpty;

        /// <inheritdoc />
        public override bool IsValid(object value)
        {
            switch (value)
            {
                case string s when string.IsNullOrWhiteSpace(s):
                case ICollection c when c.Count == 0:
                case Array a when a.Length == 0:
                case IEnumerable e when !e.Cast<object>().Any():
                    return false;
                default:
                    return true;
            }
        }
    }
}
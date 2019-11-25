using System;
using System.ComponentModel.DataAnnotations;

namespace Intellias.CQRS.Core.DataAnnotations
{
    /// <summary>
    /// Collection Validation attribute to verify all elements with possible validation attributes.
    /// Used with any IEnumerables.
    /// </summary>
    [AttributeUsage(AttributeTargets.All, AllowMultiple = true)]
    public sealed class ValidateCollectionAttribute : Attribute
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ValidateCollectionAttribute"/> class.
        /// </summary>
        /// <param name="attributeType">Type of the attribute to verify internal elements.</param>
        /// <param name="validatorArguments">Possible array of the arguments of the mentioned attribute validator.</param>
        public ValidateCollectionAttribute(Type attributeType, params object[] validatorArguments)
        {
            InlineAttribute = (ValidationAttribute)Activator.CreateInstance(attributeType, validatorArguments);
        }

        /// <summary>
        /// Attribute to verify internal elements with.
        /// </summary>
        public ValidationAttribute InlineAttribute { get; }
    }
}
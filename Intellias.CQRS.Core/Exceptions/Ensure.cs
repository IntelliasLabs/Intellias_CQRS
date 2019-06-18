using System;

namespace Intellias.CQRS.Core.Exceptions
{
    /// <summary>
    /// Ensure that businesses rule is valid
    /// </summary>
    public static class Ensure
    {
        /// <summary>
        /// Throw BusinessRuleValidationException if condition invalid
        /// </summary>
        /// <param name="condition"></param>
        /// <param name="validationMessage"></param>
        public static void That(Func<bool> condition, string validationMessage)
        {
            if (condition != null && !condition())
            {
                throw new BusinessRuleValidationException(validationMessage);
            }
        }

        /// <summary>
        /// Throw BusinessRuleValidationException if invalid
        /// </summary>
        /// <param name="isValid"></param>
        /// <param name="validationMessage"></param>
        public static void That(bool isValid, string validationMessage)
        {
            if (!isValid)
            {
                throw new BusinessRuleValidationException(validationMessage);
            }
        }
    }
}

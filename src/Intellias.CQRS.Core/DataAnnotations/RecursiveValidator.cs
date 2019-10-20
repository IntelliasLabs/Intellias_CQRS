using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Reflection;
using Intellias.CQRS.Core.Results;

namespace Intellias.CQRS.Core.DataAnnotations
{
    /// <summary>
    /// <see cref="Validator"/> that does recursive validation on IntelliGrowth types.
    /// </summary>
    public static class RecursiveValidator
    {
        /// <summary>
        /// Namespaces of the types that shouldn't be validated recursively.
        /// </summary>
        private static readonly string[] SkipRecursiveValidationNamespaces = { nameof(Microsoft), nameof(System) };

        /// <summary>
        /// Validates instance using DataAnnotations.
        /// </summary>
        /// <param name="instance">Validated instance.</param>
        /// <returns><see cref="FailedResult"/> if invalid, otherwise <see cref="SuccessfulResult"/>.</returns>
        public static IExecutionResult Validate(object instance)
        {
            if (instance == null)
            {
                return new SuccessfulResult();
            }

            var validationContext = new ValidationContext(instance);
            var validationResults = new List<ValidationResult>();

            // Validate current level of the instance.
            Validator.TryValidateObject(instance, validationContext, validationResults, true);

            var instanceType = instance.GetType();
            var result = new FailedResult(ErrorCodes.ValidationFailed, instanceType.Name, "Instance validation failed.");
            foreach (var validationResult in validationResults)
            {
                var field = validationResult.MemberNames.FirstOrDefault() ?? string.Empty;
                var error = new ExecutionError(field, validationResult.ErrorMessage);

                result.AddError(error);
            }

            // Recursively validate nested types.
            var properties = instanceType
                .GetProperties(BindingFlags.Instance | BindingFlags.Public | BindingFlags.GetProperty)
                .Where(PropertyCanBeValidated);

            foreach (var property in properties)
            {
                var propertyValue = property.GetValue(instance);
                var propertyResult = Validate(propertyValue);
                if (!(propertyResult is FailedResult fr))
                {
                    continue;
                }

                var nestedExecutionErrors = fr.Details.Select(er => new ExecutionError(er.Code, $"{property.Name}.{er.Source}", er.Message));
                foreach (var executionError in nestedExecutionErrors)
                {
                    result.AddError(executionError);
                }
            }

            return result.Details.Any()
                ? (IExecutionResult)result
                : new SuccessfulResult();
        }

        private static bool PropertyCanBeValidated(PropertyInfo propertyInfo)
        {
            var typeNamespace = propertyInfo.PropertyType.Namespace;
            return typeNamespace != null && !SkipRecursiveValidationNamespaces.Any(n => typeNamespace.StartsWith(n, StringComparison.InvariantCulture));
        }
    }
}
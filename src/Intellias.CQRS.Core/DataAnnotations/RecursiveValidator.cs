using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Reflection;
using Intellias.CQRS.Core.DataAnnotations.Validators;
using Intellias.CQRS.Core.Results;
using Intellias.CQRS.Core.Results.Errors;

namespace Intellias.CQRS.Core.DataAnnotations
{
    /// <summary>
    /// <see cref="CoreValidator"/> that does recursive validation on IntelliGrowth types.
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
            var executionErrors = new List<ExecutionError>();

            // Validate current level of the instance.
            CoreValidator.TryValidateObject(instance, validationContext, executionErrors, true);

            var instanceType = instance.GetType();
            var result = new FailedResult(CoreErrorCodes.ValidationFailed, instanceType.Name);
            foreach (var executionError in executionErrors)
            {
                var field = executionError.Source ?? string.Empty;
                var source = $"{instanceType.Name}.{field}";
                var error = new ExecutionError(executionError.CodeInfo, source);

                result.AddError(error);
            }

            // Recursively validate nested types.
            var properties = instanceType
                .GetProperties(BindingFlags.Instance | BindingFlags.Public | BindingFlags.GetProperty)
                .Where(PropertyCanBeValidated);

            foreach (var property in properties)
            {
                var propertyValue = property.GetValue(instance);

                if (typeof(System.Collections.IEnumerable).IsAssignableFrom(property.PropertyType))
                {
                    UpdateFailedResultWithArrayElementsDetails(result, property, instanceType, propertyValue);
                    continue;
                }

                var propertyResult = Validate(propertyValue);
                if (!(propertyResult is FailedResult fr))
                {
                    continue;
                }

                UpdateFailedResultWithDetails(result, fr, property, instanceType);
            }

            return result.Details.Any()
                ? (IExecutionResult)result
                : new SuccessfulResult();
        }

        private static void UpdateFailedResultWithArrayElementsDetails(
            FailedResult outputResultToUpdate,
            PropertyInfo collectionProperty,
            Type instanceType,
            object collectionValue)
        {
            if (collectionValue == null)
            {
                return;
            }

            var attributesFromValidateCollection = CustomAttributeExtensions.GetCustomAttributes(collectionProperty, true)
                .OfType<ValidateCollectionAttribute>()
                .Select(a => a.InlineAttribute)
                .ToList();

            var counter = -1;
            foreach (var arrayElement in (System.Collections.IEnumerable)collectionValue)
            {
                counter++;

                if (arrayElement == null)
                {
                    if (attributesFromValidateCollection.Any(a => a.GetType() == typeof(RequiredAttribute)))
                    {
                        var source = $"{instanceType.Name}.{collectionProperty.Name}.{counter}";
                        outputResultToUpdate.AddError(new ExecutionError(AnnotationErrorCodes.Required, source));
                    }

                    continue;
                }

                // Validate every array element itself.
                var arrayResultErrors = CoreValidator.GetValidationErrors(arrayElement, new ValidationContext(arrayElement), attributesFromValidateCollection, false);
                foreach (var arrayElementError in arrayResultErrors)
                {
                    var source = $"{instanceType.Name}.{collectionProperty.Name}.{counter}";
                    outputResultToUpdate.AddError(new ExecutionError(arrayElementError.CodeInfo, source));
                }

                var arrayType = arrayElement.GetType();
                if (arrayType.IsPrimitive || arrayType == typeof(decimal) || arrayType == typeof(string))
                {
                    continue;
                }

                // Validate properties of an array element recursively if it's not an primitive type.
                var propertyResult = Validate(arrayElement);
                if (!(propertyResult is FailedResult fr))
                {
                    continue;
                }

                UpdateFailedResultWithDetails(outputResultToUpdate, fr, collectionProperty, instanceType, counter);
            }
        }

        private static void UpdateFailedResultWithDetails(
            FailedResult outputResultToUpdate,
            FailedResult takeDetailsFrom,
            PropertyInfo property,
            Type instanceType,
            int? indexCounter = null)
        {
            var nestedExecutionErrors = takeDetailsFrom.Details.Select(error =>
            {
                var updatedInternalSource = indexCounter == null
                    ? error.Source.Replace(takeDetailsFrom.Source, property.Name)
                    : error.Source.Replace(takeDetailsFrom.Source, $"{property.Name}.{indexCounter}");

                return new ExecutionError(error.CodeInfo, $"{instanceType.Name}.{updatedInternalSource}", error.Message);
            });

            foreach (var executionError in nestedExecutionErrors)
            {
                outputResultToUpdate.AddError(executionError);
            }
        }

        private static bool PropertyCanBeValidated(PropertyInfo propertyInfo)
        {
            if (typeof(System.Collections.IEnumerable).IsAssignableFrom(propertyInfo.PropertyType) && propertyInfo.PropertyType != typeof(string))
            {
                return true;
            }

            var typeNamespace = propertyInfo.PropertyType.Namespace;
            return typeNamespace != null && !SkipRecursiveValidationNamespaces.Any(n => typeNamespace.StartsWith(n, StringComparison.InvariantCulture));
        }
    }
}
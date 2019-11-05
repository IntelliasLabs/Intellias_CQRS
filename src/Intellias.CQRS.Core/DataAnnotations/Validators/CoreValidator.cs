using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Reflection;
using Intellias.CQRS.Core.Results;
using Intellias.CQRS.Core.Results.Errors;

namespace Intellias.CQRS.Core.DataAnnotations.Validators
{
    /// <summary>
    /// Helper class to validate objects, properties and other values using their associated <see cref="ValidationAttribute" /> custom attributes.
    /// </summary>
    public static class CoreValidator
    {
        private static readonly CoreValidationAttributeStore Store = CoreValidationAttributeStore.Instance;

        /// <summary>
        /// Tests whether the given object instance is valid.
        /// </summary>
        /// <param name="instance">The object instance to test.  It cannot be null.</param>
        /// <param name="validationContext">Describes the object to validate and provides services and context for the validators.</param>
        /// <param name="executionErrors">Optional collection to receive <see cref="ExecutionError" />s for the failures.</param>
        /// <param name="validateAllProperties">
        /// If <c>true</c>, also evaluates all properties of the object (this process is not recursive over properties of the properties).
        /// </param>
        /// <returns><c>true</c> if the object is valid, <c>false</c> if any validation errors are encountered.</returns>
        public static bool TryValidateObject(
            object instance,
            ValidationContext validationContext,
            ICollection<ExecutionError> executionErrors,
            bool validateAllProperties)
        {
            if (validationContext != null && instance != validationContext.ObjectInstance)
            {
                throw new ArgumentException("The instance provided must match the ObjectInstance on the ValidationContext supplied.", nameof(instance));
            }

            var result = true;
            var breakOnFirstError = executionErrors == null;

            foreach (var error in GetObjectValidationErrors(instance, validationContext, validateAllProperties, breakOnFirstError))
            {
                result = false;

                executionErrors?.Add(error);
            }

            return result;
        }

        /// <summary>
        /// Internal iterator to enumerate all validation errors for the given object instance.
        /// </summary>
        /// <param name="instance">Object instance to test.</param>
        /// <param name="validationContext">Describes the object type.</param>
        /// <param name="validateAllProperties">if <c>true</c> also validates all properties.</param>
        /// <param name="breakOnFirstError">Whether to break on the first error or validate everything.</param>
        /// <returns>
        /// A collection of validation errors that result from validating the <paramref name="instance" /> with
        /// the given <paramref name="validationContext" />.
        /// </returns>
        private static IEnumerable<ExecutionError> GetObjectValidationErrors(
            object instance,
            ValidationContext validationContext,
            bool validateAllProperties,
            bool breakOnFirstError)
        {
            // Step 1: Validate the object properties' validation attributes.
            var propertyValidationErrors = GetObjectPropertyValidationErrors(instance, validationContext, validateAllProperties, breakOnFirstError);
            var errors = new List<ExecutionError>(propertyValidationErrors);

            // We only proceed to Step 2 if there are no errors.
            if (errors.Any())
            {
                return errors;
            }

            // Step 2: Validate the object's validation attributes.
            var attributes = Store.GetTypeValidationAttributes(validationContext);
            errors.AddRange(GetValidationErrors(instance, validationContext, attributes, breakOnFirstError));

            // We only proceed to Step 3 if there are no errors.
            if (errors.Any())
            {
                return errors;
            }

            // Step 3: Test for IValidatableObject implementation.
            if (instance is IValidatableObject validatable)
            {
                var validatableObjectErrors = validatable.Validate(validationContext)
                    .Where(r => r != ValidationResult.Success)
                    .Select(result => CreateExecutionError(null, result));

                errors.AddRange(validatableObjectErrors);
            }

            // Step 4: Test for ICoreValidatableObject implementation.
            if (instance is ICoreValidatableObject coreValidatable)
            {
                var coreValidatableObjectErrors = coreValidatable.Validate(validationContext);

                errors.AddRange(coreValidatableObjectErrors);
            }

            return errors;
        }

        /// <summary>
        /// Internal iterator to enumerate all the validation errors for all properties of the given object instance.
        /// </summary>
        /// <param name="instance">Object instance to test.</param>
        /// <param name="validationContext">Describes the object type.</param>
        /// <param name="validateAllProperties">
        /// If <c>true</c>, evaluates all the properties, otherwise just checks that ones marked with <see cref="RequiredAttribute" /> are not null.
        /// </param>
        /// <param name="breakOnFirstError">Whether to break on the first error or validate everything.</param>
        /// <returns>A list of <see cref="ExecutionError" /> instances.</returns>
        private static IEnumerable<ExecutionError> GetObjectPropertyValidationErrors(
            object instance,
            ValidationContext validationContext,
            bool validateAllProperties,
            bool breakOnFirstError)
        {
            var properties = GetPropertyValues(instance, validationContext);
            var errors = new List<ExecutionError>();

            foreach (var property in properties)
            {
                // get list of all validation attributes for this property.
                var attributes = Store.GetPropertyValidationAttributes(property.Key);

                if (validateAllProperties)
                {
                    // validate all validation attributes on this property.
                    errors.AddRange(GetValidationErrors(property.Value, property.Key, attributes, breakOnFirstError));
                }
                else
                {
                    // only validate the Required attributes.
                    var reqAttr = attributes.OfType<RequiredAttribute>().FirstOrDefault();
                    if (reqAttr != null)
                    {
                        // Note: we let the [Required] attribute do its own null testing,
                        // since the user may have subclassed it and have a deeper meaning to what 'required' means.
                        var validationResult = reqAttr.GetValidationResult(property.Value, property.Key);
                        if (validationResult != ValidationResult.Success)
                        {
                            errors.Add(CreateExecutionError(reqAttr, validationResult));
                        }
                    }
                }

                if (breakOnFirstError && errors.Any())
                {
                    break;
                }
            }

            return errors;
        }

        /// <summary>
        /// Retrieves the property values for the given instance.
        /// </summary>
        /// <param name="instance">Instance from which to fetch the properties.</param>
        /// <param name="validationContext">Describes the entity being validated.</param>
        /// <returns>A set of key value pairs, where the key is a validation context for the property and the value is its current value.</returns>
        private static ICollection<KeyValuePair<ValidationContext, object>> GetPropertyValues(object instance, ValidationContext validationContext)
        {
            var properties = instance.GetType().GetRuntimeProperties()
                .Where(p => CoreValidationAttributeStore.IsPublic(p) && !p.GetIndexParameters().Any())
                .ToArray();

            var items = new List<KeyValuePair<ValidationContext, object>>(properties.Length);
            foreach (var property in properties)
            {
                var context = new ValidationContext(instance, validationContext, validationContext.Items)
                {
                    MemberName = property.Name
                };

                if (Store.GetPropertyValidationAttributes(context).Any())
                {
                    items.Add(new KeyValuePair<ValidationContext, object>(context, property.GetValue(instance, null)));
                }
            }

            return items;
        }

        /// <summary>
        /// Internal iterator to enumerate all validation errors for an value.
        /// </summary>
        /// <param name="value">The value to pass to the validation attributes.</param>
        /// <param name="validationContext">Describes the type/member being evaluated.</param>
        /// <param name="attributes">The validation attributes to evaluate.</param>
        /// <param name="breakOnFirstError">
        /// Whether or not to break on the first validation failure. A <see cref="RequiredAttribute" /> failure will always abort with that sole failure.
        /// </param>
        /// <returns>The collection of validation errors.</returns>
        private static IEnumerable<ExecutionError> GetValidationErrors(
            object value,
            ValidationContext validationContext,
            IReadOnlyCollection<ValidationAttribute> attributes,
            bool breakOnFirstError)
        {
            var errors = new List<ExecutionError>();

            // Get the required validator if there is one and test it first, aborting on failure.
            var required = attributes.OfType<RequiredAttribute>().FirstOrDefault();
            if (required != null && !TryValidate(value, validationContext, required, out var validationError))
            {
                errors.Add(validationError);
                return errors;
            }

            // Iterate through the rest of the validators, skipping the required validator.
            foreach (var attribute in attributes)
            {
                if (ReferenceEquals(attribute, required))
                {
                    continue;
                }

                if (TryValidate(value, validationContext, attribute, out validationError))
                {
                    continue;
                }

                errors.Add(validationError);

                if (breakOnFirstError)
                {
                    break;
                }
            }

            return errors;
        }

        /// <summary>
        /// Tests whether a value is valid against a single <see cref="ValidationAttribute" /> using the <see cref="ValidationContext" />.
        /// </summary>
        /// <param name="value">The value to be tested for validity.</param>
        /// <param name="validationContext">Describes the property member to validate.</param>
        /// <param name="attribute">The validation attribute to test.</param>
        /// <param name="validationError">
        /// The validation error that occurs during validation.  Will be <c>null</c> when the return value is <c>true</c>.
        /// </param>
        /// <returns><c>true</c> if the value is valid.</returns>
        private static bool TryValidate(object value, ValidationContext validationContext, ValidationAttribute attribute, out ExecutionError validationError)
        {
            var validationResult = attribute.GetValidationResult(value, validationContext);
            if (validationResult != ValidationResult.Success)
            {
                validationError = CreateExecutionError(attribute, validationResult);
                return false;
            }

            validationError = null;
            return true;
        }

        private static ExecutionError CreateExecutionError(ValidationAttribute attribute, ValidationResult validationResult)
        {
            var errorCode = AnnotationErrorCodes.GetErrorCodeFromAttribute(attribute.GetType());
            var errorCodeInfo = new ErrorCodeInfo(errorCode, validationResult.ErrorMessage);
            var source = validationResult.MemberNames.FirstOrDefault();

            return new ExecutionError(errorCodeInfo, source, validationResult.ErrorMessage);
        }
    }
}

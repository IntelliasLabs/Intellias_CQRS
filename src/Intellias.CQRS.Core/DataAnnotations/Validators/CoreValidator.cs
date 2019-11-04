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
        /// Tests whether the given property value is valid.
        /// </summary>
        /// <remarks>
        /// This method will test each <see cref="ValidationAttribute" /> associated with the property
        /// identified by <paramref name="validationContext" />.  If <paramref name="executionErrors" /> is non-null,
        /// this method will add a <see cref="ExecutionError" /> to it for each validation failure.
        /// <para>
        /// If there is a <see cref="RequiredAttribute" /> found on the property, it will be evaluated before all other
        /// validation attributes.  If the required validator fails then validation will abort, adding that single
        /// failure into the <paramref name="executionErrors" /> when applicable, returning a value of <c>false</c>.
        /// </para>
        /// <para>
        /// If <paramref name="executionErrors" /> is null and there isn't a <see cref="RequiredAttribute" /> failure, then all validators will be evaluated.
        /// </para>
        /// </remarks>
        /// <param name="value">The value to test.</param>
        /// <param name="validationContext">Describes the property member to validate and provides services and context for the validators.</param>
        /// <param name="executionErrors">Optional collection to receive <see cref="ExecutionError" />s for the failures.</param>
        /// <returns><c>true</c> if the value is valid, <c>false</c> if any validation errors are encountered.</returns>
        /// <exception cref="ArgumentException">
        /// When the <see cref="ValidationContext.MemberName" /> of <paramref name="validationContext" /> is not a valid property.
        /// </exception>
        public static bool TryValidateProperty(object value, ValidationContext validationContext, ICollection<ExecutionError> executionErrors)
        {
            // Throw if value cannot be assigned to this property.  That is not a validation exception.
            var propertyType = Store.GetPropertyType(validationContext);
            var propertyName = validationContext.MemberName;
            EnsureValidPropertyType(propertyName, propertyType, value);

            var result = true;
            var breakOnFirstError = executionErrors == null;

            var attributes = Store.GetPropertyValidationAttributes(validationContext);

            foreach (var error in GetValidationErrors(value, validationContext, attributes, breakOnFirstError))
            {
                result = false;

                executionErrors?.Add(error.ExecutionError);
            }

            return result;
        }

        /// <summary>
        /// Tests whether the given object instance is valid.
        /// </summary>
        /// <remarks>
        /// This method evaluates all <see cref="ValidationAttribute" />s attached to the object instance's type.  It also
        /// checks to ensure all properties marked with <see cref="RequiredAttribute" /> are set.  It does not validate the
        /// property values of the object.
        /// <para>
        /// If <paramref name="executionErrors" /> is null, then execution will abort upon the first validation
        /// failure. If <paramref name="executionErrors" /> is non-null, then all validation attributes will be evaluated.
        /// </para>
        /// </remarks>
        /// <param name="instance">The object instance to test.  It cannot be <c>null</c>.</param>
        /// <param name="validationContext">Describes the object to validate and provides services and context for the validators.</param>
        /// <param name="executionErrors">Optional collection to receive <see cref="ExecutionError" />s for the failures.</param>
        /// <returns><c>true</c> if the object is valid, <c>false</c> if any validation errors are encountered.</returns>
        /// <exception cref="ArgumentNullException">When <paramref name="instance" /> is null.</exception>
        /// <exception cref="ArgumentException">
        /// When <paramref name="instance" /> doesn't match the <see cref="ValidationContext.ObjectInstance" />on <paramref name="validationContext" />.
        /// </exception>
        public static bool TryValidateObject(object instance, ValidationContext validationContext, ICollection<ExecutionError> executionErrors)
        {
            return TryValidateObject(instance, validationContext, executionErrors, validateAllProperties: false);
        }

        /// <summary>
        /// Tests whether the given object instance is valid.
        /// </summary>
        /// <remarks>
        /// This method evaluates all <see cref="ValidationAttribute" />s attached to the object instance's type.  It also
        /// checks to ensure all properties marked with <see cref="RequiredAttribute" /> are set.  If <paramref name="validateAllProperties" />
        /// is <c>true</c>, this method will also evaluate the <see cref="ValidationAttribute" />s for all the immediate
        /// properties of this object. This process is not recursive.
        /// <para>
        /// If <paramref name="executionErrors" /> is null, then execution will abort upon the first validation
        /// failure. If <paramref name="executionErrors" /> is non-null, then all validation attributes will be evaluated.
        /// </para>
        /// <para>
        /// For any given property, if it has a <see cref="RequiredAttribute" /> that fails validation, no other validators will be evaluated for that property.
        /// </para>
        /// </remarks>
        /// <param name="instance">The object instance to test.  It cannot be null.</param>
        /// <param name="validationContext">Describes the object to validate and provides services and context for the validators.</param>
        /// <param name="executionErrors">Optional collection to receive <see cref="ExecutionError" />s for the failures.</param>
        /// <param name="validateAllProperties">
        /// If <c>true</c>, also evaluates all properties of the object (this process is not recursive over properties of the properties).
        /// </param>
        /// <returns><c>true</c> if the object is valid, <c>false</c> if any validation errors are encountered.</returns>
        /// <exception cref="ArgumentNullException">When <paramref name="instance" /> is null.</exception>
        /// <exception cref="ArgumentException">
        /// When <paramref name="instance" /> doesn't match the <see cref="ValidationContext.ObjectInstance" />on <paramref name="validationContext" />.
        /// </exception>
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

                executionErrors?.Add(error.ExecutionError);
            }

            return result;
        }

        /// <summary>
        /// Tests whether the given value is valid against a specified list of <see cref="ValidationAttribute" />s.
        /// </summary>
        /// <remarks>
        /// This method will test each <see cref="ValidationAttribute" />s specified. If <paramref name="executionErrors" /> is non-null,
        /// this method will add a <see cref="ExecutionError" /> to it for each validation failure.
        /// <para>
        /// If there is a <see cref="RequiredAttribute" /> within the <paramref name="validationAttributes" />, it will
        /// be evaluated before all other validation attributes.  If the required validator fails then validation will
        /// abort, adding that single failure into the <paramref name="executionErrors" /> when applicable, returning a
        /// value of <c>false</c>.
        /// </para>
        /// <para>
        /// If <paramref name="executionErrors" /> is null and there isn't a <see cref="RequiredAttribute" /> failure, then all validators will be evaluated.
        /// </para>
        /// </remarks>
        /// <param name="value">The value to test. It cannot be null.</param>
        /// <param name="validationContext">Describes the object being validated and provides services and context for the validators.</param>
        /// <param name="executionErrors">Optional collection to receive <see cref="ExecutionError" />s for the failures.</param>
        /// <param name="validationAttributes">The list of <see cref="ValidationAttribute" />s to validate this <paramref name="value" /> against.</param>
        /// <returns><c>true</c> if the object is valid, <c>false</c> if any validation errors are encountered.</returns>
        public static bool TryValidateValue(
            object value,
            ValidationContext validationContext,
            ICollection<ExecutionError> executionErrors,
            IReadOnlyCollection<ValidationAttribute> validationAttributes)
        {
            var result = true;
            var breakOnFirstError = executionErrors == null;

            foreach (var error in GetValidationErrors(value, validationContext, validationAttributes, breakOnFirstError))
            {
                result = false;

                executionErrors?.Add(error.ExecutionError);
            }

            return result;
        }

        /// <summary>
        /// Throws a <see cref="ValidationException" /> if the given property <paramref name="value" /> is not valid.
        /// </summary>
        /// <param name="value">The value to test.</param>
        /// <param name="validationContext">
        /// Describes the object being validated and provides services and context for the validators. It cannot be <c>null</c>.
        /// </param>
        /// <exception cref="ArgumentNullException">When <paramref name="validationContext" /> is null.</exception>
        /// <exception cref="ValidationException">When <paramref name="value" /> is invalid for this property.</exception>
        public static void ValidateProperty(object value, ValidationContext validationContext)
        {
            // Throw if value cannot be assigned to this property.  That is not a validation exception.
            var propertyType = Store.GetPropertyType(validationContext);
            EnsureValidPropertyType(validationContext.MemberName, propertyType, value);

            var attributes = Store.GetPropertyValidationAttributes(validationContext);

            GetValidationErrors(value, validationContext, attributes, false).FirstOrDefault()?.ThrowValidationException();
        }

        /// <summary>
        /// Throws a <see cref="ValidationException" /> if the given <paramref name="instance" /> is not valid.
        /// </summary>
        /// <remarks>
        /// This method evaluates all <see cref="ValidationAttribute" />s attached to the object's type.
        /// </remarks>
        /// <param name="instance">The object instance to test.  It cannot be null.</param>
        /// <param name="validationContext">
        /// Describes the object being validated and provides services and context for the validators. It cannot be <c>null</c>.
        /// </param>
        /// <exception cref="ArgumentNullException">When <paramref name="instance" /> is null.</exception>
        /// <exception cref="ArgumentNullException">When <paramref name="validationContext" /> is null.</exception>
        /// <exception cref="ArgumentException">
        /// When <paramref name="instance" /> doesn't match the <see cref="ValidationContext.ObjectInstance" /> on <paramref name="validationContext" />.
        /// </exception>
        /// <exception cref="ValidationException">When <paramref name="instance" /> is found to be invalid.</exception>
        public static void ValidateObject(object instance, ValidationContext validationContext)
        {
            ValidateObject(instance, validationContext, false /*validateAllProperties*/);
        }

        /// <summary>
        /// Throws a <see cref="ValidationException" /> if the given object instance is not valid.
        /// </summary>
        /// <remarks>
        /// This method evaluates all <see cref="ValidationAttribute" />s attached to the object's type.
        /// If <paramref name="validateAllProperties" /> is <c>true</c> it also validates all the object's properties.
        /// </remarks>
        /// <param name="instance">The object instance to test. It cannot be null.</param>
        /// <param name="validationContext">
        /// Describes the object being validated and provides services and context for the validators. It cannot be <c>null</c>.
        /// </param>
        /// <param name="validateAllProperties">If <c>true</c>, also validates all the <paramref name="instance" />'s properties.</param>
        /// <exception cref="ArgumentNullException">When <paramref name="instance" /> is null.</exception>
        /// <exception cref="ArgumentNullException">When <paramref name="validationContext" /> is null.</exception>
        /// <exception cref="ArgumentException">
        /// When <paramref name="instance" /> doesn't match the <see cref="ValidationContext.ObjectInstance" /> on <paramref name="validationContext" />.
        /// </exception>
        /// <exception cref="ValidationException">When <paramref name="instance" /> is found to be invalid.</exception>
        public static void ValidateObject(object instance, ValidationContext validationContext, bool validateAllProperties)
        {
            if (instance != validationContext.ObjectInstance)
            {
                throw new ArgumentException("The instance provided must match the ObjectInstance on the ValidationContext supplied.", nameof(instance));
            }

            GetObjectValidationErrors(instance, validationContext, validateAllProperties, false).FirstOrDefault()?.ThrowValidationException();
        }

        /// <summary>
        /// Throw a <see cref="ValidationException" /> if the given value is not valid for the
        /// <see cref="ValidationAttribute" />s.
        /// </summary>
        /// <remarks>
        /// This method evaluates the <see cref="ValidationAttribute" />s supplied until a validation error occurs,
        /// at which time a <see cref="ValidationException" /> is thrown.
        /// A <see cref="RequiredAttribute" /> within the <paramref name="validationAttributes" /> will always be evaluated first.
        /// </remarks>
        /// <param name="value">The value to test.  It cannot be null.</param>
        /// <param name="validationContext">Describes the object being tested.</param>
        /// <param name="validationAttributes">The list of <see cref="ValidationAttribute" />s to validate against this instance.</param>
        /// <exception cref="ArgumentNullException">When <paramref name="validationContext" /> is null.</exception>
        /// <exception cref="ValidationException">When <paramref name="value" /> is found to be invalid.</exception>
        public static void ValidateValue(object value, ValidationContext validationContext, IReadOnlyCollection<ValidationAttribute> validationAttributes)
        {
            if (validationContext == null)
            {
                throw new ArgumentNullException(nameof(validationContext));
            }

            GetValidationErrors(value, validationContext, validationAttributes, false).FirstOrDefault()?.ThrowValidationException();
        }

        /// <summary>
        /// Determine whether the given value can legally be assigned into the specified type.
        /// </summary>
        /// <param name="destinationType">The destination <see cref="Type" /> for the value.</param>
        /// <param name="value">The value to test to see if it can be assigned as the Type indicated by <paramref name="destinationType" />.</param>
        /// <returns><c>true</c> if the assignment is legal.</returns>
        /// <exception cref="ArgumentNullException">When <paramref name="destinationType" /> is null.</exception>
        private static bool CanBeAssigned(Type destinationType, object value)
        {
            if (value == null)
            {
                // Null can be assigned only to reference types or Nullable or Nullable<>.
                return !destinationType.IsValueType || (destinationType.IsGenericType && destinationType.GetGenericTypeDefinition() == typeof(Nullable<>));
            }

            // Not null -- be sure it can be cast to the right type.
            return destinationType.IsInstanceOfType(value);
        }

        /// <summary>
        /// Determines whether the given value can legally be assigned to the given property.
        /// </summary>
        /// <param name="propertyName">The name of the property.</param>
        /// <param name="propertyType">The type of the property.</param>
        /// <param name="value">The value.  Null is permitted only if the property will accept it.</param>
        /// <exception cref="ArgumentException"> is thrown if <paramref name="value" /> is the wrong type for this property.</exception>
        private static void EnsureValidPropertyType(string propertyName, Type propertyType, object value)
        {
            if (!CanBeAssigned(propertyType, value))
            {
                throw new ArgumentException($"The value for property '{propertyName}' must be of type '{propertyType}'.", nameof(value));
            }
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
        /// <exception cref="ArgumentNullException">When <paramref name="instance" /> is null.</exception>
        /// <exception cref="ArgumentNullException">When <paramref name="validationContext" /> is null.</exception>
        /// <exception cref="ArgumentException">
        /// When <paramref name="instance" /> doesn't match the
        /// <see cref="ValidationContext.ObjectInstance" /> on <paramref name="validationContext" />.
        /// </exception>
        private static IEnumerable<ValidationError> GetObjectValidationErrors(
            object instance,
            ValidationContext validationContext,
            bool validateAllProperties,
            bool breakOnFirstError)
        {
            if (validationContext == null)
            {
                throw new ArgumentNullException(nameof(validationContext));
            }

            // Step 1: Validate the object properties' validation attributes.
            var propertyValidationErrors = GetObjectPropertyValidationErrors(instance, validationContext, validateAllProperties, breakOnFirstError);
            var errors = new List<ValidationError>(propertyValidationErrors);

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
                    .Select(result => new ValidationError(null, instance, result));

                errors.AddRange(validatableObjectErrors);
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
        /// <returns>A list of <see cref="ValidationError" /> instances.</returns>
        private static IEnumerable<ValidationError> GetObjectPropertyValidationErrors(
            object instance,
            ValidationContext validationContext,
            bool validateAllProperties,
            bool breakOnFirstError)
        {
            var properties = GetPropertyValues(instance, validationContext);
            var errors = new List<ValidationError>();

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
                            errors.Add(new ValidationError(reqAttr, property.Value, validationResult));
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
        /// <returns>
        /// A set of key value pairs, where the key is a validation context for the property and the value is its current value.
        /// </returns>
        /// <remarks>Ignores indexed properties.</remarks>
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
        /// <remarks>
        /// If a <see cref="RequiredAttribute" /> is found, it will be evaluated first, and if that fails,
        /// validation will abort, regardless of the <paramref name="breakOnFirstError" /> parameter value.
        /// </remarks>
        /// <param name="value">The value to pass to the validation attributes.</param>
        /// <param name="validationContext">Describes the type/member being evaluated.</param>
        /// <param name="attributes">The validation attributes to evaluate.</param>
        /// <param name="breakOnFirstError">
        /// Whether or not to break on the first validation failure. A <see cref="RequiredAttribute" /> failure will always abort with that sole failure.
        /// </param>
        /// <returns>The collection of validation errors.</returns>
        /// <exception cref="ArgumentNullException">When <paramref name="validationContext" /> is null.</exception>
        private static IEnumerable<ValidationError> GetValidationErrors(
            object value,
            ValidationContext validationContext,
            IReadOnlyCollection<ValidationAttribute> attributes,
            bool breakOnFirstError)
        {
            var errors = new List<ValidationError>();

            // Get the required validator if there is one and test it first, aborting on failure.
            ValidationError validationError;
            var required = attributes.OfType<RequiredAttribute>().FirstOrDefault();
            if (required != null)
            {
                if (!TryValidate(value, validationContext, required, out validationError))
                {
                    errors.Add(validationError);
                    return errors;
                }
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
        /// <exception cref="ArgumentNullException">When <paramref name="validationContext" /> is null.</exception>
        private static bool TryValidate(object value, ValidationContext validationContext, ValidationAttribute attribute, out ValidationError validationError)
        {
            var validationResult = attribute.GetValidationResult(value, validationContext);
            if (validationResult != ValidationResult.Success)
            {
                validationError = new ValidationError(attribute, value, validationResult);
                return false;
            }

            validationError = null;
            return true;
        }

        /// <summary>
        /// Private helper class to encapsulate a ValidationAttribute with the failed value and the user-visible
        /// target name against which it was validated.
        /// </summary>
        private class ValidationError
        {
            private readonly ValidationAttribute attribute;
            private readonly object value;
            private readonly ValidationResult result;

            public ValidationError(ValidationAttribute attribute, object value, ValidationResult validationResult)
            {
                this.attribute = attribute;
                this.value = value;
                this.result = validationResult;

                var errorCode = new ErrorCodeInfo(CoreErrorCodes.Prefix, GetErrorCode(attribute), validationResult.ErrorMessage);
                ExecutionError = new ExecutionError(errorCode, validationResult.MemberNames.FirstOrDefault());
            }

            public ExecutionError ExecutionError { get; }

            public void ThrowValidationException() => throw new ValidationException(result, attribute, value);

            private static string GetErrorCode(ValidationAttribute validationAttribute)
            {
                return validationAttribute.GetType().Name.Replace(nameof(Attribute), string.Empty);
            }
        }
    }
}

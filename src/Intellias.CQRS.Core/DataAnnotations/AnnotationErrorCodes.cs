using System;
using System.ComponentModel.DataAnnotations;
using Intellias.CQRS.Core.Results.Errors;

namespace Intellias.CQRS.Core.DataAnnotations
{
    /// <summary>
    /// Error codes produced by attributes from <see cref="Intellias.CQRS.Core.DataAnnotations"/>
    /// or <see cref="System.ComponentModel.DataAnnotations"/>.
    /// </summary>
    public static class AnnotationErrorCodes
    {
        /// <summary>
        /// Error codes prefix.
        /// </summary>
        public const string Prefix = "Annotation";

        /// <summary>
        /// Error code for <see cref="NotEmptyAttribute"/> annotation.
        /// </summary>
        public static ErrorCodeInfo NotEmpty => new ErrorCodeInfo(Prefix, nameof(NotEmpty), "Value must not be empty.");

        /// <summary>
        /// Error code for <see cref="CompareAttribute"/> annotation.
        /// </summary>
        public static ErrorCodeInfo Compare => new ErrorCodeInfo(Prefix, nameof(Compare), "Properties must be equal.");

        /// <summary>
        /// Error code for <see cref="CreditCardAttribute"/> annotation.
        /// </summary>
        public static ErrorCodeInfo CreditCard => new ErrorCodeInfo(Prefix, nameof(CreditCard), "Credit card number is invalid.");

        /// <summary>
        /// Error code for <see cref="CustomValidationAttribute"/> annotation.
        /// </summary>
        public static ErrorCodeInfo CustomValidation => new ErrorCodeInfo(Prefix, nameof(CustomValidation), "Custom validation failed.");

        /// <summary>
        /// Error code for <see cref="DataTypeAttribute"/> annotation.
        /// </summary>
        public static ErrorCodeInfo DataType => new ErrorCodeInfo(Prefix, nameof(DataType), "Clarifies PhoneNumber or Url annotations.");

        /// <summary>
        /// Error code for <see cref="EmailAddressAttribute"/> annotation.
        /// </summary>
        public static ErrorCodeInfo EmailAddress => new ErrorCodeInfo(Prefix, nameof(EmailAddress), "Email address is invalid.");

        /// <summary>
        /// Error code for <see cref="EnumDataTypeAttribute"/> annotation.
        /// </summary>
        public static ErrorCodeInfo EnumDataType => new ErrorCodeInfo(Prefix, nameof(EnumDataType), "Enum value is invalid.");

        /// <summary>
        /// Error code for <see cref="FileExtensionsAttribute"/> annotation.
        /// </summary>
        public static ErrorCodeInfo FileExtensions => new ErrorCodeInfo(Prefix, nameof(FileExtensions), "File extension is invalid.");

        /// <summary>
        /// Error code for <see cref="MaxLengthAttribute"/> annotation.
        /// </summary>
        public static ErrorCodeInfo MaxLength => new ErrorCodeInfo(Prefix, nameof(MaxLength), "Value length is greater that max length.");

        /// <summary>
        /// Error code for <see cref="MinLengthAttribute"/> annotation.
        /// </summary>
        public static ErrorCodeInfo MinLength => new ErrorCodeInfo(Prefix, nameof(MinLength), "Value length is less than min length.");

        /// <summary>
        /// Error code for <see cref="PhoneAttribute"/> annotation.
        /// </summary>
        public static ErrorCodeInfo Phone => new ErrorCodeInfo(Prefix, nameof(Phone), "Phone number is invalid.");

        /// <summary>
        /// Error code for <see cref="RangeAttribute"/> annotation.
        /// </summary>
        public static ErrorCodeInfo Range => new ErrorCodeInfo(Prefix, nameof(Range), "Value is out of range.");

        /// <summary>
        /// Error code for <see cref="RegularExpressionAttribute"/> annotation.
        /// </summary>
        public static ErrorCodeInfo RegularExpression => new ErrorCodeInfo(Prefix, nameof(RegularExpression), "Value doesn't match regular expression.");

        /// <summary>
        /// Error code for <see cref="RequiredAttribute"/> annotation.
        /// </summary>
        public static ErrorCodeInfo Required => new ErrorCodeInfo(Prefix, nameof(Required), "Value must be not null.");

        /// <summary>
        /// Error code for <see cref="StringLengthAttribute"/> annotation.
        /// </summary>
        public static ErrorCodeInfo StringLength => new ErrorCodeInfo(Prefix, nameof(StringLength), "String length is invalid.");

        /// <summary>
        /// Error code for <see cref="UrlAttribute"/> annotation.
        /// </summary>
        public static ErrorCodeInfo Url => new ErrorCodeInfo(Prefix, nameof(Url), "Url is invalid.");

        /// <summary>
        /// Returns error code from <see cref="ValidationAttribute"/> type.
        /// </summary>
        /// <param name="attributeType">Type of the validation attribute.</param>
        /// <returns>Error code.</returns>
        public static string GetErrorCodeFromAttribute(Type attributeType)
        {
            return typeof(ValidationAttribute).IsAssignableFrom(attributeType)
                ? $"{Prefix}.{attributeType.Name.Replace(nameof(Attribute), string.Empty)}"
                : string.Empty;
        }
    }
}
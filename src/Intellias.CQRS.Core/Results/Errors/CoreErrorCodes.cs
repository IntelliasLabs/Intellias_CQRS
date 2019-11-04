using System.Diagnostics.CodeAnalysis;

namespace Intellias.CQRS.Core.Results.Errors
{
    /// <summary>
    /// Common error codes collection.
    /// </summary>
    [ExcludeFromCodeCoverage]
    public static class CoreErrorCodes
    {
        /// <summary>
        /// Subdomain error codes prefix.
        /// </summary>
        public const string Prefix = nameof(Core);

        /// <summary>
        /// Validation failed.
        /// </summary>
        public static ErrorCodeInfo ValidationFailed => new ErrorCodeInfo(Prefix, nameof(ValidationFailed), "Validation failed.");

        /// <summary>
        /// Unhandled error.
        /// </summary>
        public static ErrorCodeInfo UnhandledError => new ErrorCodeInfo(Prefix, nameof(UnhandledError), "Unhandled error.");

        /// <summary>
        /// Resource unavailable.
        /// </summary>
        public static ErrorCodeInfo ResourceUnavailable => new ErrorCodeInfo(Prefix, nameof(ResourceUnavailable), "Resource unavailable.");

        /// <summary>
        /// Connection failed.
        /// </summary>
        public static ErrorCodeInfo ConnectionFailed => new ErrorCodeInfo(Prefix, nameof(ConnectionFailed), "Connection failed.");

        /// <summary>
        /// Version conflict.
        /// </summary>
        public static ErrorCodeInfo VersionConflict => new ErrorCodeInfo(Prefix, nameof(VersionConflict), "Version conflict.");

        /// <summary>
        /// Access denied.
        /// </summary>
        public static ErrorCodeInfo AccessDenied => new ErrorCodeInfo(Prefix, nameof(AccessDenied), "Access denied.");

        /// <summary>
        /// Aggregate root hasn't been found.
        /// </summary>
        public static ErrorCodeInfo AggregateRootNotFound => new ErrorCodeInfo(Prefix, nameof(AggregateRootNotFound), "Aggregate Root hasn't been found.");

        /// <summary>
        /// The mentioned name is already in use. Please enter another one.
        /// </summary>
        public static ErrorCodeInfo NameIsInUse => new ErrorCodeInfo(Prefix, nameof(NameIsInUse), "The mentioned name is already in use. Please enter another one.");

        /// <summary>
        /// The mentioned name is not in use. Please enter another one.
        /// </summary>
        public static ErrorCodeInfo NameIsNotFound => new ErrorCodeInfo(Prefix, nameof(NameIsNotFound), "The mentioned name is not in use. Please enter another one.");

        /// <summary>
        /// Reserve operation failed.
        /// </summary>
        public static ErrorCodeInfo ReserveNameFailed => new ErrorCodeInfo(Prefix, nameof(ReserveNameFailed), "Reserving name failed.");

        /// <summary>
        /// Updating name failed.
        /// </summary>
        public static ErrorCodeInfo UpdateNameFailed => new ErrorCodeInfo(Prefix, nameof(UpdateNameFailed), "Updating name failed.");

        /// <summary>
        /// Deleting name failed.
        /// </summary>
        public static ErrorCodeInfo DeleteNameFailed => new ErrorCodeInfo(Prefix, nameof(DeleteNameFailed), "Deleting name failed.");

        /// <summary>
        /// Value must not be empty.
        /// </summary>
        public static ErrorCodeInfo CantBeEmpty => new ErrorCodeInfo(Prefix, nameof(CantBeEmpty), "Value must not be empty.");

        /// <summary>
        /// Properties are not equal.
        /// </summary>
        public static ErrorCodeInfo ComparisonFailed => new ErrorCodeInfo(Prefix, nameof(ComparisonFailed), "Properties must be equal.");

        /// <summary>
        /// Credit card number is invalid.
        /// </summary>
        public static ErrorCodeInfo CreditCardNumberIsInvalid => new ErrorCodeInfo(Prefix, nameof(CreditCardNumberIsInvalid), "Credit card number is invalid.");

        /// <summary>
        /// Email address is invalid.
        /// </summary>
        public static ErrorCodeInfo EmailAddressIsInvalid => new ErrorCodeInfo(Prefix, nameof(EmailAddressIsInvalid), "Email address is invalid.");

        /// <summary>
        /// Enum value is invalid.
        /// </summary>
        public static ErrorCodeInfo EnumValueIsInvalid => new ErrorCodeInfo(Prefix, nameof(EnumValueIsInvalid), "Enum value is invalid.");

        /// <summary>
        /// File extension is invalid.
        /// </summary>
        public static ErrorCodeInfo FileExtensionIsInvalid => new ErrorCodeInfo(Prefix, nameof(FileExtensionIsInvalid), "File extension is invalid.");

        /// <summary>
        /// Value length is greater than max length.
        /// </summary>
        public static ErrorCodeInfo LengthIsGreaterThanMax => new ErrorCodeInfo(Prefix, nameof(LengthIsGreaterThanMax), "Value length is greater that max length.");

        /// <summary>
        /// Value length is less than min length.
        /// </summary>
        public static ErrorCodeInfo LengthIsLessThanMin => new ErrorCodeInfo(Prefix, nameof(LengthIsLessThanMin), "Value length is less than min length.");

        /// <summary>
        /// Phone number is invalid.
        /// </summary>
        public static ErrorCodeInfo PhoneNumberIsInvalid => new ErrorCodeInfo(Prefix, nameof(PhoneNumberIsInvalid), "Phone number is invalid.");

        /// <summary>
        /// Value is out of range.
        /// </summary>
        public static ErrorCodeInfo ValueIsOutOfRange => new ErrorCodeInfo(Prefix, nameof(ValueIsOutOfRange), "Value is out of range.");

        /// <summary>
        /// Value doesn't match regular expression.
        /// </summary>
        public static ErrorCodeInfo ValueDoesntMatchRegularExpression => new ErrorCodeInfo(Prefix, nameof(ValueDoesntMatchRegularExpression), "Value doesn't match regular expression.");

        /// <summary>
        /// Value must be not null.
        /// </summary>
        public static ErrorCodeInfo ValueIsRequired => new ErrorCodeInfo(Prefix, nameof(ValueIsRequired), "Value must be not null.");

        /// <summary>
        /// String length is invalid.
        /// </summary>
        public static ErrorCodeInfo StringLengthIsInvalid => new ErrorCodeInfo(Prefix, nameof(StringLengthIsInvalid), "String length is invalid.");

        /// <summary>
        /// Url is invalid.
        /// </summary>
        public static ErrorCodeInfo UrlIsInvalid => new ErrorCodeInfo(Prefix, nameof(UrlIsInvalid), "Url is invalid.");
    }
}

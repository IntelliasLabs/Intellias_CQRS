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
        public static readonly ErrorCodeInfo ValidationFailed = new ErrorCodeInfo(Prefix, nameof(ValidationFailed), "Validation failed.");

        /// <summary>
        /// Unhandled error.
        /// </summary>
        public static readonly ErrorCodeInfo UnhandledError = new ErrorCodeInfo(Prefix, nameof(UnhandledError), "Unhandled error.");

        /// <summary>
        /// Resource unavailable.
        /// </summary>
        public static readonly ErrorCodeInfo ResourceUnavailable = new ErrorCodeInfo(Prefix, nameof(ResourceUnavailable), "Resource unavailable.");

        /// <summary>
        /// Connection failed.
        /// </summary>
        public static readonly ErrorCodeInfo ConnectionFailed = new ErrorCodeInfo(Prefix, nameof(ConnectionFailed), "Connection failed.");

        /// <summary>
        /// Version conflict.
        /// </summary>
        public static readonly ErrorCodeInfo VersionConflict = new ErrorCodeInfo(Prefix, nameof(VersionConflict), "Version conflict.");

        /// <summary>
        /// Access denied.
        /// </summary>
        public static readonly ErrorCodeInfo AccessDenied = new ErrorCodeInfo(Prefix, nameof(AccessDenied), "Access denied.");

        /// <summary>
        /// Value has invalid format.
        /// </summary>
        public static readonly ErrorCodeInfo InvalidFormat = new ErrorCodeInfo(Prefix, nameof(InvalidFormat), "Entry has invalid format.");

        /// <summary>
        /// Value is not found.
        /// </summary>
        public static readonly ErrorCodeInfo NotFound = new ErrorCodeInfo(Prefix, nameof(NotFound), "Entry is not found.");

        /// <summary>
        /// Aggregate root hasn't been found.
        /// </summary>
        public static readonly ErrorCodeInfo AggregateRootNotFound = new ErrorCodeInfo(Prefix, nameof(AggregateRootNotFound), "Aggregate Root hasn't been found.");

        /// <summary>
        /// The mentioned name is already in use. Please enter another one.
        /// </summary>
        public static readonly ErrorCodeInfo NameIsInUse = new ErrorCodeInfo(Prefix, nameof(NameIsInUse), "The mentioned name is already in use. Please enter another one.");

        /// <summary>
        /// The mentioned name is not in use. Please enter another one.
        /// </summary>
        public static readonly ErrorCodeInfo NameIsNotFound = new ErrorCodeInfo(Prefix, nameof(NameIsNotFound), "The mentioned name is not in use. Please enter another one.");

        /// <summary>
        /// Reserve operation failed.
        /// </summary>
        public static readonly ErrorCodeInfo ReserveNameFailed = new ErrorCodeInfo(Prefix, nameof(ReserveNameFailed), "Reserving name failed.");

        /// <summary>
        /// Updating name failed.
        /// </summary>
        public static readonly ErrorCodeInfo UpdateNameFailed = new ErrorCodeInfo(Prefix, nameof(UpdateNameFailed), "Updating name failed.");

        /// <summary>
        /// Deleting name failed.
        /// </summary>
        public static readonly ErrorCodeInfo DeleteNameFailed = new ErrorCodeInfo(Prefix, nameof(DeleteNameFailed), "Deleting name failed.");
    }
}

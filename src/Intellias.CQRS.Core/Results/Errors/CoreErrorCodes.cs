using System.Diagnostics.CodeAnalysis;

namespace Intellias.CQRS.Core.Results.Errors
{
    /// <summary>
    /// Common error codes collection.
    /// </summary>
    [ExcludeFromCodeCoverage]
    public static class CoreErrorCodes
    {
        private const string Prefix = nameof(Core);

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
        /// Agregate root hasn't been found.
        /// </summary>
        public static ErrorCodeInfo AggregateRootNotFound => new ErrorCodeInfo(Prefix, nameof(AggregateRootNotFound), "Agregate Root hasn't been found.");
    }
}

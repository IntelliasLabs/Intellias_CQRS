namespace Intellias.CQRS.Core.Results
{
    /// <summary>
    /// Common error codes.
    /// </summary>
    public static class ErrorCodes
    {
        /// <summary>
        /// ValidationFailed.
        /// </summary>
        public static string ValidationFailed => nameof(ValidationFailed);

        /// <summary>
        /// UnhandledError.
        /// </summary>
        public static string UnhandledError => nameof(UnhandledError);

        /// <summary>
        /// Resource Unavailable.
        /// </summary>
        public static string ResourceUnavailable => nameof(ResourceUnavailable);

        /// <summary>
        /// Connection Failed.
        /// </summary>
        public static string ConnectionFailed => nameof(ConnectionFailed);

        /// <summary>
        /// Version Conflict.
        /// </summary>
        public static string VersionConflict => nameof(VersionConflict);

        /// <summary>
        /// Access Denied.
        /// </summary>
        public static string AccessDenied => nameof(AccessDenied);
    }
}

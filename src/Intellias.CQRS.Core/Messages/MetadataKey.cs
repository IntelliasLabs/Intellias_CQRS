﻿namespace Intellias.CQRS.Core.Messages
{
    /// <summary>
    /// Metadata key.
    /// </summary>
    public enum MetadataKey
    {
        /// <summary>
        /// TypeName of Agreegate Root, should be set by API gateway
        /// </summary>
        AgreegateType,

        /// <summary>
        /// User identity, should be set by API gateway
        /// </summary>
        UserId,

        /// <summary>
        /// User roles metadata, should be set by API gateway
        /// </summary>
        Roles,

        /// <summary>
        /// HTTP request headers of command, should be set by API gateway
        /// </summary>
        RequestHeaders,

        /// <summary>
        /// Message additional payload. Could be set by API gateway for additional parameters passing, etc.
        /// </summary>
        Payload
    }
}

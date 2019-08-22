using System;

namespace Intellias.CQRS.Core.Queries.Mutable
{
    /// <inheritdoc />
    public abstract class BaseMutableQueryModel : IMutableQueryModel
    {
        /// <inheritdoc />
        public string Id { get; set; } = string.Empty;

        /// <inheritdoc />
        public string ETag { get; set; } = string.Empty;

        /// <inheritdoc />
        public DateTimeOffset Timestamp { get; set; } = DateTimeOffset.UtcNow;

        /// <inheritdoc />
        public AppliedEvent AppliedEvent { get; set; } = AppliedEvent.Empty;
    }
}
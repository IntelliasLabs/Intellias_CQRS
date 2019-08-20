using System;

namespace Intellias.CQRS.Core.Queries.Immutable
{
    /// <inheritdoc />
    public class BaseImmutableQueryModel : IImmutableQueryModel
    {
        /// <inheritdoc />
        public string Id { get; set; } = string.Empty;

        /// <inheritdoc />
        public int Version { get; set; }

        /// <inheritdoc />
        public DateTimeOffset Timestamp { get; set; } = DateTimeOffset.UtcNow;

        /// <inheritdoc />
        public AppliedEvent AppliedEvent { get; set; } = AppliedEvent.Empty;
    }
}
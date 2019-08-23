using System;

namespace Intellias.CQRS.Core.Queries.Immutable
{
    /// <summary>
    /// Base abstraction for query models that represent the state snapshot.
    /// </summary>
    public interface IImmutableQueryModel
    {
        /// <summary>
        /// Query model id.
        /// </summary>
        string Id { get; set; }

        /// <summary>
        /// Query model version.
        /// </summary>
        int Version { get; set; }

        /// <summary>
        /// Last time query model is updated.
        /// </summary>
        DateTimeOffset Timestamp { get; set; }

        /// <summary>
        /// Last applied event to query model. Empty if query model is just created.
        /// </summary>
        AppliedEvent AppliedEvent { get; set; }
    }
}
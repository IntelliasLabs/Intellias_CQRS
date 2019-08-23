using System;

namespace Intellias.CQRS.Core.Queries.Mutable
{
    /// <summary>
    /// Base abstraction for query models that contain only latest state.
    /// </summary>
    public interface IMutableQueryModel
    {
        /// <summary>
        /// Query model id.
        /// </summary>
        string Id { get; set; }

        /// <summary>
        /// Query model concurrency check tag.
        /// </summary>
        string ETag { get; set; }

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
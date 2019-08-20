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
        string Id { get; }

        /// <summary>
        /// Last time query model is updated.
        /// </summary>
        DateTimeOffset Timestamp { get; }

        /// <summary>
        /// Last applied event to query model. Empty if query model is just created.
        /// </summary>
        AppliedEvent AppliedEvent { get; }
    }
}
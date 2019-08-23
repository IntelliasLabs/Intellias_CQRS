using System;

namespace Intellias.CQRS.Core.Queries
{
    /// <summary>
    /// Last IIntegrationEvent that updated query model.
    /// </summary>
    public class AppliedEvent
    {
        /// <summary>
        /// No event constant.
        /// </summary>
        public static readonly AppliedEvent Empty = new AppliedEvent(string.Empty, DateTimeOffset.MinValue);

        /// <summary>
        /// Initializes a new instance of the <see cref="AppliedEvent"/> class.
        /// </summary>
        /// <param name="id">Value for <see cref="Id"/>.</param>
        /// <param name="created">Value for <see cref="Created"/>.</param>
        public AppliedEvent(string id, DateTimeOffset created)
        {
            Id = id;
            Created = created;
        }

        /// <summary>
        /// Integration Event id.
        /// </summary>
        public string Id { get; }

        /// <summary>
        /// Integration Event creation timestamp.
        /// </summary>
        public DateTimeOffset Created { get; }
    }
}
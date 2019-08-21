using System;

namespace Intellias.CQRS.Core.Queries
{
    /// <summary>
    /// Read model interface.
    /// </summary>
    public interface IQueryModel
    {
        /// <summary>
        /// Generic Id of read model.
        /// </summary>
        string Id { get; set; }

        /// <summary>
        /// Version of Query Model.
        /// </summary>
        int Version { get; set; }

        /// <summary>
        /// Timestamp of last query item update.
        /// </summary>
        DateTime Timestamp { get; set; }
    }
}

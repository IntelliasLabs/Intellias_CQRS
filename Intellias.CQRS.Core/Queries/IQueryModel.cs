using Intellias.CQRS.Core.Messages;

namespace Intellias.CQRS.Core.Queries
{
    /// <summary>
    /// Read model interface
    /// </summary>
    public interface IQueryModel : IIdentified
    {
        /// <summary>
        /// Version of Query Model
        /// </summary>
        int Version { get; set; }
    }
}

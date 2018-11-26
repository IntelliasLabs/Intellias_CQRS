namespace Intellias.CQRS.Core.Queries
{
    /// <summary>
    /// Read model interface
    /// </summary>
    public interface IQueryModel
    {
        /// <summary>
        /// Generic Id of read model
        /// </summary>
        string Id { get; set; }
    }
}

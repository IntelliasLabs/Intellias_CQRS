namespace Intellias.CQRS.Core.Queries
{
    /// <summary>
    /// Read model interface
    /// </summary>
    internal interface IQueryModel
    {
        /// <summary>
        /// Generic Id of read model
        /// </summary>
        string Id { get; set; }
    }
}

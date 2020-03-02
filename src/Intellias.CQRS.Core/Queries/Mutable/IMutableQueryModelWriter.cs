using System.Threading.Tasks;

namespace Intellias.CQRS.Core.Queries.Mutable
{
    /// <summary>
    /// Modifies mutable query models of type <typeparamref name="TQueryModel"/>.
    /// </summary>
    /// <typeparam name="TQueryModel">Type of the query model.</typeparam>
    public interface IMutableQueryModelWriter<TQueryModel>
        where TQueryModel : IMutableQueryModel, new()
    {
        /// <summary>
        /// Creates query model.
        /// </summary>
        /// <param name="model">Query model to be created.</param>
        /// <returns>Created query model.</returns>
        Task<TQueryModel> CreateAsync(TQueryModel model);

        /// <summary>
        /// Replaces query model with the new updated version.
        /// </summary>
        /// <param name="model">Query model to be created.</param>
        /// <returns>Stored query model.</returns>
        Task<TQueryModel> ReplaceAsync(TQueryModel model);

        /// <summary>
        /// Deletes query model.
        /// </summary>
        /// <param name="id">Query model id.</param>
        /// <returns>Deleted query model.</returns>
        Task DeleteAsync(string id);
    }
}
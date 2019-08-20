using System.Threading.Tasks;

namespace Intellias.CQRS.Core.Queries.Mutable
{
    /// <summary>
    /// Modifies mutable query models of type <typeparamref name="TQueryModel"/>.
    /// </summary>
    /// <typeparam name="TQueryModel">Type of the query model.</typeparam>
    public interface IMutableQueryModelWriter<in TQueryModel>
        where TQueryModel : IMutableQueryModel, new()
    {
        /// <summary>
        /// Creates query model.
        /// </summary>
        /// <param name="model">Query model to be created.</param>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
        Task CreateAsync(TQueryModel model);

        /// <summary>
        /// Replaces query model with the new updated version.
        /// </summary>
        /// <param name="model">Query model to be created.</param>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
        Task ReplaceAsync(TQueryModel model);
    }
}
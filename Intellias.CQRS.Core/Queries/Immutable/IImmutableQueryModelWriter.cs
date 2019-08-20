using System.Threading.Tasks;

namespace Intellias.CQRS.Core.Queries.Immutable
{
    /// <summary>
    /// Modifies immutable query models of type <typeparamref name="TQueryModel"/>.
    /// </summary>
    /// <typeparam name="TQueryModel">Type of the query model.</typeparam>
    public interface IImmutableQueryModelWriter<in TQueryModel>
        where TQueryModel : IImmutableQueryModel, new()
    {
        /// <summary>
        /// Creates query model.
        /// </summary>
        /// <param name="model">Query model to be created.</param>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
        Task CreateAsync(TQueryModel model);
    }
}
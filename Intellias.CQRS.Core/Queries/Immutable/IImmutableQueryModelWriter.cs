using System.Threading.Tasks;

namespace Intellias.CQRS.Core.Queries.Immutable
{
    /// <summary>
    /// Modifies immutable query models of type <typeparamref name="TQueryModel"/>.
    /// </summary>
    /// <typeparam name="TQueryModel">Type of the query model.</typeparam>
    public interface IImmutableQueryModelWriter<TQueryModel>
        where TQueryModel : IImmutableQueryModel, new()
    {
        /// <summary>
        /// Creates query model.
        /// </summary>
        /// <param name="model">Query model to be created.</param>
        /// <returns>Created query model.</returns>
        Task<TQueryModel> CreateAsync(TQueryModel model);
    }
}
using System.Threading.Tasks;
using Intellias.CQRS.Core.Queries.Immutable;

namespace Intellias.CQRS.QueryStore.AzureTable.Immutable
{
    /// <summary>
    /// Azure Storage Account Table storage for <see cref="IImmutableQueryModel"/>.
    /// </summary>
    /// <typeparam name="TQueryModel">Type of the query model.</typeparam>
    public class ImmutableQueryModelTableStorage<TQueryModel> :
        IImmutableQueryModelReader<TQueryModel>,
        IImmutableQueryModelWriter<TQueryModel>
        where TQueryModel : IImmutableQueryModel, new()
    {
        /// <inheritdoc />
        public Task<TQueryModel> FindAsync(string id, int version) => throw new System.NotImplementedException();

        /// <inheritdoc />
        public Task<TQueryModel> GetAsync(string id, int version) => throw new System.NotImplementedException();

        /// <inheritdoc />
        public Task<TQueryModel> GetLatestAsync(string id) => throw new System.NotImplementedException();

        /// <inheritdoc />
        public Task<TQueryModel> CreateAsync(TQueryModel model) => throw new System.NotImplementedException();
    }
}
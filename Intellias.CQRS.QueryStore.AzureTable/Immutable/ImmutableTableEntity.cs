using System.Globalization;
using Intellias.CQRS.Core.Queries.Immutable;
using Intellias.CQRS.QueryStore.AzureTable.Common;

namespace Intellias.CQRS.QueryStore.AzureTable.Immutable
{
    /// <summary>
    /// Table entity that stores <see cref="IImmutableQueryModel"/>.
    /// </summary>
    /// <typeparam name="TQueryModel">Query model type.</typeparam>
    public class ImmutableTableEntity<TQueryModel> : BaseJsonTableEntity<TQueryModel>
        where TQueryModel : class, IImmutableQueryModel
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ImmutableTableEntity{TQueryModel}"/> class.
        /// </summary>
        public ImmutableTableEntity()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ImmutableTableEntity{TQueryModel}"/> class.
        /// </summary>
        /// <param name="queryModel">Query model to be stored.</param>
        public ImmutableTableEntity(TQueryModel queryModel)
            : base(queryModel, true)
        {
            PartitionKey = queryModel.Id;
            RowKey = GetRowKey(queryModel.Version);
        }

        /// <inheritdoc />
        protected override void SetupDeserializedData(TQueryModel data)
        {
            data.Timestamp = Timestamp;
        }

        private static string GetRowKey(int version)
        {
            // Build from Created row key that stores data in reverse order.
            return string.Format(CultureInfo.InvariantCulture, "{0:D20}", int.MaxValue - version);
        }
    }
}
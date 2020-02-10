using Intellias.CQRS.Core.Queries.Mutable;
using Intellias.CQRS.Persistence.AzureStorage.Common;

namespace Intellias.CQRS.QueryStore.AzureTable.Mutable
{
    /// <summary>
    /// Table entity that stores <see cref="IMutableQueryModel"/>.
    /// </summary>
    /// <typeparam name="TQueryModel">Query model type.</typeparam>
    public class MutableTableEntity<TQueryModel> : BaseJsonTableEntity<TQueryModel>
        where TQueryModel : class, IMutableQueryModel
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="MutableTableEntity{TQueryModel}"/> class.
        /// </summary>
        public MutableTableEntity()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="MutableTableEntity{TQueryModel}"/> class.
        /// </summary>
        /// <param name="queryModel">Query model to be stored.</param>
        public MutableTableEntity(TQueryModel queryModel)
            : base(queryModel, true)
        {
            PartitionKey = typeof(TQueryModel).Name;
            RowKey = queryModel.Id;
            ETag = string.IsNullOrWhiteSpace(queryModel.ETag) ? "*" : queryModel.ETag;
        }

        /// <inheritdoc />
        protected override void SetupDeserializedData(TQueryModel data)
        {
            data.Timestamp = Timestamp;
            data.ETag = ETag;
        }
    }
}
namespace Intellias.CQRS.Core.Queries
{
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="TQueryModel"></typeparam>
    public class ReadAllQuery<TQueryModel> : IQuery<CollectionQueryModel<TQueryModel>>
        where TQueryModel : class, IQueryModel
    {
    }
}

namespace Intellias.CQRS.Core.Queries
{
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="TReadModel"></typeparam>
    public class ReadAllQuery<TReadModel> : IQuery<CollectionReadModel<TReadModel>>
        where TReadModel : class, IReadModel
    {
    }
}

using System.Collections.Generic;

namespace Intellias.CQRS.Core.Queries
{
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="TReadModel"></typeparam>
    public class ReadAllQuery<TReadModel> : IQuery<IReadOnlyCollection<TReadModel>>
        where TReadModel : class, IReadModel
    {
    }
}

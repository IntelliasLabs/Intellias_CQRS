using System;

namespace Intellias.CQRS.Core.Queries
{
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="TReadModel"></typeparam>
    public class ReadModelByIdQuery<TReadModel> : IQuery<TReadModel>
        where TReadModel : class, IReadModel
    {
        /// <summary>
        /// 
        /// </summary>
        public string Id { get; }

        /// <summary>
        ///
        /// </summary>
        /// <param name="id"></param>
        public ReadModelByIdQuery(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                throw new ArgumentNullException(nameof(id));
            }

            Id = id;
        }
    }
}

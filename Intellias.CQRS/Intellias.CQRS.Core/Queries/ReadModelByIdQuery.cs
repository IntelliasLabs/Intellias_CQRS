using System;

namespace Intellias.CQRS.Core.Queries
{
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="TQueryModel"></typeparam>
    public class ReadModelByIdQuery<TQueryModel> : IQuery<TQueryModel>
        where TQueryModel : class, IQueryModel
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

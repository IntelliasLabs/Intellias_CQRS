namespace Intellias.CQRS.Core.Queries
{
    /// <summary>
    /// Query abstraction
    /// </summary>
    public interface IQuery
    {
    }

    /// <summary>
    /// Generic Query abstraction
    /// </summary>
    /// <typeparam name="TResult">Query result</typeparam>
    public interface IQuery<TResult> : IQuery
    {
    }
}

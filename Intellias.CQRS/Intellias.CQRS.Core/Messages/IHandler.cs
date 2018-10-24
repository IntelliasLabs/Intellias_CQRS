namespace Intellias.CQRS.Core.Messages
{
    /// <summary>
    /// Defines a handler for a message.
    /// </summary>
    /// <typeparam name="T">Message type being handled</typeparam>
    public interface IHandler<in T> where T : IMessage
    {
    }
}

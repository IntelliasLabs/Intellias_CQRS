namespace Intellias.CQRS.Core.Messages
{
    /// <inheritdoc />
    public abstract class AbstractMessage : IMessage
    {
        /// <summary>
        /// Constructs abstract message
        /// </summary>
        protected AbstractMessage()
        {
            Id = Unified.NewCode();
        }

        /// <inheritdoc />
        public string Id { get; protected set; }
    }
}

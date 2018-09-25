using System.Threading.Tasks;
using Product.Domain.Core.Messages;

namespace Product.Domain.Core.Commands
{
#pragma warning disable SA1629 // Documentation text should end with a period
                              /// <summary>
                              /// ICommandHandler interface
                              /// </summary>
                              /// <typeparam name="T"></typeparam>
    public interface ICommandHandler<T> : IHandler<T, ICommandResult>
#pragma warning restore SA1629 // Documentation text should end with a period
        where T : ICommand
    {
        /// <summary>
        /// Handle message.
        /// </summary>
        /// <param name="message">abstract message.</param>
        /// <returns>async task awaiter.</returns>
        new Task<ICommandResult> Handle(T message);
    }
}

using Product.Domain.Core.Messages;
using System.Threading.Tasks;

namespace Product.Domain.Core.Commands
{
    public interface ICommandHandler<T> : IHandler<T, ICommandResult> where T : ICommand
    {
        /// <summary>
        /// Handle message
        /// </summary>
        /// <param name="message">abstract message</param>
        /// <returns>async task awaiter</returns>
        new Task<ICommandResult> Handle(T message);
    }
}

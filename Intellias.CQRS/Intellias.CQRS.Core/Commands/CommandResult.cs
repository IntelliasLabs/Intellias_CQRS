using Product.Domain.Core.Messages;
using System;
using System.Collections.Generic;
using System.Text;

namespace Product.Domain.Core.Commands
{
    public class CommandResult : ICommandResult
    {
        private CommandResult() { }

        private CommandResult(string failureReason)
        {
            FailureReason = failureReason;
        }

        public string FailureReason { get; }
        public bool IsSuccess => string.IsNullOrEmpty(FailureReason);

        public static CommandResult Success { get; } = new CommandResult();

        public static CommandResult Fail(string reason)
        {
            return new CommandResult(reason);
        }

        public static implicit operator bool(CommandResult result)
        {
            return result.IsSuccess;
        }
    }
}

using System.Collections.Generic;
using System.Linq;
using FluentAssertions.Equivalency;
using Intellias.CQRS.Core.Messages;

namespace Intellias.CQRS.Tests.Core.Infrastructure.AssertionRules
{
    /// <summary>
    /// Selection rule for comparing signals.
    /// </summary>
    public class SignalSelectionRule : IMemberSelectionRule
    {
        /// <inheritdoc />
        public bool IncludesMembers => false;

        /// <inheritdoc />
        public IEnumerable<SelectedMemberInfo> SelectMembers(
            IEnumerable<SelectedMemberInfo> selectedMembers,
            IMemberInfo context,
            IEquivalencyAssertionOptions config)
        {
            return selectedMembers.Where(m => m.Name != nameof(IMessage.Id) && m.Name != nameof(IMessage.Created));
        }
    }
}
using System.Collections.Generic;
using System.Linq;
using FluentAssertions.Equivalency;
using Intellias.CQRS.Core.Events;

namespace Intellias.CQRS.Tests.Utils.AssertionRules
{
    public class IntegrationEventSelectionRule : IMemberSelectionRule
    {
        public bool IncludesMembers => false;

        public IEnumerable<SelectedMemberInfo> SelectMembers(
            IEnumerable<SelectedMemberInfo> selectedMembers,
            IMemberInfo context,
            IEquivalencyAssertionOptions config)
        {
            return selectedMembers.Where(m => m.Name != nameof(IIntegrationEvent.Id) && m.Name != nameof(IIntegrationEvent.Created));
        }
    }
}
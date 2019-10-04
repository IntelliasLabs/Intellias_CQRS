using System.Collections.Generic;
using System.Linq;
using FluentAssertions.Equivalency;
using Intellias.CQRS.Core.Events;

namespace Intellias.CQRS.Tests.Core.Infrastructure.AssertionRules
{
    /// <summary>
    /// Selection rule for comparing <see cref="IIntegrationEvent"/>.
    /// </summary>
    public class IntegrationEventSelectionRule : IMemberSelectionRule
    {
        /// <inheritdoc />
        public bool IncludesMembers => false;

        /// <inheritdoc />
        public IEnumerable<SelectedMemberInfo> SelectMembers(
            IEnumerable<SelectedMemberInfo> selectedMembers,
            IMemberInfo context,
            IEquivalencyAssertionOptions config)
        {
            return selectedMembers.Where(m => m.Name != nameof(IIntegrationEvent.Id) && m.Name != nameof(IIntegrationEvent.Created));
        }
    }
}
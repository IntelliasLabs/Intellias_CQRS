using System.Collections.Generic;
using System.Linq;
using FluentAssertions.Equivalency;
using Intellias.CQRS.Core.Queries.Immutable;

namespace Intellias.CQRS.Tests.Core.Infrastructure.AssertionRules
{
    /// <summary>
    /// Selection rule for comparing <see cref="IImmutableQueryModel"/>.
    /// </summary>
    public class ImmutableQueryModelSelectionRule : IMemberSelectionRule
    {
        /// <inheritdoc />
        public bool IncludesMembers => false;

        /// <inheritdoc />
        public IEnumerable<SelectedMemberInfo> SelectMembers(
            IEnumerable<SelectedMemberInfo> selectedMembers,
            IMemberInfo context,
            IEquivalencyAssertionOptions config)
        {
            return selectedMembers.Where(m => m.Name != nameof(IImmutableQueryModel.Timestamp));
        }
    }
}
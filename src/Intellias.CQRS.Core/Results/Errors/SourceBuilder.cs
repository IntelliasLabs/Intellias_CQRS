using System;
using System.Linq.Expressions;
using Intellias.CQRS.Core.Commands;
using Intellias.CQRS.Core.Messages;

namespace Intellias.CQRS.Core.Results.Errors
{
    /// <summary>
    /// Class for building error source.
    /// </summary>
    public static class SourceBuilder
    {
        /// <summary>
        /// Creates errors source from property path.
        /// </summary>
        /// <typeparam name="TCommand">Type of Command.</typeparam>
        /// <param name="getProperty">Get property expression.</param>
        /// <returns>Source.</returns>
        public static string BuildErrorSource<TCommand>(Expression<Func<TCommand, object>> getProperty)
           where TCommand : ICommand
        {
            // If convertion to 'object' is needed -> skip this expression.
            var currentExpression = getProperty.Body.NodeType == ExpressionType.Convert
                ? ((UnaryExpression)getProperty.Body).Operand
                : getProperty.Body;

            var source = string.Empty;

            while (currentExpression.NodeType != ExpressionType.Parameter)
            {
                // c => c.Requirements[0].Version --- just get 'Version' field.
                if (currentExpression.NodeType == ExpressionType.MemberAccess)
                {
                    var propertyExpression = (MemberExpression)currentExpression;
                    source = $"{propertyExpression.Member.Name}.{source}";

                    currentExpression = propertyExpression.Expression;
                    continue;
                }

                // c => c.Requirements[0].Version --- for getting index from collection.
                if (currentExpression.NodeType == ExpressionType.ArrayIndex)
                {
                    var arrayExpression = (BinaryExpression)currentExpression;

                    currentExpression = arrayExpression.Left;
                    continue;
                }

                // If we need internal convertion from 'Command' to Imessage that contains fields like 'AggregateRootId', 'CorrelationId' etc.
                if (currentExpression.NodeType == ExpressionType.Convert && currentExpression.Type == typeof(IMessage))
                {
                    break;
                }

                throw new InvalidOperationException("We should use only member and arrayIndex expresions during building expression.");
            }

            var commandTypeName = typeof(TCommand).Name;
            source = $"{commandTypeName}.{source}";

            // Delete the last '.' symbol at the end of string.
            return source.Remove(source.Length - 1);
        }
    }
}

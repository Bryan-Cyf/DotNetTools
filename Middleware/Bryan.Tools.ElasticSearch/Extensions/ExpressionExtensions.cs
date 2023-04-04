using System.Linq.Expressions;

namespace Tools.Elastic
{
    /// <summary>
    ///     Extension methods for Expression objects.
    /// </summary>
    internal static class ExpressionExtensions
    {
        /// <summary>
        ///     Remove all the outer quotes from an expression.
        /// </summary>
        /// <param name="expression">Expression that might contain outer quotes.</param>
        /// <returns>Expression that no longer contains outer quotes.</returns>
        private static Expression StripQuotes(this Expression expression)
        {
            while (expression.NodeType == ExpressionType.Quote)
                expression = ((UnaryExpression) expression).Operand;
            return expression;
        }

        /// <summary>
        ///     Get the lambda for an expression stripping any necessary outer quotes.
        /// </summary>
        /// <param name="expression">
        ///     Expression that should be a lamba possibly wrapped
        ///     in outer quotes.
        /// </param>
        /// <returns>LambdaExpression no longer wrapped in quotes.</returns>
        public static LambdaExpression GetLambda(this Expression expression)
        {
            return (LambdaExpression) expression.StripQuotes();
        }
    }
}
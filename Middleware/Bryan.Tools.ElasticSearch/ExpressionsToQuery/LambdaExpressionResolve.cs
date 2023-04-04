using System.Linq.Expressions;

namespace Tools.Elastic
{
    public class LambdaExpressionResolve : BaseResolve
    {
        public LambdaExpressionResolve(ExpressionParameter parameter) : base(parameter)
        {
            var lambda = Expression as LambdaExpression;
            var expression = lambda.Body;
            Expression = expression;
            Start();
        }
    }
}
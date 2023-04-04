using System.Linq.Expressions;

namespace Tools.Elastic
{
    public class BaseResolve
    {
        public BaseResolve(ExpressionParameter parameter)
        {
            Expression = parameter.CurrentExpression;
            Context = parameter.Context;
            BaseParameter = parameter;
        }

        protected Expression Expression { get; set; }
        private Expression ExactExpression { get; set; }

        protected ExpressionContext Context { get; set; }
        protected bool? IsLeft { get; set; }

        private ExpressionParameter BaseParameter { get; }

        public BaseResolve Start()
        {
            var expression = Expression;
            var parameter = new ExpressionParameter
            {
                Context = Context,
                CurrentExpression = expression,
                BaseExpression = ExactExpression,
                BaseParameter = BaseParameter
            };
            return expression switch
            {
                //else if (expression is BinaryExpression && expression.NodeType == ExpressionType.Coalesce)
                //{
                //    return new CoalesceResolveItems(parameter);
                //}
                LambdaExpression _ => new LambdaExpressionResolve(parameter),
                //else if (expression is BlockExpression)
                //{
                //    Check.ThrowNotSupportedException("BlockExpression");
                //}
                //else if (expression is ConditionalExpression)
                //{
                //    return new ConditionalExpressionResolve(parameter);
                //}
                BinaryExpression _ => new BinaryExpressionResolve(parameter),
                //else if (expression is MemberExpression && ((MemberExpression)expression).Expression == null)
                //{
                //    return new MemberNoExpressionResolve(parameter);
                //}
                MethodCallExpression _ => new MethodCallExpressionResolve(parameter),
                //else if (expression is MemberExpression && ((MemberExpression)expression).Expression.NodeType == ExpressionType.New)
                //{
                //    return new MemberNewExpressionResolve(parameter);
                //}
                MemberExpression memberExpression when memberExpression.Expression.NodeType == ExpressionType.Constant => new MemberConstExpressionResolve(parameter),
                ConstantExpression _ => new ConstantExpressionResolve(parameter),
                MemberExpression _ => new MemberExpressionResolve(parameter),
                _ => null
            };
        }
    }
}
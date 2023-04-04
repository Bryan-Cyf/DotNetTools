using System.Linq.Expressions;
using Nest;

namespace Tools.Elastic
{
    public class ExpressionsGetQuery
    {
        public static QueryContainer GetQuery(Expression expression, MappingIndex mappingIndex)
        {
            var parameter = new ExpressionParameter {CurrentExpression = expression, Context = new ExpressionContext(mappingIndex)};
            new BaseResolve(parameter).Start();
            return parameter.Context.QueryContainer;
        }
    }
}
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;
namespace Tools
{
    public static class ExpressionHelper
    {
        private static Expression<T> Combine<T>(this Expression<T> first, Expression<T> second, Func<Expression, Expression, Expression> merge)
        {
            MyExpressionVisitor visitor = new MyExpressionVisitor(first.Parameters[0]);
            Expression bodyone = visitor.Visit(first.Body);
            Expression bodytwo = visitor.Visit(second.Body);
            return Expression.Lambda<T>(merge(bodyone, bodytwo), first.Parameters[0]);
        }

        /// <summary>
        /// &#038;
        /// </summary>
        public static Expression<Func<T, bool>> And<T>(this Expression<Func<T, bool>> first, Expression<Func<T, bool>> second)
        {
            return first.Combine(second, Expression.And);
        }

        /// <summary>
        /// and
        /// </summary>
        public static Expression<Func<T, bool>> AndAlso<T>(this Expression<Func<T, bool>> first, Expression<Func<T, bool>> second)
        {
            return first.Combine(second, Expression.AndAlso);
        }

        /// <summary>
        /// or
        /// </summary>
        public static Expression<Func<T, bool>> Or<T>(this Expression<Func<T, bool>> first, Expression<Func<T, bool>> second)
        {
            return first.Combine(second, Expression.Or);
        }

        public class MyExpressionVisitor : ExpressionVisitor
        {
            public ParameterExpression _Parameter { get; set; }

            public MyExpressionVisitor(ParameterExpression Parameter)
            {
                _Parameter = Parameter;
            }
            protected override Expression VisitParameter(ParameterExpression p)
            {
                return _Parameter;
            }

            public override Expression Visit(Expression node)
            {
                return base.Visit(node);//Visit会根据VisitParameter()方法返回的Expression修改这里的node变量
            }
        }
    }
}

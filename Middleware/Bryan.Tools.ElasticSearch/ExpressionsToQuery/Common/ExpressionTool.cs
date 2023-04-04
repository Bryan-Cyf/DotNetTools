using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;
using Nest;

namespace Tools.Elastic
{
    public class ExpressionTool
    {
        public static bool IsOperator(ExpressionType expressiontype)
        {
            return expressiontype == ExpressionType.And || expressiontype == ExpressionType.AndAlso
                                                        || expressiontype == ExpressionType.Or || expressiontype == ExpressionType.OrElse;
        }

        public static QueryBase GetOperator(ExpressionType expressiontype)
        {
            switch (expressiontype)
            {
                case ExpressionType.Equal:
                    return new TermQuery();
                case ExpressionType.GreaterThan:
                    return new TermRangeQuery
                    {
                        GreaterThan = "Y"
                    };
                case ExpressionType.GreaterThanOrEqual:
                    return new TermRangeQuery
                    {
                        GreaterThanOrEqualTo = "Y"
                    };
                case ExpressionType.LessThan:
                    return new TermRangeQuery
                    {
                        LessThan = "Y"
                    };
                case ExpressionType.LessThanOrEqual:
                    return new TermRangeQuery
                    {
                        LessThanOrEqualTo = "Y"
                    };
                case ExpressionType.NotEqual:
                    return new BoolQuery();

                default:
                    return null;
            }
        }

        public static object GetValue(object value)
        {
            if (value == null) return null;
            var type = value.GetType();
            return type.GetTypeInfo().IsEnum ? Convert.ToInt64(value) : value;
        }

        public static object GetMemberValue(MemberInfo member, Expression expression)
        {
            var rootExpression = expression as MemberExpression;
            var memberInfos = new Stack<MemberInfo>();
            var fieldInfo = member as FieldInfo;
            object reval = null;
            MemberExpression memberExpr = null;
            while (expression is MemberExpression)
            {
                memberExpr = expression as MemberExpression;
                memberInfos.Push(memberExpr.Member);
                if (memberExpr.Expression == null)
                {
                    var isProperty = memberExpr.Member.MemberType == MemberTypes.Property;
                    var isField = memberExpr.Member.MemberType == MemberTypes.Field;
                    if (isProperty)
                    {
                        try
                        {
                            //reval = GetPropertyValue(memberExpr);
                        }
                        catch
                        {
                            reval = null;
                        }
                    }
                    else if (isField)
                    {
                        //reval = GetFiledValue(memberExpr);
                    }
                }

                if (memberExpr.Expression == null)
                {
                }

                expression = memberExpr.Expression;
            }

            // fetch the root object reference:
            var constExpr = expression as ConstantExpression;
            if (constExpr == null)
            {
                // DynamicInvoke(rootExpression);
            }

            var objReference = constExpr.Value;
            // "ascend" back whence we came from and resolve object references along the way:
            while (memberInfos.Count > 0) // or some other break condition
            {
                var mi = memberInfos.Pop();
                if (mi.MemberType == MemberTypes.Property)
                {
                    var objProp = objReference.GetType().GetProperty(mi.Name);
                    if (objProp == null)
                    {
                        //objReference = DynamicInvoke(expression, rootExpression == null ? memberExpr : rootExpression);
                    }
                    else
                    {
                        objReference = objProp.GetValue(objReference, null);
                    }
                }
                else if (mi.MemberType == MemberTypes.Field)
                {
                    var objField = objReference.GetType().GetField(mi.Name);
                    if (objField == null)
                    {
                        //objReference = DynamicInvoke(expression, rootExpression == null ? memberExpr : rootExpression);
                    }
                    else
                    {
                        objReference = objField.GetValue(objReference);
                    }
                }
            }

            reval = objReference;
            return reval;
        }
    }
}
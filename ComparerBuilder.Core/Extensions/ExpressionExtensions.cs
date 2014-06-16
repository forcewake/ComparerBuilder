namespace ComparerBuilder.Core.Extensions
{
    using System;
    using System.Linq.Expressions;
    using ComparerBuilder.Core.Constants;
    using ComparerBuilder.Core.Visitors;

    public static class ExpressionExtensions
    {
        public static BinaryExpression ReferenceEqual(this Expression left, Expression right)
        {
            return Expression.ReferenceEqual(left, right);
        }

        public static BinaryExpression IsNull(this Expression value)
        {
            return Expression.ReferenceEqual(value, ExpressionConstants.Null);
        }

        public static BinaryExpression IsNotNull(this Expression value)
        {
            return Expression.ReferenceNotEqual(value, ExpressionConstants.Null);
        }

        public static ParameterExpression Left<T>()
        {
            return Expression.Parameter(typeof (T), null);
        }

        public static ParameterExpression Right<T>()
        {
            return Expression.Parameter(typeof (T), null);
        }

        public static bool IsValueType<T>()
        {
            return typeof (T).IsValueType;
        }

        public static Tuple<Expression, Expression> CreateParameters<TValue, TProperty>(
            this Expression<Func<TValue, TProperty>> expression)
        {
            if (expression == null)
            {
                throw new ArgumentNullException("expression");
            }

            if (expression.Parameters.Count != 1)
            {
                throw new ArgumentException("expression.Parameters.Count != 1", "expression");
            }

            var left = new ReplaceVisitor(expression.Parameters[0], Left<TValue>());
            var right = new ReplaceVisitor(expression.Parameters[0], Right<TValue>());
            return Tuple.Create(left.Visit(expression.Body), right.Visit(expression.Body));
        }
    }
}
namespace ComparerBuilder.Core.Implementation
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Linq.Expressions;
    using ComparerBuilder.Core.Constants;
    using ComparerBuilder.Core.Extensions;
    using ComparerBuilder.Core.Helpers;

    public class ComparerBuilder<TType> : IComparerBuilder<TType>
    {
        public ComparerBuilder()
        {
            EqualsExpressions = new List<Expression>();
            GetHashCodeExpressions = new List<Expression>();
            CompareExpressions = new List<Expression>();
        }

        protected IList<Expression> EqualsExpressions { get; set; }
        protected IList<Expression> GetHashCodeExpressions { get; set; }
        protected IList<Expression> CompareExpressions { get; set; }

        protected virtual Expression MakeEquals<TProperty>(Expression left, Expression right, IEqualityComparer<TProperty> comparer)
        {
            if (left == null)
            {
                throw new ArgumentNullException("left");
            }
            
            if (right == null)
            {
                throw new ArgumentNullException("right");
            }

            if (comparer != null)
            {
                // return comparer.Equals(left, right);
                Func<TProperty, TProperty, bool> @delegate = comparer.Equals;
                var instance = Expression.Constant(comparer);
                return Expression.Call(instance, @delegate.Method, left, right);
            }
            else
            {
                if (typeof (TProperty).IsValueType)
                {
                    // return left == right;
                    return Expression.Equal(left, right);
                }
                else
                {
                    // return Object.Equals(left, right);
                    return Expression.Call(ExpressionConstants.EqualsDelegate.Method, left, right);
                }
            }
        }

        protected virtual Expression MakeGetHashCode<TProperty>(Expression value, IEqualityComparer<TProperty> comparer)
        {
            if (value == null)
            {
                throw new ArgumentNullException("value");
            }

            if (comparer != null)
            {
                // return comparer.GetHashCode(left);
                Func<TProperty, int> hashCode = comparer.GetHashCode;
                var instance = Expression.Constant(comparer);
                return Expression.Call(instance, hashCode.Method, value);
            }

            if (typeof (TProperty).IsValueType)
            {
                // return left.GetHashCode();
                return Expression.Call(value, ExpressionConstants.GetHashCodeDelegate.Method);
            }
            else
            {
                // return Comparers.GetHashCode(left); [(left == null) ? 0 : left.GetHashCode();]
                Func<TProperty, int> hashCode = ComparerHelper.GetHashCode;
                return Expression.Call(hashCode.Method, value);
            }
        }

        protected virtual Expression MakeCompare<TProperty>(Expression left, Expression right, IComparer<TProperty> comparer)
        {
            if (left == null)
            {
                throw new ArgumentNullException("left");
            }
            
            if (right == null)
            {
                throw new ArgumentNullException("right");
            }

            if (comparer != null)
            {
                // return comparer.Compare(left, right);
                Func<TProperty, TProperty, int> @delegate = comparer.Compare;
                var instance = Expression.Constant(comparer);
                return Expression.Call(instance, @delegate.Method, left, right);
            }
            else
            {
                // return (left < right) ? -1 : (left > right ? 1 : 0);
                return Expression.Condition(Expression.LessThan(left, right), ExpressionConstants.MinusOne,
                    Expression.Condition(Expression.GreaterThan(left, right), ExpressionConstants.One, ExpressionConstants.Zero));
            }
        }

        private void AddCore<TProperty>(Tuple<Expression, Expression> args, IEqualityComparer<TProperty> comparer)
        {
            if (args == null)
            {
                throw new ArgumentNullException("args");
            }

            var equals = MakeEquals(args.Item1, args.Item2, comparer);
            EqualsExpressions.Add(equals);

            var hash = MakeGetHashCode(args.Item1, comparer);
            GetHashCodeExpressions.Add(hash);
        }

        private void AddCore<TProperty>(Tuple<Expression, Expression> args, IComparer<TProperty> comparer)
        {
            if (args == null)
            {
                throw new ArgumentNullException("args");
            }

            var compare = MakeCompare(args.Item1, args.Item2, comparer);
            CompareExpressions.Add(compare);
        }

        public ComparerBuilder<TType> Add<TProperty>(Expression<Func<TType, TProperty>> expression, IEqualityComparer<TProperty> equality,
            IComparer<TProperty> comparison)
        {
            var args = expression.CreateParameters();
            AddCore(args, equality);
            AddCore(args, comparison);
            return this;
        }

        public ComparerBuilder<TType> Add<TProperty, TComparer>(Expression<Func<TType, TProperty>> expression, TComparer comparer)
            where TComparer : IEqualityComparer<TProperty>, IComparer<TProperty>
        {
            return Add(expression, comparer, comparer);
        }

        public ComparerBuilder<TType> AddEquality<TProperty>(Expression<Func<TType, TProperty>> expression, IEqualityComparer<TProperty> equality = null)
        {
            var args = expression.CreateParameters();
            AddCore(args, equality);
            return this;
        }

        public ComparerBuilder<TType> AddComparison<TProperty>(Expression<Func<TType, TProperty>> expression, IComparer<TProperty> comparison = null)
        {
            var args = expression.CreateParameters();
            AddCore(args, comparison);
            return this;
        }

        public ComparerBuilder<TType> AddDefault<TProperty>(Expression<Func<TType, TProperty>> expression)
        {
            return Add(expression, EqualityComparer<TProperty>.Default, Comparer<TProperty>.Default);
        }

        public ComparerBuilder<TType> Add<TProperty>(Expression<Func<TType, TProperty>> expression)
        {
            return Add(expression, null, null);
        }

        private static Expression<Func<TType, TType, bool>> BuildEquals(IEnumerable<Expression> items)
        {
            if (items == null)
            {
                throw new ArgumentNullException("items");
            }

            var expression = items.Aggregate(Expression.AndAlso);
            ParameterExpression left = ExpressionExtensions.Left<TType>();
            ParameterExpression right = ExpressionExtensions.Right<TType>();
            Expression body;
            
            if (ExpressionExtensions.IsValueType<TType>())
            {
                body = expression;
            }
            else
            {
                body = Expression.OrElse(
                    left.ReferenceEqual(right),
                    Expression.AndAlso(
                        Expression.AndAlso(
                            left.IsNotNull(),
                            right.IsNotNull()),
                        expression));
            }
            return Expression.Lambda<Func<TType, TType, bool>>(body, left, right);
        }

        private static Expression<Func<TType, int>> BuildGetHashCode(IEnumerable<Expression> items)
        {
            if (items == null)
            {
                throw new ArgumentNullException("items");
            }

            ParameterExpression left = ExpressionExtensions.Left<TType>();

            var expressions = items as Expression[] ?? items.ToArray();
            var expression = expressions.Skip(1).Select((item, index) => Tuple.Create(item, index + 1))
                .Aggregate(expressions.First(), (acc, item) =>
                    Expression.ExclusiveOr(acc,
                        Expression.Call(ExpressionConstants.RotateRightDelegate.Method, item.Item1, Expression.Constant(item.Item2))));
            var body = ExpressionExtensions.IsValueType<TType>()
                ? expression
                // return ((object)x == null) ? 0 : expression;
                : Expression.Condition(left.IsNull(), ExpressionConstants.Zero, expression);
            return Expression.Lambda<Func<TType, int>>(body, left);
        }

        private static Expression<Func<TType, TType, int>> BuildCompare(IEnumerable<Expression> items)
        {
            if (items == null)
            {
                throw new ArgumentNullException("items");
            }

            ParameterExpression left = ExpressionExtensions.Left<TType>();
            ParameterExpression right = ExpressionExtensions.Right<TType>();

            var reverse = items.Reverse();
            var expressions = reverse as Expression[] ?? reverse.ToArray();
            Expression seed = Expression.Return(ExpressionConstants.Return, expressions.First());
            var expression = expressions.Skip(1).Aggregate(seed,
                (acc, value) => Expression.IfThenElse(
                    Expression.NotEqual(Expression.Assign(ExpressionConstants.Compare, value), ExpressionConstants.Zero),
                    ExpressionConstants.ReturnCompare, acc));
            Expression body;
            if (ExpressionExtensions.IsValueType<TType>())
            {
                body = expression;
            }
            else
            {
                body = Expression.IfThenElse(left.ReferenceEqual(right), ExpressionConstants.ReturnZero,
                    Expression.IfThenElse(left.IsNull(), ExpressionConstants.ReturnMinusOne,
                        Expression.IfThenElse(right.IsNull(), ExpressionConstants.ReturnOne, expression)));
            }
            var block = Expression.Block(ExpressionConstants.CompareValiables, body, ExpressionConstants.LabelZero);
            return Expression.Lambda<Func<TType, TType, int>>(block, left, right);
        }

        public EqualityComparer<TType> ToEqualityComparer()
        {
            if (EqualsExpressions.Count == 0 || this.GetHashCodeExpressions.Count == 0)
            {
                return ComparerHelper.EmptyEqualityComparer<TType>();
            }

            var equals = BuildEquals(this.EqualsExpressions);
            var hash = BuildGetHashCode(this.GetHashCodeExpressions);
            return ComparerHelper.Create(equals.Compile(), hash.Compile());
        }

        public Comparer<TType> ToComparer()
        {
            if (CompareExpressions.Count == 0)
            {
                return ComparerHelper.EmptyComparer<TType>();
            }

            var compare = BuildCompare(this.CompareExpressions);
            return ComparerHelper.Create(compare.Compile());
        }
    }
}
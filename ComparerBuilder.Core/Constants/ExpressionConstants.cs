namespace ComparerBuilder.Core.Constants
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Linq.Expressions;
    using ComparerBuilder.Core.Helpers;

    public static class ExpressionConstants
    {
        public static readonly ConstantExpression Null = Expression.Constant(null);
        public static readonly ConstantExpression Zero = Expression.Constant(0);

        public static readonly ConstantExpression One = Expression.Constant(1);
        public static readonly ConstantExpression MinusOne = Expression.Constant(-1);

        public static readonly ParameterExpression Compare = Expression.Parameter(typeof(int));
        public static readonly IEnumerable<ParameterExpression> CompareValiables = Enumerable.Repeat(Compare, 1);
        public static readonly LabelTarget Return = Expression.Label(typeof(int));
        public static readonly LabelExpression LabelZero = Expression.Label(Return, Zero);
        public static readonly GotoExpression ReturnZero = Expression.Return(Return, Zero);
        public static readonly GotoExpression ReturnOne = Expression.Return(Return, One);
        public static readonly GotoExpression ReturnMinusOne = Expression.Return(Return, MinusOne);
        public static readonly GotoExpression ReturnCompare = Expression.Return(Return, Compare);

        public static readonly Func<object, object, bool> EqualsDelegate = Object.Equals;
        public static readonly Func<int> GetHashCodeDelegate = new object().GetHashCode;
        public static readonly Func<int, int, int> RotateRightDelegate = ComparerHelper.RotateRight;
    }
}
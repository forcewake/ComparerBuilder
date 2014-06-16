namespace ComparerBuilder.Core
{
    using System;
    using System.Collections.Generic;
    using System.Linq.Expressions;
    using ComparerBuilder.Core.Implementation;

    public interface IComparerBuilder<TType>
    {
        ComparerBuilder<TType> Add<TProperty>(Expression<Func<TType, TProperty>> expression, IEqualityComparer<TProperty> equality,
            IComparer<TProperty> comparison);

        ComparerBuilder<TType> Add<TProperty, TComparer>(Expression<Func<TType, TProperty>> expression, TComparer comparer)
            where TComparer : IEqualityComparer<TProperty>, IComparer<TProperty>;

        ComparerBuilder<TType> AddEquality<TProperty>(Expression<Func<TType, TProperty>> expression, IEqualityComparer<TProperty> equality = null);
        ComparerBuilder<TType> AddComparison<TProperty>(Expression<Func<TType, TProperty>> expression, IComparer<TProperty> comparison = null);
        ComparerBuilder<TType> AddDefault<TProperty>(Expression<Func<TType, TProperty>> expression);
        ComparerBuilder<TType> Add<TProperty>(Expression<Func<TType, TProperty>> expression);

        EqualityComparer<TType> ToEqualityComparer();

        Comparer<TType> ToComparer();
    }
}
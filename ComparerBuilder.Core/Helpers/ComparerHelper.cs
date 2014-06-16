namespace ComparerBuilder.Core.Helpers
{
    using System;
    using System.Collections.Generic;
    using ComparerBuilder.Core.Comparers;

    public static class ComparerHelper
    {
        public static Comparer<T> EmptyComparer<T>()
        {
            return ConstComparer<T>.Default;
        }

        public static EqualityComparer<T> EmptyEqualityComparer<T>()
        {
            return ConstEqualityComparer<T>.Default;
        }

        public static EqualityComparer<T> Const<T>(bool equals, int hash)
        {
            return new ConstEqualityComparer<T>(@equals, hash);
        }

        public static Comparer<T> Const<T>(int compare)
        {
            return new ConstComparer<T>(compare);
        }

        public static EqualityComparer<T> Create<T>(Func<T, T, bool> equals, Func<T, int> hash)
        {
            if (@equals == null)
            {
                throw new ArgumentNullException("equals");
            }

            return new MethodEqualityComparer<T>(@equals, hash);
        }

        public static EqualityComparer<TValue> Create<TValue>(Func<TValue, TValue, bool> equals) where TValue : class
        {
            return Create(@equals, item => (item == null) ? 0 : item.GetHashCode());
        }

        public static Comparer<T> Create<T>(Func<T, T, int> compare)
        {
            if (compare == null)
            {
                throw new ArgumentNullException("compare");
            }

            return new MethodComparer<T>(compare);
        }

        public static int RotateRight(int value, int places)
        {
            if ((places &= 0x1F) == 0)
            {
                return value;
            }

            var mask = ~0x7FFFFFFF >> (places - 1);
            return ((value >> places) & ~mask) | ((value << (32 - places)) & mask);
        }

        public static int GetHashCode<TValue>(TValue value)
        {
            return value == null ? 0 : value.GetHashCode();
        }
    }
}
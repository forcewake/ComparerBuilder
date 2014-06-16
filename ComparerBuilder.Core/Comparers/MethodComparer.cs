namespace ComparerBuilder.Core.Comparers
{
    using System;
    using System.Collections.Generic;

    [Serializable]
    internal sealed class MethodComparer<T> : Comparer<T>
    {
        public MethodComparer(Func<T, T, int> compare)
        {
            if (compare == null)
            {
                throw new ArgumentNullException("compare");
            }

            CompareMethod = compare;
        }

        private Func<T, T, int> CompareMethod { get; set; }

        public override int Compare(T x, T y)
        {
            return CompareMethod(x, y);
        }
    }
}
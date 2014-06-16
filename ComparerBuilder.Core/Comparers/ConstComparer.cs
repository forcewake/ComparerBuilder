namespace ComparerBuilder.Core.Comparers
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;

    [Serializable]
    internal sealed class ConstComparer<T> : Comparer<T>
    {
        private static readonly Comparer<T> @default = new ConstComparer<T>(0);

        public ConstComparer(int compare)
        {
            CompareValue = compare;
        }

        public static new Comparer<T> Default
        {
            [DebuggerStepThrough]
            get { return @default; }
        }

        private int CompareValue { get; set; }

        public override int Compare(T x, T y)
        {
            return CompareValue;
        }
    }
}
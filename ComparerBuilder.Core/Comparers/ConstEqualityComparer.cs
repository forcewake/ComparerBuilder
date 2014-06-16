namespace ComparerBuilder.Core.Comparers
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;

    [Serializable]
    internal sealed class ConstEqualityComparer<T> : EqualityComparer<T>
    {
        private static readonly EqualityComparer<T> @default = new ConstEqualityComparer<T>(true, 0);

        public ConstEqualityComparer(bool equals, int hash)
        {
            EqualsValue = @equals;
            GetHashCodeValue = hash;
        }

        public static new EqualityComparer<T> Default
        {
            [DebuggerStepThrough]
            get { return @default; }
        }

        private bool EqualsValue { get; set; }

        private int GetHashCodeValue { get; set; }

        public override bool Equals(T x, T y)
        {
            return EqualsValue;
        }

        public override int GetHashCode(T obj)
        {
            return GetHashCodeValue;
        }
    }
}
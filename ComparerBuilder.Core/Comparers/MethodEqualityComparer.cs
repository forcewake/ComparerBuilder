namespace ComparerBuilder.Core.Comparers
{
    using System;
    using System.Collections.Generic;

    [Serializable]
    internal sealed class MethodEqualityComparer<T> : EqualityComparer<T>
    {
        public MethodEqualityComparer(Func<T, T, bool> equals, Func<T, int> hash)
        {
            if (@equals == null)
            {
                throw new ArgumentNullException("equals");
            }
            
            if (hash == null)
            {
                throw new ArgumentNullException("hash");
            }

            EqualsMethod = @equals;
            GetHashCodeMethod = hash;
        }

        private Func<T, T, bool> EqualsMethod { get; set; }

        private Func<T, int> GetHashCodeMethod { get; set; }

        public override bool Equals(T x, T y)
        {
            return EqualsMethod(x, y);
        }

        public override int GetHashCode(T obj)
        {
            return GetHashCodeMethod(obj);
        }
    }
}
namespace ComparerBuilder.Example
{
    using System;
    using System.Collections.Generic;
    using ComparerBuilder.Core;

    public class R : IEquatable<R>, IComparable, IComparable<R>
    {
        private static readonly IComparerBuilder<R> Builder = new Core.Implementation.ComparerBuilder<R>()
          .Add(x => x.A)
          .Add(x => x.B, StringComparer.Ordinal)
          .AddDefault(x => x.C);

        private static readonly EqualityComparer<R> Equality = Builder.ToEqualityComparer();
        private static readonly Comparer<R> Comparison = Builder.ToComparer();

        public R(int a, string b, object c)
        {
            A = a;
            B = b;
            C = c;
        }

        public int A { get; private set; }
        public string B { get; private set; }
        public object C { get; private set; }

        public override bool Equals(object obj)
        {
            return Equals(obj as R);
        }

        public override int GetHashCode()
        {
            return Equality.GetHashCode(this);
        }

        public bool Equals(R other)
        {
            return Equality.Equals(this, other);
        }

        int IComparable.CompareTo(object obj)
        {
            var other = obj as R;
            if (obj != null && (object)other == null)
            {
                throw new ArgumentException("obj != null && !(obj is R)", "obj");
            }
            return CompareTo((R)obj);
        }

        public int CompareTo(R other)
        {
            return Comparison.Compare(this, other);
        }

        public static bool operator ==(R left, R right)
        {
            return Equals(left, right);
        }

        public static bool operator !=(R left, R right)
        {
            return !(left == right);
        }

        public static bool operator <(R left, R right)
        {
            return Comparison.Compare(left, right) < 0;
        }

        public static bool operator <=(R left, R right)
        {
            return Comparison.Compare(left, right) <= 0;
        }

        public static bool operator >(R left, R right)
        {
            return !(left <= right);
        }

        public static bool operator >=(R left, R right)
        {
            return !(left < right);
        }
    }
}

namespace ComparerBuilder.Core.Visitors
{
    using System;
    using System.Linq.Expressions;

    internal sealed class ReplaceVisitor : ExpressionVisitor
    {
        public ReplaceVisitor(Expression what, Expression to)
        {
            if (what == null)
            {
                throw new ArgumentNullException("what");
            }
            
            if (to == null)
            {
                throw new ArgumentNullException("to");
            }

            What = what;
            To = to;
        }

        public Expression What { get; private set; }
        public Expression To { get; private set; }

        public override Expression Visit(Expression node)
        {
            if (node == What)
            {
                return To;
            }

            return base.Visit(node);
        }
    }
}
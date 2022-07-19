using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;

namespace ShiftSoftware.ShiftGrid.Core
{
    internal class AggregateColumnRemover<T, T2> : ExpressionVisitor
    {
        internal Expression<Func<IGrouping<int, T>, T2>> Query { get; set; }
        private List<string> ColumnsToRemove;

        public AggregateColumnRemover(Expression<Func<IGrouping<int, T>, T2>> query)
        {
            this.Query = query;
        }

        public Expression<Func<IGrouping<int, T>, T2>> RemoveColumns(List<string> columnsToRemove)
        {
            this.ColumnsToRemove = columnsToRemove;
            
            var expression = this.Visit(this.Query) as Expression<Func<IGrouping<int, T>, T2>>;

            //System.Diagnostics.Debug.WriteLine(expression);

            return expression;
        }

        protected override Expression VisitMember(MemberExpression node)
        {
            if (this.ColumnsToRemove.Any(x => x.Equals(node.Member.Name, StringComparison.InvariantCultureIgnoreCase)))
            {
                return AnonymousColumnRemover<T>.GetDefaultExpressionFor(node.Member);
            }

            return base.VisitMember(node);
        }

        protected override Expression VisitMethodCall(MethodCallExpression node)
        {
            if (new HiddenMemberChecker(this.ColumnsToRemove).ContainsHiddenMember(node))
                return AnonymousColumnRemover<T>.GetDefaultExpressionFor(node.Method.ReturnType);

            return base.VisitMethodCall(node);
        }

        public override Expression Visit(Expression node)
        {
            //System.Diagnostics.Debug.WriteLine(node + "\t\t" + node?.NodeType);

            return base.Visit(node);
        }
    }

    class HiddenMemberChecker : ExpressionVisitor
    {
        public List<string> ColumnsToRemove { get; set; }
        private bool containsHiddenMember = false;

        public HiddenMemberChecker(List<string> columnsToRemove)
        {
            this.ColumnsToRemove = columnsToRemove;
        }

        protected override Expression VisitMember(MemberExpression node)
        {
            if (this.ColumnsToRemove.Any(x => x.Equals(node.Member.Name, StringComparison.InvariantCultureIgnoreCase)))
            {
                this.containsHiddenMember = true;
            }

            return base.VisitMember(node);
        }

        public bool ContainsHiddenMember(Expression expression)
        {
            this.Visit(expression);

            return this.containsHiddenMember;
        }
    }
}

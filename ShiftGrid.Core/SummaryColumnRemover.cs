using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;

namespace ShiftSoftware.ShiftGrid.Core
{
    internal class SummaryColumnRemover<T> : ExpressionVisitor
    {
        internal Expression<Func<IGrouping<int, T>, object>> Query { get; set; }
        private List<string> ColumnsToRemove;

        public SummaryColumnRemover(Expression<Func<IGrouping<int, T>, object>> query)
        {
            this.Query = query;
        }

        public Expression<Func<IGrouping<int, T>, object>> RemoveColumns(List<string> columnsToRemove)
        {
            this.ColumnsToRemove = columnsToRemove;
            
            var expression = this.Visit(this.Query) as Expression<Func<IGrouping<int, T>, object>>;

            //System.Diagnostics.Debug.WriteLine(expression);

            return expression;
        }

        protected override Expression VisitMember(MemberExpression node)
        {
            if (this.ColumnsToRemove.Contains(node.Member.Name))
            {
                return AnonymousColumnRemover<T>.GetDefaultExpressionFor(node.Member);
            }

            return base.VisitMember(node);
        }
        public override Expression Visit(Expression node)
        {
            return base.Visit(node);
        }
    }
}

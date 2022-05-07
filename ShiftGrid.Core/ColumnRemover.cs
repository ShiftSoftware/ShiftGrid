using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Reflection.Emit;
using System.Text;

namespace ShiftSoftware.ShiftGrid.Core
{
    internal class ColumnRemover<T> : ExpressionVisitor
    {
        private IQueryable Query;
        private List<string> ColumnsToRemove;

        public ColumnRemover(IQueryable<T> query)
        {
            this.Query = query;
        }

        public IQueryable<T> RemoveColumns(List<string> columnsToRemove)
        {
            this.ColumnsToRemove = columnsToRemove;
            return Query.Provider.CreateQuery<T>(this.Visit(Query.Expression));
        }

        public Expression ExcludeMember(MemberInitExpression node)
        {
            List<MemberBinding> bindings = new List<MemberBinding> { };

            foreach (var binding in node.Bindings)
            {
                if (this.ColumnsToRemove != null && this.ColumnsToRemove.Contains(binding.Member.Name))
                    continue;

                bindings.Add(binding);
            }

            var ex = Expression.MemberInit(
                node.NewExpression,
                bindings.ToArray()
            );

            //System.Diagnostics.Debug.WriteLine(ex);

            return ex;
        }

        protected override Expression VisitMemberInit(MemberInitExpression node)
        {
            return this.ExcludeMember(node);
        }

        public override Expression Visit(Expression node)
        {
            //System.Diagnostics.Debug.WriteLine(node + "\t\t" + node?.NodeType);
            return base.Visit(node);
        }
    }
}

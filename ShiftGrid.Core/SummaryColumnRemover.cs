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
            return this.Visit(this.Query) as Expression<Func<IGrouping<int, T>, object>>;
        }

        protected override Expression VisitMember(MemberExpression node)
        {
            if (this.ColumnsToRemove.Contains(node.Member.Name))
            {
                if (node.Member.MemberType == System.Reflection.MemberTypes.Property)
                {
                    var type = ((System.Reflection.PropertyInfo)node?.Member).PropertyType;

                    if (type.IsValueType)
                    {
                        return MemberExpression.New(type);
                    }
                }
            }

            return base.VisitMember(node);
        }
        public override Expression Visit(Expression node)
        {
            return base.Visit(node);
        }
    }
}

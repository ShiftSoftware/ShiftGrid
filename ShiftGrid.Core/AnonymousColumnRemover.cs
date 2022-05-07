using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;

namespace ShiftSoftware.ShiftGrid.Core
{
    internal class AnonymousColumnRemover<T> : ExpressionVisitor
    {
        private IQueryable Query;
        private List<string> ColumnsToRemove;

        public AnonymousColumnRemover(IQueryable<T> query)
        {
            this.Query = query;
        }

        public IQueryable<T> RemoveColumns(List<string> columnsToRemove)
        {
            this.ColumnsToRemove = columnsToRemove;
            return Query.Provider.CreateQuery<T>(this.Visit(Query.Expression));
        }
        public override Expression Visit(Expression node)
        {
            //System.Diagnostics.Debug.WriteLine(node + "\t" + node?.NodeType);
            return base.Visit(node);
        }

        protected override Expression VisitNew(NewExpression node)
        {
            var index = 0;
            var args = new List<Expression>();

            if (node == null || node.Members == null)
                return base.VisitNew(node);

            foreach (var member in node.Members)
            {
                if (this.ColumnsToRemove.Contains(member.Name))
                {
                    args.Add(GetDefaultExpressionFor(member));
                }
                else
                    args.Add(node.Arguments[index]);

                index++;
            }

            var theNewExpression = Expression.New(
                node.Constructor, args, node.Members
            );

            return base.VisitNew(theNewExpression);
        }

        public static Expression GetDefaultExpressionFor(MemberInfo member)
        {
            Expression expression = null;

            if (member.MemberType == MemberTypes.Property)
            {
                var type = ((PropertyInfo)member).PropertyType;

                if (type.IsValueType)
                    expression = MemberExpression.New(type);
                else
                    expression = MemberExpression.Constant(null, type);
            }

            return expression;
        }
    }
}

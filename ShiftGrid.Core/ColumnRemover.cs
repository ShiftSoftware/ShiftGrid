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

        //The code below is supposed to exclude Accessing Members that are marked as Invisible Columns on Anonymous Types.
        //But I was unable to do it. Instead of that, I created an exception to prevent people from trying to hide columns on an Anonymous Type.
        //This should resolved at some point in the Future.

        //protected override Expression VisitNew(NewExpression node)
        //{
        //    var annonType = node.Constructor.DeclaringType;

        //    var propertyTypes = annonType.GetProperties().Select(x => x.PropertyType).ToArray();

        //    var types = new List<Type>();
        //    var members = new List<System.Reflection.MemberInfo>();
        //    var arguments = new List<Expression>();

        //    for (int i = 0; i < node.Members.Count; i++)
        //    {
        //        var member = node.Members[i];

        //        if (this.ColumnsToRemove != null && this.ColumnsToRemove.Contains(member.Name))
        //            continue;

        //        types.Add(propertyTypes[i]);
        //        members.Add(member);
        //        arguments.Add(node.Arguments.ElementAt(i));
        //    }

        //    //foreach (var item in members)
        //    //{
        //    //    System.Diagnostics.Debug.WriteLine(item);
        //    //}

        //    //long id = 1;
        //    //Type anonType = new { ID = id, Title = "Hi" }.GetType();

        //    //var exp = Expression.New(
        //    //    anonType.GetConstructor(types.ToArray()),
        //    //    arguments
        //    //);

        //    //var lambda = LambdaExpression.Lambda(exp);
        //    //object myObj = lambda.Compile().DynamicInvoke();

        //    AssemblyName aName = new AssemblyName("DynamicAssemblyExample");
        //    AssemblyBuilder ab =
        //        AssemblyBuilder.DefineDynamicAssembly(
        //            aName,
        //            AssemblyBuilderAccess.Run);

        //    // The module name is usually the same as the assembly name.
        //    ModuleBuilder mb =
        //        ab.DefineDynamicModule(aName.Name);

        //    TypeBuilder tb = mb.DefineType(
        //        "MyDynamicType",
        //         TypeAttributes.Public);

        //  var idField =   tb.DefineField(
        //   "ID",
        //   typeof(long),
        //   FieldAttributes.Public);

        //    tb.DefineField(
        //  "Title",
        //  typeof(string),
        //  FieldAttributes.Public);

        //    var pointCtor = tb.DefineConstructor(
        //        System.Reflection.MethodAttributes.Public,
        //        System.Reflection.CallingConventions.Standard,
        //        types.ToArray()
        //    );

        //    ILGenerator ctor1IL = pointCtor.GetILGenerator();
        //    ctor1IL.Emit(OpCodes.Ldarg_0);

        //    tb.CreateTypeInfo();

        //    var newExpression = Expression.New(
        //        pointCtor,
        //        arguments
        //    );

        //    return base.VisitNew(newExpression);
        //    //System.Diagnostics.Debug.WriteLine(newExpression);

        //    //return base.VisitNew(node);
        //}
    }
}

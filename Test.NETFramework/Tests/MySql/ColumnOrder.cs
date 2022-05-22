using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Test.NETFramework.Tests.MySql
{
    [TestClass]
    public class ColumnOrder : ShiftGrid.Test.Shared.Tests.ColumnOrder
    {
        public ColumnOrder() : base(typeof(EF.MySQLDb), new Utils(typeof(EF.MySQLDb)))
        {

        }
    }
}
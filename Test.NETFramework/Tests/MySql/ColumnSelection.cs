using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Test.NETFramework.Tests.MySql
{
    [TestClass]
    public class ColumnSelection : ShiftGrid.Test.Shared.Tests.ColumnSelection
    {
        public ColumnSelection() : base(typeof(EF.MySQLDb), new Utils(typeof(EF.MySQLDb)))
        {

        }
    }
}
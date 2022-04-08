using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ShiftGrid.Test.NET.Tests.MySql
{
    [TestClass]
    public class ColumnOrder : Tests.ColumnOrder
    {
        public ColumnOrder() : base(typeof(EF.MySQLDb))
        {

        }
    }
}
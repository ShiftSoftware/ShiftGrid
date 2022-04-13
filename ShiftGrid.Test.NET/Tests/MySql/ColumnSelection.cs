using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ShiftGrid.Test.NET.Tests.MySql
{
    [TestClass]
    public class ColumnSelection : Tests.ColumnSelection
    {
        public ColumnSelection() : base(typeof(EF.MySQLDb))
        {

        }
    }
}
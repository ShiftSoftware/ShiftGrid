using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ShiftGrid.Test.NET.Tests.SQLServer
{
    [TestClass]
    public class ColumnOrder : Tests.ColumnOrder
    {
        public ColumnOrder() : base(typeof(EF.DB))
        {

        }
    }
}
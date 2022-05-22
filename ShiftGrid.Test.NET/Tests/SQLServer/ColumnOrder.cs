using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ShiftGrid.Test.NET.Tests.SQLServer
{
    [TestClass]
    public class ColumnOrder : ShiftGrid.Test.Shared.Tests.ColumnOrder
    {
        public ColumnOrder() : base(typeof(EF.DB), new Utils(typeof(EF.DB)))
        {

        }
    }
}
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Test.NETFramework.Tests.SQLServer
{
    [TestClass]
    public class ColumnOrder : ShiftGrid.Test.Shared.Tests.ColumnOrder
    {
        public ColumnOrder() : base(typeof(EF.DB), new Utils(typeof(EF.DB)))
        {

        }
    }
}
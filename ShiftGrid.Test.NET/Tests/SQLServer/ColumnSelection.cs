using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ShiftGrid.Test.NET.Tests.SQLServer
{
    [TestClass]
    public class ColumnSelection : ShiftGrid.Test.Shared.Tests.ColumnSelection
    {
        public ColumnSelection() : base(typeof(EF.DB), new Utils(typeof(EF.DB)))
        {

        }
    }
}
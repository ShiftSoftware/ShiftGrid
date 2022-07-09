using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Test.NETCore.Tests.Postgres
{
    [TestClass]
    public class ColumnSelection : ShiftGrid.Test.Shared.Tests.ColumnSelection
    {
        public ColumnSelection() : base(typeof(EF.PostgresDB), new Utils(typeof(EF.PostgresDB)))
        {

        }
    }
}

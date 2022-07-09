using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Test.NETFramework.Tests.Postgres
{
    [TestClass]
    public class ColumnOrder : ShiftGrid.Test.Shared.Tests.ColumnOrder
    {
        public ColumnOrder() : base(typeof(EF.PostgresDB), new Utils(typeof(EF.PostgresDB)))
        {

        }
    }
}
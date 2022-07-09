using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Test.NETCore.Tests.Postgres
{
    [TestClass]
    public class Filters : ShiftGrid.Test.Shared.Tests.Filters
    {
        public Filters() : base(typeof(EF.PostgresDB), new Utils(typeof(EF.PostgresDB)))
        {

        }
    }
}

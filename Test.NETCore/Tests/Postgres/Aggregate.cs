using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Test.NETCore.Tests.Postgres
{
    [TestClass]
    public class Aggregate : ShiftGrid.Test.Shared.Tests.Aggregate
    {
        public Aggregate() : base(typeof(EF.PostgresDB), new Utils(typeof(EF.PostgresDB)))
        {

        }
    }
}

using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Test.NETCore.Tests.Postgres
{
    [TestClass]
    public class Sort : ShiftGrid.Test.Shared.Tests.Sort
    {
        public Sort() : base(typeof(EF.PostgresDB), new Utils(typeof(EF.PostgresDB)))
        {

        }
    }
}

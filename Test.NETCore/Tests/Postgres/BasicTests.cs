using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Test.NETCore.Tests.Postgres
{
    [TestClass]
    public class BasicTests : ShiftGrid.Test.Shared.Tests.BasicTests
    {
        public BasicTests() : base(typeof(EF.PostgresDB), new Utils(typeof(EF.PostgresDB)))
        {

        }
    }
}

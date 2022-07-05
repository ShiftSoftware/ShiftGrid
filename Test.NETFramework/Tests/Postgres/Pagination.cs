using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Test.NETFramework.Tests.Postgres
{
    [TestClass]
    public class Pagination : ShiftGrid.Test.Shared.Tests.Pagination
    {
        public Pagination() : base(typeof(EF.PostgresDB), new Utils(typeof(EF.PostgresDB)))
        {

        }
    }
}
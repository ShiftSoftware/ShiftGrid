using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ShiftGrid.Test.NET.Tests.MySql
{
    [TestClass]
    public class Pagination : Shared.Tests.Pagination
    {
        public Pagination() : base(typeof(EF.MySQLDb), new Utils(typeof(EF.MySQLDb)))
        {

        }
    }
}
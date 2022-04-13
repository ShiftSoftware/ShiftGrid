using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ShiftGrid.Test.NET.Tests.MySql
{
    [TestClass]
    public class Pagination : Tests.Pagination
    {
        public Pagination() : base(typeof(EF.MySQLDb))
        {

        }
    }
}
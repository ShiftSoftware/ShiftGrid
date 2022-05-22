using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ShiftGrid.Test.NET.Tests.MySql
{
    [TestClass]
    public class Filters : Shared.Tests.Filters
    {
        public Filters() : base(typeof(EF.MySQLDb), new Utils(typeof(EF.MySQLDb)))
        {

        }
    }
}
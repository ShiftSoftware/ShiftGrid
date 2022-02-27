using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ShiftGrid.Test.NET.Tests.MySql
{
    [TestClass]
    public class Filters : Tests.Filters
    {
        public Filters() : base(typeof(EF.MySQLDb))
        {

        }
    }
}
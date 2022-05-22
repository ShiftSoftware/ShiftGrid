using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Test.NETCore.Tests.MySql
{
    [TestClass]
    public class Filters : ShiftGrid.Test.Shared.Tests.Filters
    {
        public Filters() : base(typeof(EF.MySqlDB), new Utils(typeof(EF.MySqlDB)))
        {

        }
    }
}

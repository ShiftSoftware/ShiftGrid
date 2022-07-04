using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Test.NETCore.Tests.MySql
{
    [TestClass]
    public class Aggregate : ShiftGrid.Test.Shared.Tests.Aggregate
    {
        public Aggregate() : base(typeof(EF.MySqlDB), new Utils(typeof(EF.MySqlDB)))
        {

        }
    }
}

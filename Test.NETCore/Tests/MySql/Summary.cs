using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Test.NETCore.Tests.MySql
{
    [TestClass]
    public class Summary : ShiftGrid.Test.Shared.Tests.Summary
    {
        public Summary() : base(typeof(EF.MySqlDB), new Utils(typeof(EF.MySqlDB)))
        {

        }
    }
}

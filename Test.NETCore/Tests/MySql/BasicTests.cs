using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Test.NETCore.Tests.MySql
{
    [TestClass]
    public class BasicTests : ShiftGrid.Test.Shared.Tests.BasicTests
    {
        public BasicTests() : base(typeof(EF.MySqlDB), new Utils(typeof(EF.MySqlDB)))
        {

        }
    }
}

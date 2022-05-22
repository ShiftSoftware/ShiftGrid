using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Test.NETCore.Tests.MySql
{
    [TestClass]
    public class Sort : ShiftGrid.Test.Shared.Tests.Sort
    {
        public Sort() : base(typeof(EF.MySqlDB), new Utils(typeof(EF.MySqlDB)))
        {

        }
    }
}

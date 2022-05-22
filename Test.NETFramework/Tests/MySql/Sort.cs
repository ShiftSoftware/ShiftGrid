using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Test.NETFramework.Tests.MySql
{
    [TestClass]
    public class Sort : ShiftGrid.Test.Shared.Tests.Sort
    {
        public Sort() : base(typeof(EF.MySQLDb), new Utils(typeof(EF.MySQLDb)))
        {

        }
    }
}
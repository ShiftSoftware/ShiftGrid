using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Test.NETFramework.Tests.MySql
{
    [TestClass]
    public class Summary : ShiftGrid.Test.Shared.Tests.Summary
    {
        public Summary() : base(typeof(EF.MySQLDb), new Utils(typeof(EF.MySQLDb)))
        {

        }
    }
}
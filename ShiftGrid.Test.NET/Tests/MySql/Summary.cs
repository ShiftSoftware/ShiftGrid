using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ShiftGrid.Test.NET.Tests.MySql
{
    [TestClass]
    public class Summary : Shared.Tests.Summary
    {
        public Summary() : base(typeof(EF.MySQLDb), new Utils(typeof(EF.MySQLDb)))
        {

        }
    }
}
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Test.NETFramework.Tests.MySql
{
    [TestClass]
    public class Aggregate : ShiftGrid.Test.Shared.Tests.Aggregate
    {
        public Aggregate() : base(typeof(EF.MySQLDb), new Utils(typeof(EF.MySQLDb)))
        {

        }
    }
}
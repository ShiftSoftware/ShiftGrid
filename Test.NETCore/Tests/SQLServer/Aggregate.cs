using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Test.NETCore.Tests.SQLServer
{
    [TestClass]
    public class Aggregate : ShiftGrid.Test.Shared.Tests.Aggregate
    {
        public Aggregate() : base(typeof(EF.DB), new Utils(typeof(EF.DB)))
        {

        }
    }
}

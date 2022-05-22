using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Test.NETFramework.Tests.SQLServer
{
    [TestClass]
    public class Sort : ShiftGrid.Test.Shared.Tests.Sort
    {
        public Sort() : base(typeof(EF.DB), new Utils(typeof(EF.DB)))
        {

        }
    }
}
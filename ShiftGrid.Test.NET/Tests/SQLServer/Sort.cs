using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ShiftGrid.Test.NET.Tests.SQLServer
{
    [TestClass]
    public class Sort : ShiftGrid.Test.Shared.Tests.Sort
    {
        public Sort() : base(typeof(EF.DB), new Utils(typeof(EF.DB)))
        {

        }
    }
}
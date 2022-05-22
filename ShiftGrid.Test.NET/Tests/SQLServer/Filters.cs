using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ShiftGrid.Test.NET.Tests.SQLServer
{
    [TestClass]
    public class Filters : ShiftGrid.Test.Shared.Tests.Filters
    {
        public Filters() : base(typeof(EF.DB), new Utils(typeof(EF.DB)))
        {

        }
    }
}
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ShiftGrid.Test.NET.Tests.SQLServer
{
    [TestClass]
    public class Filters : Tests.Filters
    {
        public Filters() : base(typeof(EF.DB))
        {

        }
    }
}
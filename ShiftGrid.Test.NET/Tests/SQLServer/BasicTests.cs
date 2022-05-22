using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ShiftGrid.Test.NET.Tests.SQLServer
{
    [TestClass]
    public class BasicTests : ShiftGrid.Test.Shared.Tests.BasicTests
    {
        public BasicTests() : base(typeof(EF.DB), new Utils(typeof(EF.DB)))
        {

        }
    }
}
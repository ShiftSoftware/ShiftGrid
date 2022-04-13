using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ShiftGrid.Test.NET.Tests.SQLServer
{
    [TestClass]
    public class BasicTests : Tests.BasicTests
    {
        public BasicTests() : base(typeof(EF.DB))
        {

        }
    }
}
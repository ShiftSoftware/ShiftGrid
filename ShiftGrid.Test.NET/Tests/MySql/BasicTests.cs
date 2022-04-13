using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ShiftGrid.Test.NET.Tests.MySql
{
    [TestClass]
    public class BasicTests : Tests.BasicTests
    {
        public BasicTests() : base(typeof(EF.MySQLDb))
        {

        }
    }
}
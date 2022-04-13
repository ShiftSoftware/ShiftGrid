using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ShiftGrid.Test.NET.Tests.MySql
{
    [TestClass]
    public class Sort : Tests.Sort
    {
        public Sort() : base(typeof(EF.MySQLDb))
        {

        }
    }
}
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ShiftGrid.Test.NET.Tests.SQLServer
{
    [TestClass]
    public class Pagination : Tests.Pagination
    {
        public Pagination() : base(typeof(EF.DB))
        {

        }
    }
}
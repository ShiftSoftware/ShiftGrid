using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ShiftGrid.Test.NET.Tests.SQLServer
{
    [TestClass]
    public class Sort : Tests.Sort
    {
        public Sort() : base(typeof(EF.DB))
        {

        }
    }
}
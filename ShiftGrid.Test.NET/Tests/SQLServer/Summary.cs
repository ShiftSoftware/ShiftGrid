using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ShiftGrid.Test.NET.Tests.SQLServer
{
    [TestClass]
    public class Summary : Tests.Summary
    {
        public Summary() : base(typeof(EF.DB))
        {

        }
    }
}
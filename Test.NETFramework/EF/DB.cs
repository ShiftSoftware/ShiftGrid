using System.Data.Entity;

namespace Test.NETFramework.EF
{
    public class DB :DBBase
    {
        public DB() : base("name=ShiftGrid_SQLServer")
        {
        }
    }
}

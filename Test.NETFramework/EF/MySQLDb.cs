using System.Data.Entity;

namespace Test.NETFramework.EF
{
    public class MySQLDb : DBBase
    {
        public MySQLDb() : base("name=ShiftGrid_MySQL")
        {
        }
    }
}

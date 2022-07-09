using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Test.NETFramework.EF
{
    public class PostgresDB: DBBase
    {
        public PostgresDB() : base("name=ShiftGrid_Postgres")
        {

        }
    }
}

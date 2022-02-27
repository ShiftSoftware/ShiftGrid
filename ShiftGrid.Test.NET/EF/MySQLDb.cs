using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace ShiftGrid.Test.NET.EF
{
    [DbConfigurationType(typeof(MySql.Data.EntityFramework.MySqlEFConfiguration))]
    public class MySQLDb: DBBase
    {
        public MySQLDb() : base("name=ShiftGrid_MySQL")
        {

        }
    }
}
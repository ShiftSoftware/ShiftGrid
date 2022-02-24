using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace ShiftGrid.Test.NET.EF
{
    [DbConfigurationType(typeof(MySql.Data.EntityFramework.MySqlEFConfiguration))]
    public class MySQLDb: DbContext
    {
        public MySQLDb() : base("name=ShiftGrid_MySQL")
        {

        }

        public virtual DbSet<Models.TestItem> TestItems { get; set; }
        public virtual DbSet<Models.Type> Types { get; set; }
    }
}
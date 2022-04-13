using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace ShiftGrid.Test.NET.EF
{
    public class DBBase : DbContext
    {
        public DBBase(string connectionString): base(connectionString)
        {

        }

        public virtual DbSet<Models.TestItem> TestItems { get; set; }
        public virtual DbSet<Models.Type> Types { get; set; }
    }
}
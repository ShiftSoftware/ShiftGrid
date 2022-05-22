using ShiftGrid.Test.Shared.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace ShiftGrid.Test.NET.EF
{
    public class MySQLDb: DbContext, DBBase
    {
        public new Database Database { get { return base.Database; } }
        public MySQLDb() : base("name=ShiftGrid_MySQL")
        {
        }
        public virtual DbSet<TestItem> TestItems { get; set; }
        public virtual DbSet<Shared.Models.Type> Types { get; set; }

        System.Threading.Tasks.Task DBBase.SaveChangesAsync()
        {
            return base.SaveChangesAsync();
        }
    }
}
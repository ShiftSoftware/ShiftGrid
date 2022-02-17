using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace ShiftGrid.Test.NET.EF
{
    public class DB : DbContext
    {
        public DB() : base("name=ShiftGrid")
        {

        }

        public virtual DbSet<Models.TestItem> TestItems { get; set; }
        public virtual DbSet<Models.Type> Types { get; set; }
    }
}
using ShiftGrid.Test.Shared.Models;
using System;
using System.Data.Entity;
using ShiftGrid.Test.Shared.Models;

namespace Test.NETFramework.EF
{
    public class DBBase: DbContext
    {
        public DBBase(string connectionString) : base(connectionString)
        {

        }

        public virtual DbSet<TestItem> TestItems { get; set; }
        public virtual DbSet<ShiftGrid.Test.Shared.Models.Type> Types { get; set; }
    }
}

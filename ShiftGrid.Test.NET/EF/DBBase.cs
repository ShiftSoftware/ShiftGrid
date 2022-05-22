using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using ShiftGrid.Test.Shared.Models;

namespace ShiftGrid.Test.NET.EF
{
    public interface DBBase/* : DbContext*/
    {
        //public DBBase(string connectionString): base(connectionString)
        //{

        //}

        Database Database { get; }
        DbSet<TestItem> TestItems { get; set; }
        DbSet<Shared.Models.Type> Types { get; set; }
        System.Threading.Tasks.Task SaveChangesAsync();
    }
}
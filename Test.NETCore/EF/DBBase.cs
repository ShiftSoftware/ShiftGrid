using Microsoft.EntityFrameworkCore;
using ShiftGrid.Test.Shared.Models;

namespace Test.NETCore.EF
{
    public class DBBase : DbContext
    {
        public DBBase()
        {
            this.Logs = new List<string>();
        }

        public List<string> Logs { get; set; }
        public virtual DbSet<TestItem> TestItems { get; set; }
        public virtual DbSet<ShiftGrid.Test.Shared.Models.Type> Types { get; set; }
    }
}

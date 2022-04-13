using System;
using System.Collections.Generic;
using System.Data.Entity.Migrations;
using System.Linq;
using System.Web;

namespace ShiftGrid.Test.NET.MySQLMigrations
{
    internal sealed class Configuration: DbMigrationsConfiguration<ShiftGrid.Test.NET.EF.MySQLDb>
    {
        public Configuration()
        {
            AutomaticMigrationsEnabled = false;
            MigrationsDirectory = "MySQLMigrations";
        }

        protected override void Seed(ShiftGrid.Test.NET.EF.MySQLDb context)
        {
            //  This method will be called after migrating to the latest version.

            //  You can use the DbSet<T>.AddOrUpdate() helper extension method
            //  to avoid creating duplicate seed data.
        }
    }
}
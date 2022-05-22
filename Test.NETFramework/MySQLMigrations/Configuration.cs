using System;
using System.Collections.Generic;
using System.Data.Entity.Migrations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Test.NETFramework.MySQLMigrations
{
    internal sealed class Configuration : DbMigrationsConfiguration<EF.MySQLDb>
    {
        public Configuration()
        {
            AutomaticMigrationsEnabled = false;
            MigrationsDirectory = "MySQLMigrations";
        }
    }
}

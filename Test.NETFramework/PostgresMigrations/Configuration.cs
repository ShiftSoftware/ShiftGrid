using System;
using System.Collections.Generic;
using System.Data.Entity.Migrations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Test.NETFramework.PostgresMigrations
{
    internal sealed class Configuration : DbMigrationsConfiguration<EF.PostgresDB>
    {
        public Configuration()
        {
            AutomaticMigrationsEnabled = false;
            MigrationsDirectory = "PostgresMigrations";
        }
    }
}

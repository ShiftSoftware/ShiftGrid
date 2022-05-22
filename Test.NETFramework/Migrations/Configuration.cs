using System.Data.Entity.Migrations;

namespace Test.NETFramework.Migrations
{
    internal sealed class Configuration : DbMigrationsConfiguration<EF.DB>
    {
        public Configuration()
        {
            AutomaticMigrationsEnabled = false;
        }
    }
}

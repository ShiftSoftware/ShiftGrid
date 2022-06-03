using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace PlayGround.EF
{
    internal class DB : DbContext
    {
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            // Build a config object, using env vars and JSON providers.
            IConfiguration config = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json")
                .Build();

            // Get values from the config given their key and their target type.
            var sqlServerConnectionString = config.GetRequiredSection("Settings").GetConnectionString("SqlServer");
            optionsBuilder.UseSqlServer(sqlServerConnectionString);
        }

        public DbSet<Employee> Employees { get; set; }
        public DbSet<Department> Departments { get; set; }

    }
}

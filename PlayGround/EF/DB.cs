using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace PlayGround.EF
{
    internal class DB : DbContext
    {
        public List<string> Logs { get; set; } = new List<string>();

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            // Build a config object, using env vars and JSON providers.
            IConfiguration config = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json")
                .Build();

            // Get values from the config given their key and their target type.
            var sqlServerConnectionString = config.GetRequiredSection("Settings").GetConnectionString("SqlServer");
            optionsBuilder.UseSqlServer(sqlServerConnectionString);

            optionsBuilder.LogTo((s) =>
            {
                if (s.Contains("Executed DbCommand"))
                    this.Logs.Add(s);
            }, Microsoft.Extensions.Logging.LogLevel.Information);
        }

        public DbSet<Employee> Employees { get; set; }
        public DbSet<Department> Departments { get; set; }

    }
}

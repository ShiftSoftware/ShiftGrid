using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace Test.NETCore.EF
{
    internal class PostgresDB : DBBase
    {
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            // Build a config object, using env vars and JSON providers.
            IConfiguration config = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json")
                .Build();

            // Get values from the config given their key and their target type.
            var PostgresConnectionString = config.GetRequiredSection("Settings").GetConnectionString("Postgres");

            optionsBuilder.UseLazyLoadingProxies();

            optionsBuilder.LogTo((s) =>
            {
                if (s.Contains("Executed DbCommand"))
                    this.Logs.Add(s);
            }, Microsoft.Extensions.Logging.LogLevel.Information);

            optionsBuilder.UseNpgsql(PostgresConnectionString);
        }
    }
}

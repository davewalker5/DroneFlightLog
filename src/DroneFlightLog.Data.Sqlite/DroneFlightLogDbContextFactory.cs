using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;

namespace DroneFlightLog.Data.Sqlite
{
    public class DroneFlightLogDbContextFactory : IDesignTimeDbContextFactory<DroneFlightLogDbContext>
    {
        public DroneFlightLogDbContext CreateDbContext(string[] args)
        {
            // Construct a configuration object that contains the key/value pairs from the settings file
            // at the root of the main application
            IConfigurationRoot configuration = new ConfigurationBuilder()
                                                    .SetBasePath(AppDomain.CurrentDomain.BaseDirectory)
                                                    .AddJsonFile("appsettings.json")
                                                    .Build();

            // Use the configuration object to read the connection string
            var optionsBuilder = new DbContextOptionsBuilder<DroneFlightLogDbContext>();
            optionsBuilder.UseSqlite(configuration.GetConnectionString("DroneLogDb"));

            // Construct and return a database context
            return new DroneFlightLogDbContext(optionsBuilder.Options);
        }
    }
}

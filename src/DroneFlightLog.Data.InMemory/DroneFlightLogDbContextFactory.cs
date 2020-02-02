using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace DroneFlightLog.Data.InMemory
{
    public class DroneFlightLogDbContextFactory : IDesignTimeDbContextFactory<DroneFlightLogDbContext>
    {
        public DroneFlightLogDbContext CreateDbContext(string[] args)
        {
            var optionsBuilder = new DbContextOptionsBuilder<DroneFlightLogDbContext>();
            optionsBuilder.UseInMemoryDatabase(Guid.NewGuid().ToString().Replace("-", ""));
            return new DroneFlightLogDbContext(optionsBuilder.Options);
        }
    }
}

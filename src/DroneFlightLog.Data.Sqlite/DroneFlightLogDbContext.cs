using System.Diagnostics.CodeAnalysis;
using DroneFlightLog.Data.Entities;
using DroneFlightLog.Data.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace DroneFlightLog.Data.Sqlite
{
    [ExcludeFromCodeCoverage]
    public class DroneFlightLogDbContext : DbContext, IDroneFlightLogDbContext
    {
        public DbSet<Operator> Operators { get; set; }
        public DbSet<Address> Addresses { get; set; }

        public DbSet<Drone> Drones { get; set; }
        public DbSet<Model> Models { get; set; }
        public DbSet<Manufacturer> Manufacturers { get; set; }

        public DbSet<Flight> Flights { get; set; }
        public DbSet<FlightProperty> FlightProperties { get; set; }
        public DbSet<FlightPropertyValue> FlightPropertyValues { get; set; }
        public DbSet<Location> Locations { get; set; }

        public DbSet<FlightLogUser> FlightLogUsers { get; set; }

        public DroneFlightLogDbContext(DbContextOptions<DroneFlightLogDbContext> options) : base(options)
        {
        }
    }
}

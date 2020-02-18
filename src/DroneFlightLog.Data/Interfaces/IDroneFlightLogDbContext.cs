using DroneFlightLog.Data.Entities;
using Microsoft.EntityFrameworkCore;

namespace DroneFlightLog.Data.Interfaces
{
    public interface IDroneFlightLogDbContext
    {
        DbSet<Operator> Operators { get; set; }
        DbSet<Address> Addresses { get; set; }

        DbSet<Drone> Drones { get; set; }
        DbSet<Model> Models { get; set; }
        DbSet<Manufacturer> Manufacturers { get; set; }

        DbSet<Flight> Flights { get; set; }
        DbSet<FlightProperty> FlightProperties { get; set; }
        DbSet<FlightPropertyValue> FlightPropertyValues { get; set; }
        DbSet<Location> Locations { get; set; }

        DbSet<FlightLogUser> FlightLogUsers { get; set; }
    }
}

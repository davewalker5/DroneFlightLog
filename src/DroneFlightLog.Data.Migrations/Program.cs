using System;
using DroneFlightLog.Data.Sqlite;
using Microsoft.EntityFrameworkCore;

namespace DroneFlightLog.Data.Migrations
{
    class Program
    {
        static void Main(string[] args)
        {
            DroneFlightLogDbContext context = new DroneFlightLogDbContextFactory().CreateDbContext(null);
            context.Database.Migrate();
        }
    }
}

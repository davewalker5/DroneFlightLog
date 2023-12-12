using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Reflection;
using DroneFlightLog.Data.Entities;
using DroneFlightLog.Data.Factory;
using DroneFlightLog.Data.Interfaces;
using DroneFlightLog.Data.Sqlite;
using DroneFlightLog.Importer.Entities;
using DroneFlightLog.Importer.Logic;
using Microsoft.Extensions.Configuration;

namespace DroneFlightLog.Importer
{
    class Program
    {
        static void Main(string[] args)
        {
            Assembly assembly = Assembly.GetExecutingAssembly();
            FileVersionInfo info = FileVersionInfo.GetVersionInfo(assembly.Location);
            Console.WriteLine($"Drone Flight Log Importer {info.FileVersion}");

            if (args.Length == 1)
            {
                // Read the application settings
                IConfiguration configuration = new ConfigurationBuilder()
                    .AddJsonFile("appsettings.json")
                    .Build();

                IConfigurationSection section = configuration.GetSection("AppSettings");
                AppSettings settings = section.Get<AppSettings>();

                // Create the factory for acessing the SQLite database
                DroneFlightLogDbContext context = new DroneFlightLogDbContextFactory().CreateDbContext(null);
                IDroneFlightLogFactory<DroneFlightLogDbContext> factory = new DroneFlightLogFactory<DroneFlightLogDbContext>(context);

                CsvReader<DroneFlightLogDbContext> reader = null;
                try
                {
                    //  Use the CSV reader to read the flights
                    reader = new CsvReader<DroneFlightLogDbContext>(settings, factory);
                    IList<Flight> flights = reader.Read(args[0]);

                    // Store the flights in the database
                    FlightWriter<DroneFlightLogDbContext> writer = new FlightWriter<DroneFlightLogDbContext>(factory);
                    writer.Save(flights);
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"{ex.Message}");
                    if ((reader != null) && (reader.LastError != null))
                    {
                        Console.WriteLine($"Record {reader.LastError.Record} : {reader.LastError.Message}");
                    }
                }
            }
            else
            {
                string executable = AppDomain.CurrentDomain.FriendlyName;
                Console.WriteLine($"Usage : {executable} path_to_csv_file");
            }
        }
    }
}

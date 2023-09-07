using System;
using System.Diagnostics;
using System.Reflection;
using DroneFlightLog.Data.Factory;
using DroneFlightLog.Data.Sqlite;
using DroneFlightLog.Manager.Entities;
using DroneFlightLog.Manager.Logic;
using Microsoft.EntityFrameworkCore;

namespace DroneFlightLog.Manager
{
    class Program
    {
        static void Main(string[] args)
        {
            Assembly assembly = Assembly.GetExecutingAssembly();
            FileVersionInfo info = FileVersionInfo.GetVersionInfo(assembly.Location);
            Console.WriteLine($"Drone Flight Log Database Management {info.FileVersion}");

            Operation op = new CommandParser().ParseCommandLine(args);
            if (op.Valid)
            {
                DroneFlightLogDbContext context = new DroneFlightLogDbContextFactory().CreateDbContext(null);
                DroneFlightLogFactory<DroneFlightLogDbContext> factory = new DroneFlightLogFactory<DroneFlightLogDbContext>(context);

                try
                {
                    switch (op.Type)
                    {
                        case OperationType.add:
                            factory.Users.AddUser(op.UserName, op.Password);
                            Console.WriteLine($"Added user {op.UserName}");
                            break;
                        case OperationType.setpassword:
                            factory.Users.SetPassword(op.UserName, op.Password);
                            Console.WriteLine($"Set password for user {op.UserName}");
                            break;
                        case OperationType.delete:
                            factory.Users.DeleteUser(op.UserName);
                            Console.WriteLine($"Deleted user {op.UserName}");
                            break;
                        case OperationType.update:
                            context.Database.Migrate();
                            Console.WriteLine($"Applied the latest database migrations");
                            break;
                        default:
                            break;
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error : {ex.Message}");
                }
            }
            else
            {
                string executable = AppDomain.CurrentDomain.FriendlyName;
                Console.WriteLine("Usage:");
                Console.WriteLine($"[1] {executable} add username password");
                Console.WriteLine($"[2] {executable} setpassword username password");
                Console.WriteLine($"[3] {executable} delete username");
                Console.WriteLine($"[4] {executable} update");
            }
        }
    }
}

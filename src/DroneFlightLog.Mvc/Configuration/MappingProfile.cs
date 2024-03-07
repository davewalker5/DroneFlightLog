using AutoMapper;
using DroneFlightLog.Mvc.Entities;
using DroneFlightLog.Mvc.Models;

namespace DroneFlightLog.Mvc.Configuration
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<Manufacturer, AddManufacturerViewModel>();
            CreateMap<Location, AddLocationViewModel>();
            CreateMap<Model, AddModelViewModel>();
            CreateMap<Model, EditModelViewModel>();
            CreateMap<Drone, AddDroneViewModel>();
            CreateMap<Drone, EditDroneViewModel>();
            CreateMap<FlightProperty, AddFlightPropertyViewModel>();
            CreateMap<FlightProperty, EditFlightPropertyViewModel>();
            CreateMap<Flight, AddFlightViewModel>();
            CreateMap<Flight, EditFlightViewModel>();
            CreateMap<Flight, FlightDetailsViewModel>();
        }
    }
}

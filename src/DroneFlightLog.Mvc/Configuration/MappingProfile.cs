﻿using AutoMapper;
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
            CreateMap<Flight, FlightDetailsViewModel>();
        }
    }
}

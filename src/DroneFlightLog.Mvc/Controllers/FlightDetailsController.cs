using System.Collections.Generic;
using System.Threading.Tasks;
using AutoMapper;
using DroneFlightLog.Mvc.Api;
using DroneFlightLog.Mvc.Configuration;
using DroneFlightLog.Mvc.Entities;
using DroneFlightLog.Mvc.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace DroneFlightLog.Mvc.Controllers
{
    [Authorize]
    public class FlightDetailsController : Controller
    {
        private readonly FlightPropertyClient _properties;
        private readonly FlightClient _flights;
        private readonly IOptions<AppSettings> _settings;
        private readonly IMapper _mapper;

        public FlightDetailsController(FlightPropertyClient properties, FlightClient flights, IOptions<AppSettings> settings, IMapper mapper)
        {
            _properties = properties;
            _flights = flights;
            _settings = settings;
            _mapper = mapper;
        }

        /// <summary>
        /// Serve the flight details page
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet]
        public async Task<IActionResult> Index(int id)
        {
            Flight flight = await _flights.GetFlightAsync(id);
            FlightDetailsViewModel model = _mapper.Map<FlightDetailsViewModel>(flight);
            await LoadModelPropertiesAsync(model);
            return View(model);
        }

        /// <summary>
        /// Update the flight property values in response to a POST event
        /// </summary>
        /// <param name="posted"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> Index(FlightDetailsViewModel model)
        {
            // Load the flight details
            Flight flight = await _flights.GetFlightAsync(model.Id);
            _mapper.Map<Flight, FlightDetailsViewModel>(flight, model);

            // The (complex) model properties will not be bound by the model binder
            // and so need to be loaded, here, and updated using the dictionary of
            // property values that are bound by the model binder
            await LoadModelPropertiesAsync(model);

            if (ModelState.IsValid)
            {
                // Apply property values from the bound collection
                model.UpdatePropertiesFromBoundValues();

                // Save newly added property values
                for (int i = 0; i < model.Properties.Count; i++)
                {
                    FlightPropertyValue value = model.Properties[i];
                    if (value.IsNewPropertyValue)
                    {
                        model.Properties[i] = await AddNewPropertyValueAsync(model.Id, model.Properties[i]);
                    }
                    else if (value.Id > 0)
                    {
                        model.Properties[i] = await _properties.UpdateFlightPropertyValueAsync(
                                                                    value.Id,
                                                                    value.NumberValue,
                                                                    value.StringValue,
                                                                    value.DateValue);
                    }
                }
            }

            return View(model);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        private async Task LoadModelPropertiesAsync(FlightDetailsViewModel model)
        {
            // Load any existing flight property values
            model.Properties = await _properties.GetFlightPropertyValuesAsync(model.Id);

            // The model will only contain values for properties where the value
            // has been set. This is not necessarily all available properties but
            // we need controls for all properties to be rendered so we can enter
            // values for them. To achieve this, merge in the list of available
            // properties (with empty values)
            List<FlightProperty> availableProperties = await _properties.GetFlightPropertiesAsync();
            model.MergeProperties(availableProperties);

            model.PropertiesPerRow = _settings.Value.FlightPropertiesPerRow;
        }

        /// <summary>
        /// Create a new flight property value
        /// </summary>
        /// <param name="flightId"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        private async Task<FlightPropertyValue> AddNewPropertyValueAsync(int flightId, FlightPropertyValue value)
        {
            // Create a new property value
            FlightPropertyValue updated;
            switch (value.Property.DataType)
            {
                case FlightPropertyDataType.Date:
                    // TODO Date value creation
                    updated = value;
                    break;
                case FlightPropertyDataType.Number:
                    updated = await _properties.AddFlightPropertyNumberValueAsync(
                                    flightId, value.Property.Id, value.NumberValue);
                    break;
                case FlightPropertyDataType.String:
                    updated = await _properties.AddFlightPropertyStringValueAsync(
                                    flightId, value.Property.Id, value.StringValue);
                    break;
                default:
                    updated = value;
                    break;
            }

            return updated;
        }
    }
}

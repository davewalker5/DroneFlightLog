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
        private readonly DroneFlightLogClient _client;
        private readonly IOptions<AppSettings> _settings;
        private readonly IMapper _mapper;

        public FlightDetailsController(FlightPropertyClient properties, DroneFlightLogClient client, IOptions<AppSettings> settings, IMapper mapper)
        {
            _properties = properties;
            _client = client;
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
            Flight flight = await _client.GetFlightAsync(id);
            FlightDetailsViewModel model = _mapper.Map<FlightDetailsViewModel>(flight);
            await LoadModelProperties(model);
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
            Flight flight = await _client.GetFlightAsync(model.Id);
            _mapper.Map<Flight, FlightDetailsViewModel>(flight, model);

            // The (complex) model properties will not be bound by the model binder
            // and so need to be loaded, here, and updated using the dictionary of
            // property values that are bound by the model binder
            await LoadModelProperties(model);

            if (ModelState.IsValid)
            {
                // Apply property values from the bound collection
                model.UpdatePropertiesFromBoundValues();

                // Save newly added property values
                // foreach (FlightPropertyValue value in model.Properties.Where(p => p.IsNewPropertyValue))
                for (int i = 0; i < model.Properties.Count; i++)
                {
                    FlightPropertyValue value = model.Properties[i];
                    if (value.IsNewPropertyValue)
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
                                                model.Id, value.Property.Id, value.NumberValue);
                                break;
                            case FlightPropertyDataType.String:
                                updated = await _properties.AddFlightPropertyStringValueAsync(
                                                model.Id, value.Property.Id, value.StringValue);
                                break;
                            default:
                                updated = value;
                                break;
                        }

                        model.Properties[i] = updated;
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
        private async Task LoadModelProperties(FlightDetailsViewModel model)
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
    }
}

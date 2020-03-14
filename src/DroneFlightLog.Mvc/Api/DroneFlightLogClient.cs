using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using DroneFlightLog.Mvc.Configuration;
using DroneFlightLog.Mvc.Controllers;
using DroneFlightLog.Mvc.Entities;
using DroneFlightLog.Mvc.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;

namespace DroneFlightLog.Mvc.Api
{
    public class DroneFlightLogClient
    {
        private const string CacheManufacturers = "Manufacturers";
        private const string CacheModels = "Models";
        private const string CacheDrones = "Drones";
        private const string CacheLocations = "Locations";
        private const string CacheOperators = "Operators";
        private const string CacheFlightProperties = "FlightProperties";

        private readonly HttpClient _client;
        private readonly IOptions<AppSettings> _settings;
        private readonly ICacheWrapper _cache;

        public DroneFlightLogClient(HttpClient client, IOptions<AppSettings> settings, IHttpContextAccessor accessor, ICacheWrapper cache)
        {
            _cache = cache;

            // Configure the Http client
            _settings = settings;
            _client = client;
            _client.BaseAddress = new Uri(_settings.Value.ApiUrl);

            // Retrieve the token from session and create the authentication
            // header, if present
            string token = accessor.HttpContext.Session.GetString(LoginController.TokenSessionKey);
            if (!string.IsNullOrEmpty(token))
            {
                SetAuthenticationHeader(token);
            }
        }

        /// <summary>
        /// Authenticate with the service and, if successful, return the JWT token
        /// </summary>
        /// <param name="username"></param>
        /// <param name="password"></param>
        /// <returns></returns>
        public async Task<string> AuthenticateAsync(string username, string password)
        {
            // Construct the JSON containing the user credentials
            dynamic credentials = new { UserName = username, Password = password };
            string jsonCredentials = JsonConvert.SerializeObject(credentials);

            // Send the request. The route is configured in appsettings.json
            string route = _settings.Value.ApiRoutes.First(r => r.Name == "Authenticate").Route;
            StringContent content = new StringContent(jsonCredentials, Encoding.UTF8, "application/json");
            HttpResponseMessage response = await _client.PostAsync(route, content);

            string token = null;
            if (response.IsSuccessStatusCode)
            {
                // Read the token from the response body and set up the default request
                // authentication header
                token = await response.Content.ReadAsStringAsync();
                SetAuthenticationHeader(token);
            }

            return token;
        }

        /// <summary>
        /// Return a list of manufacturers
        /// </summary>
        /// <returns></returns>
        public async Task<List<Manufacturer>> GetManufacturersAsync()
        {
            List<Manufacturer> manufacturers = _cache.Get<List<Manufacturer>>(CacheManufacturers);
            if (manufacturers == null)
            {
                string json = await GetIndirectAsync("Manufacturers");
                manufacturers = JsonConvert.DeserializeObject<List<Manufacturer>>(json);
                _cache.Set(CacheManufacturers, manufacturers, _settings.Value.CacheLifetimeSeconds);
            }
            return manufacturers;
        }

        /// <summary>
        /// Create a new manufacturer
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public async Task<Manufacturer> AddManufacturerAsync(string name)
        {
            _cache.Remove(CacheManufacturers);
            string data = $"\"{name}\"";
            string json = await PostIndirectAsync("Manufacturers", data);
            Manufacturer manufacturer = JsonConvert.DeserializeObject<Manufacturer>(json);
            return manufacturer;
        }

        /// <summary>
        /// Return a list of drone models
        /// </summary>
        /// <returns></returns>
        public async Task<List<Model>> GetModelsAsync()
        {
            List<Model> models = _cache.Get<List<Model>>(CacheModels);
            if (models == null)
            {
                string json = await GetIndirectAsync("Models");
                models = JsonConvert.DeserializeObject<List<Model>>(json);
                _cache.Set(CacheModels, models, _settings.Value.CacheLifetimeSeconds);
            }
            return models;
        }

        /// <summary>
        /// Create a new model
        /// </summary>
        /// <param name="name"></param>
        /// <param name="manufacturerId"></param>
        /// <returns></returns>
        public async Task<Model> AddModelAsync(string name, int manufacturerId)
        {
            _cache.Remove(CacheModels);

            dynamic template = new
            {
                Name = name,
                ManufacturerId = manufacturerId
            };

            string data = JsonConvert.SerializeObject(template);
            string json = await PostIndirectAsync("Models", data);

            Model model = JsonConvert.DeserializeObject<Model>(json);
            return model;
        }

        /// <summary>
        /// Return a list of drones
        /// </summary>
        /// <returns></returns>
        public async Task<List<Drone>> GetDronesAsync()
        {
            List<Drone> drones = _cache.Get<List<Drone>>(CacheDrones);
            if (drones == null)
            {
                string json = await GetIndirectAsync("Drones");
                drones = JsonConvert.DeserializeObject<List<Drone>>(json);
                _cache.Set(CacheDrones, drones, _settings.Value.CacheLifetimeSeconds);
            }
            return drones;
        }

        /// <summary>
        /// Create a new drone
        /// </summary>
        /// <param name="name"></param>
        /// <param name="serialNumber"></param>
        /// <param name="modelId"></param>
        /// <returns></returns>
        public async Task<Drone> AddDroneAsync(string name, string serialNumber, int modelId)
        {
            _cache.Remove(CacheDrones);

            dynamic template = new
            {
                Name = name,
                SerialNumber = serialNumber,
                ModelId = modelId
            };

            string data = JsonConvert.SerializeObject(template);
            string json = await PostIndirectAsync("Drones", data);

            Drone drone = JsonConvert.DeserializeObject<Drone>(json);
            return drone;
        }

        /// <summary>
        /// Return a list of locations
        /// </summary>
        /// <returns></returns>
        public async Task<List<Location>> GetLocationsAsync()
        {
            List<Location> locations = _cache.Get<List<Location>>(CacheLocations);
            if (locations == null)
            {
                string json = await GetIndirectAsync("Locations");
                locations = JsonConvert.DeserializeObject<List<Location>>(json);
                _cache.Set(CacheLocations, locations, _settings.Value.CacheLifetimeSeconds);
            }
            return locations;
        }

        /// <summary>
        /// Create a new location
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public async Task<Location> AddLocationAsync(string name)
        {
            _cache.Remove(CacheLocations);
            string data = $"\"{name}\"";
            string json = await PostIndirectAsync("Locations", data);
            Location location = JsonConvert.DeserializeObject<Location>(json);
            return location;
        }

        /// <summary>
        /// Return a list of operator details
        /// </summary>
        /// <returns></returns>
        public async Task<List<Operator>> GetOperatorsAsync()
        {
            List<Operator> operators = _cache.Get<List<Operator>>(CacheOperators);
            if  (operators == null)
            {
                string json = await GetIndirectAsync("Operators");
                operators = JsonConvert.DeserializeObject<List<Operator>>(json);
                _cache.Set(CacheOperators, operators, _settings.Value.CacheLifetimeSeconds);
            }
            return operators;
        }

        /// <summary>
        /// Create a new operator
        /// </summary>
        /// <param name="firstNames"></param>
        /// <param name="surname"></param>
        /// <param name="operatorNumber"></param>
        /// <param name="flyerNumber"></param>
        /// <param name="doB"></param>
        /// <param name="addressId"></param>
        /// <returns></returns>
        public async Task<Operator> AddOperatorAsync(string firstNames, string surname, string operatorNumber, string flyerNumber, DateTime doB, int addressId)
        {
            _cache.Remove(CacheOperators);

            dynamic template = new
            {
                FirstNames = firstNames,
                Surname = surname,
                OperatorNumber = operatorNumber,
                FlyerNumber = flyerNumber,
                DoB = doB,
                AddressId = addressId
            };

            string data = JsonConvert.SerializeObject(template);
            string json = await PostIndirectAsync("Operators", data);

            Operator op = JsonConvert.DeserializeObject<Operator>(json);
            return op;
        }

        /// <summary>
        /// Return a list of available flight properties
        /// </summary>
        /// <returns></returns>
        public async Task<List<FlightProperty>> GetFlightPropertiesAsync()
        {
            List<FlightProperty> properties = _cache.Get<List<FlightProperty>>(CacheFlightProperties);
            if (properties == null)
            {
                string json = await GetIndirectAsync("FlightProperties");
                properties = JsonConvert.DeserializeObject<List<FlightProperty>>(json);
                _cache.Set(CacheFlightProperties, properties, _settings.Value.CacheLifetimeSeconds);
            }
            return properties;
        }

        /// <summary>
        /// Create a new flight property
        /// </summary>
        /// <param name="name"></param>
        /// <param name="dataType"></param>
        /// <param name="isSingleInstance"></param>
        /// <returns></returns>
        public async Task<FlightProperty> AddFlightPropertyAsync(string name, int dataType, bool isSingleInstance)
        {
            _cache.Remove(CacheFlightProperties);

            dynamic template = new
            {
                Name = name,
                DataType = dataType,
                IsSingleInstance = isSingleInstance
            };

            string data = JsonConvert.SerializeObject(template);
            string json = await PostIndirectAsync("FlightProperties", data);

            FlightProperty property = JsonConvert.DeserializeObject<FlightProperty>(json);
            return property;
        }

        /// <summary>
        /// Return a list of flight property values for a specified flight
        /// </summary>
        /// <param name="flightId"></param>
        /// <returns></returns>
        public async Task<List<FlightPropertyValue>> GetFlightPropertyValuesAsync(int flightId)
        {
            string baseRoute = _settings.Value.ApiRoutes.First(r => r.Name == "FlightPropertyValues").Route;
            string route = $"{baseRoute}/{flightId}";
            string json = await GetDirectAsync(route);
            List<FlightPropertyValue> values = JsonConvert.DeserializeObject<List<FlightPropertyValue>>(json);
            return values;
        }

        /// <summary>
        /// Create a new numeric property value
        /// </summary>
        /// <param name="flightId"></param>
        /// <param name="propertyId"></param>
        /// <param name="propertyValue"></param>
        /// <returns></returns>
        public async Task<FlightPropertyValue> AddFlightPropertyNumberValueAsync(int flightId, int propertyId, decimal? propertyValue)
        {
            dynamic template =  new
            {
                FlightId = flightId,
                PropertyId = propertyId,
                NumberValue = propertyValue
            };

            string data = JsonConvert.SerializeObject(template);
            string json = await PostIndirectAsync("FlightPropertyValues", data);

            FlightPropertyValue value = JsonConvert.DeserializeObject<FlightPropertyValue>(json);
            return value;
        }

        /// <summary>
        /// Create a new string property value
        /// </summary>
        /// <param name="flightId"></param>
        /// <param name="propertyId"></param>
        /// <param name="propertyValue"></param>
        /// <returns></returns>
        public async Task<FlightPropertyValue> AddFlightPropertyStringValueAsync(int flightId, int propertyId, string propertyValue)
        {
            dynamic template = new
            {
                FlightId = flightId,
                PropertyId = propertyId,
                StringValue = propertyValue
            };

            string data = JsonConvert.SerializeObject(template);
            string json = await PostIndirectAsync("FlightPropertyValues", data);

            FlightPropertyValue value = JsonConvert.DeserializeObject<FlightPropertyValue>(json);
            return value;
        }

        /// <summary>
        /// Return the specified page of flights
        /// </summary>
        /// <param name="page"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        public async Task<List<Flight>> GetFlightsAsync(int page, int pageSize)
        {
            string baseRoute = _settings.Value.ApiRoutes.First(r => r.Name == "Flights").Route;
            string route = $"{baseRoute}/{page}/{pageSize}";
            string json = await GetDirectAsync(route);
            List<Flight> flight = JsonConvert.DeserializeObject<List<Flight>>(json);
            return flight;
        }

        /// <summary>
        /// Retrieve a single flight given its ID
        /// </summary>
        /// <param name="flightId"></param>
        /// <returns></returns>
        public async Task<Flight> GetFlightAsync(int flightId)
        {
            // TODO : This needs to be replaced with a call to retrieve a single flight
            // by Id. For now, retrieve an arbitrary large number that will cover them
            // all then pick the one required
            List<Flight> flights = await GetFlightsAsync(1, 1000000);
            Flight flight = flights.First(f => f.Id == flightId);
            return flight;
        }

        /// <summary>
        /// Return the specified page of flights, filtered by operator
        /// </summary>
        /// <param name="operatorId"></param>
        /// <param name="page"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        public async Task<List<Flight>> GetFlightsByOperatorAsync(int operatorId, int page, int pageSize)
        {
            string baseRoute = _settings.Value.ApiRoutes.First(r => r.Name == "Flights").Route;
            string route = $"{baseRoute}/operator/{operatorId}/{page}/{pageSize}";
            string json = await GetDirectAsync(route);
            List<Flight> flight = JsonConvert.DeserializeObject<List<Flight>>(json);
            return flight;
        }

        /// <summary>
        /// Return the specified page of flights, filtered by drone
        /// </summary>
        /// <param name="droneId"></param>
        /// <param name="page"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        public async Task<List<Flight>> GetFlightsByDroneAsync(int droneId, int page, int pageSize)
        {
            string baseRoute = _settings.Value.ApiRoutes.First(r => r.Name == "Flights").Route;
            string route = $"{baseRoute}/drone/{droneId}/{page}/{pageSize}";
            string json = await GetDirectAsync(route);
            List<Flight> flight = JsonConvert.DeserializeObject<List<Flight>>(json);
            return flight;
        }

        /// <summary>
        /// Return the specified page of flights, filtered by location
        /// </summary>
        /// <param name="locationId"></param>
        /// <param name="page"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        public async Task<List<Flight>> GetFlightsByLocationAsync(int locationId, int page, int pageSize)
        {
            string baseRoute = _settings.Value.ApiRoutes.First(r => r.Name == "Flights").Route;
            string route = $"{baseRoute}/location/{locationId}/{page}/{pageSize}";
            string json = await GetDirectAsync(route);
            List<Flight> flight = JsonConvert.DeserializeObject<List<Flight>>(json);
            return flight;
        }

        /// <summary>
        /// Return the specified page of flights, filtered by location
        /// </summary>
        /// <param name="start"></param>
        /// <param name="end"></param>
        /// <param name="page"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        public async Task<List<Flight>> GetFlightsByDateAsync(DateTime start, DateTime end, int page, int pageSize)
        {
            string baseRoute = _settings.Value.ApiRoutes.First(r => r.Name == "Flights").Route;
            string startDateSegment = HttpUtility.UrlEncode(start.ToString(_settings.Value.ApiDateFormat));
            string endDateSegment = HttpUtility.UrlEncode(end.ToString(_settings.Value.ApiDateFormat));
            string route = $"{baseRoute}/date/{startDateSegment}/{endDateSegment}/{page}/{pageSize}";

            string json = await GetDirectAsync(route);
            List<Flight> flight = JsonConvert.DeserializeObject<List<Flight>>(json);
            return flight;
        }

        /// <summary>
        /// Create a new flight
        /// </summary>
        /// <param name="droneId"></param>
        /// <param name="locationId"></param>
        /// <param name="operatorId"></param>
        /// <param name="start"></param>
        /// <param name="end"></param>
        /// <returns></returns>
        public async Task<Flight> AddFlightAsync(int droneId, int locationId, int operatorId, DateTime start, DateTime end)
        {
            dynamic template = new
            {
                LocationId = locationId,
                OperatorId = operatorId,
                DroneId = droneId,
                Start = start,
                End = end
            };

            string data = JsonConvert.SerializeObject(template);
            string json = await PostIndirectAsync("Flights", data);

            Flight flight = JsonConvert.DeserializeObject<Flight>(json);
            return flight;
        }

        /// <summary>
        /// Find and return a matching address or create a new one if not found
        /// </summary>
        /// <param name="number"></param>
        /// <param name="street"></param>
        /// <param name="town"></param>
        /// <param name="county"></param>
        /// <param name="postcode"></param>
        /// <param name="country"></param>
        /// <returns></returns>
        public async Task<Address> FindOrAddAddressAsync(string number, string street, string town, string county, string postcode, string country)
        {
            Address address = await FindAddressAsync(number, postcode, country);
            if (address == null)
            {
                address = await AddAddressAsync(number, street, town, county, postcode, country);
            }

            return address;
        }

        /// <summary>
        /// Find and return an address, returning null if it doesn't exist
        /// </summary>
        /// <param name="number"></param>
        /// <param name="postcode"></param>
        /// <param name="country"></param>
        /// <returns></returns>
        private async Task<Address> FindAddressAsync(string number, string postcode, string country)
        {
            string baseRoute = _settings.Value.ApiRoutes.First(r => r.Name == "Addresses").Route;
            string numberSegment = HttpUtility.UrlEncode(number);
            string postcodeSegment = HttpUtility.UrlEncode(postcode);
            string countrySegment = HttpUtility.UrlEncode(country);
            string route = $"{baseRoute}/{numberSegment}/{postcodeSegment}/{countrySegment}";

            string json = await GetDirectAsync(route);
            Address address = (json != null) ? JsonConvert.DeserializeObject<Address>(json) : null;
            return address;
        }

        /// <summary>
        /// Create a new address
        /// </summary>
        /// <param name="number"></param>
        /// <param name="street"></param>
        /// <param name="town"></param>
        /// <param name="county"></param>
        /// <param name="postcode"></param>
        /// <param name="country"></param>
        /// <returns></returns>
        private async Task<Address> AddAddressAsync(string number, string street, string town, string county, string postcode, string country)
        {
            dynamic template = new
            {
                Number = number,
                Street = street,
                Town = town,
                County = county,
                Postcode = postcode,
                Country = country
            };

            string data = JsonConvert.SerializeObject(template);
            string json = await PostIndirectAsync("Addresses", data);

            Address address = JsonConvert.DeserializeObject<Address>(json);
            return address;
        }

        /// <summary>
        /// Given a route name, retrieve the route value (read from the application
        /// settings) and use the "direct" method to read the JSON response
        /// </summary>
        /// <param name="routeName"></param>
        /// <returns></returns>
        private async Task<string> GetIndirectAsync(string routeName)
        {
            string route = _settings.Value.ApiRoutes.First(r => r.Name == routeName).Route;
            return await GetDirectAsync(route);
        }

        /// <summary>
        /// Add the authorization header to the default request headers
        /// </summary>
        /// <param name="token"></param>
        private void SetAuthenticationHeader(string token)
        {
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
        }

        /// <summary>
        /// Given a route, use GET to retrieve data and return the response content
        /// as a JSON string
        /// </summary>
        /// <param name="route"></param>
        /// <returns></returns>
        private async Task<string> GetDirectAsync(string route)
        {
            string json = null;

            HttpResponseMessage response = await _client.GetAsync(route);
            if (response.IsSuccessStatusCode)
            {
                // Read the response body
                json = await response.Content.ReadAsStringAsync();
            }

            return json;
        }

        /// <summary>
        /// Given a route name, retrieve the route value (read from the application
        /// settings), POST the data to it, await the response and return the response
        /// content as a JSON string
        /// </summary>
        /// <param name="routeName"></param>
        /// <param name="data"></param>
        /// <returns></returns>
        private async Task<string> PostIndirectAsync(string routeName, string data)
        {
            string json = null;

            // Get the route and create the message content
            string route = _settings.Value.ApiRoutes.First(r => r.Name == routeName).Route;
            StringContent content = new StringContent(data, Encoding.UTF8, "application/json");

            // Send the request
            HttpResponseMessage response = await _client.PostAsync(route, content);
            if (response.IsSuccessStatusCode)
            {
                // Read the response body
                json = await response.Content.ReadAsStringAsync();
            }

            return json;
        }
    }
}

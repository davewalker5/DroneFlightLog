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
        /// Return a list of locations
        /// </summary>
        /// <returns></returns>
        public async Task<List<Location>> GetLocationsAsync()
        {
            List<Location> locations = _cache.Get<List<Location>>(CacheLocations);
            if (locations == null)
            {
                string json = await SendIndirectAsync("Locations", null, HttpMethod.Get);
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
            string json = await SendIndirectAsync("Locations", data, HttpMethod.Post);
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
                string json = await SendIndirectAsync("Operators", null, HttpMethod.Get);
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
            string json = await SendIndirectAsync("Operators", data, HttpMethod.Post);

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
                string json = await SendIndirectAsync("FlightProperties", null, HttpMethod.Get);
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
            string json = await SendIndirectAsync("FlightProperties", data, HttpMethod.Post);

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
            string json = await SendDirectAsync(route, null, HttpMethod.Get);
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
            string json = await SendIndirectAsync("FlightPropertyValues", data, HttpMethod.Post);

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
            string json = await SendIndirectAsync("FlightPropertyValues", data, HttpMethod.Post);

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
            string json = await SendDirectAsync(route, null, HttpMethod.Get);
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
            string json = await SendDirectAsync(route, null, HttpMethod.Get);
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
            string json = await SendDirectAsync(route, null, HttpMethod.Get);
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
            string json = await SendDirectAsync(route, null, HttpMethod.Get);
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

            string json = await SendIndirectAsync(route, null, HttpMethod.Get);
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
            string json = await SendIndirectAsync("Flights", data, HttpMethod.Post);

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

            string json = await SendDirectAsync(route, null, HttpMethod.Get);
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
            string json = await SendIndirectAsync("Addresses", data, HttpMethod.Post);

            Address address = JsonConvert.DeserializeObject<Address>(json);
            return address;
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
        /// Given a route name, some data (null in the case of GET) and an HTTP method,
        /// look up the route from the application settings then send the request to
        /// the service and return the JSON response
        /// </summary>
        /// <param name="routeName"></param>
        /// <param name="data"></param>
        /// <param name="method"></param>
        /// <returns></returns>
        private async Task<string> SendIndirectAsync(string routeName, string data, HttpMethod method)
        {
            string route = _settings.Value.ApiRoutes.First(r => r.Name == routeName).Route;
            string json = await SendDirectAsync(route, data, method);
            return json;
        }

        /// <summary>
        /// Given a route, some data (null in the case of GET) and an HTTP method,
        /// send the request to the service and return the JSON response
        /// </summary>
        /// <param name="route"></param>
        /// <param name="data"></param>
        /// <param name="method"></param>
        /// <returns></returns>
        private async Task<string> SendDirectAsync(string route, string data, HttpMethod method)
        {
            string json = null;

            HttpResponseMessage response = null;
            if (method == HttpMethod.Get)
            {
                response = await _client.GetAsync(route);
            }
            else if (method == HttpMethod.Post)
            {
                StringContent content = new StringContent(data, Encoding.UTF8, "application/json");
                response = await _client.PostAsync(route, content);
            }
            else if (method == HttpMethod.Put)
            {
                StringContent content = new StringContent(data, Encoding.UTF8, "application/json");
                response = await _client.PutAsync(route, content);
            }

            if ((response != null) && response.IsSuccessStatusCode)
            {
                json = await response.Content.ReadAsStringAsync();
            }

            return json;
        }
    }
}

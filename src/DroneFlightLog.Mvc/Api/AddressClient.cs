using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web;
using DroneFlightLog.Mvc.Configuration;
using DroneFlightLog.Mvc.Entities;
using DroneFlightLog.Mvc.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;

namespace DroneFlightLog.Mvc.Api
{
    public class AddressClient : DroneFlightLogClientBase
    {
        private const string RouteKey = "Addresses";

        public AddressClient(HttpClient client, IOptions<AppSettings> settings, IHttpContextAccessor accessor, ICacheWrapper cache)
            : base(client, settings, accessor, cache)
        {
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
            address ??= await AddAddressAsync(number, street, town, county, postcode, country);
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
            string baseRoute = _settings.Value.ApiRoutes.First(r => r.Name == RouteKey).Route;
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
            string json = await SendIndirectAsync(RouteKey, data, HttpMethod.Post);

            Address address = JsonConvert.DeserializeObject<Address>(json);
            return address;
        }
    }
}

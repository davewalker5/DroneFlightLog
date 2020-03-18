using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using DroneFlightLog.Mvc.Configuration;
using DroneFlightLog.Mvc.Entities;
using DroneFlightLog.Mvc.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;

namespace DroneFlightLog.Mvc.Api
{
    public class OperatorClient : DroneFlightLogClientBase
    {
        private const string RouteKey = "Operators";
        private const string CacheKey = "Operators";

        public OperatorClient(HttpClient client, IOptions<AppSettings> settings, IHttpContextAccessor accessor, ICacheWrapper cache)
            : base(client, settings, accessor, cache)
        {
        }

        /// <summary>
        /// Return a list of operator details
        /// </summary>
        /// <returns></returns>
        public async Task<List<Operator>> GetOperatorsAsync()
        {
            List<Operator> operators = _cache.Get<List<Operator>>(CacheKey);
            if (operators == null)
            {
                string json = await SendIndirectAsync(RouteKey, null, HttpMethod.Get);
                operators = JsonConvert.DeserializeObject<List<Operator>>(json);
                _cache.Set(CacheKey, operators, _settings.Value.CacheLifetimeSeconds);
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
            _cache.Remove(CacheKey);

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
            string json = await SendIndirectAsync(RouteKey, data, HttpMethod.Post);

            Operator op = JsonConvert.DeserializeObject<Operator>(json);
            return op;
        }
    }
}

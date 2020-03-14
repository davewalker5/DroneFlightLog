using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using DroneFlightLog.Mvc.Entities;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace DroneFlightLog.Mvc.Binders
{
    public class FlightPropertyModelBinder : IModelBinder
    {
        // This regular expression will match FPV_x, where "x" is an integer,
        // which is the form of the names for the property value inputs
        private static readonly Regex _regex = new Regex(
                $"^{FlightPropertyValue.ControlNamePrefix}[0-9]+$",
                RegexOptions.Compiled);

        public Task BindModelAsync(ModelBindingContext bindingContext)
        {
            // Determine the list of keys in the form collection that are matches
            // for the property value pattern
            IFormCollection form = bindingContext.HttpContext.Request.Form;
            IEnumerable<string> propertyValueKeys = form.Keys.Where(k => _regex.IsMatch(k));

            // Iterate over the matching keys and produce a dictionary of property
            // ID versus value string from the form
            Dictionary<int, string> model = new Dictionary<int, string>();
            foreach (string key in propertyValueKeys)
            {
                string propertyIdString = key.Replace(FlightPropertyValue.ControlNamePrefix, "");
                int propertyId = int.Parse(propertyIdString);
                model.Add(propertyId, form[key]);
            }

            bindingContext.Result = ModelBindingResult.Success(model);
            return Task.CompletedTask;
        }
    }
}

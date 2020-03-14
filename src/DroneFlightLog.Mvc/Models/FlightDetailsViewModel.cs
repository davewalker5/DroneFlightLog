using System.Collections.Generic;
using System.Linq;
using DroneFlightLog.Mvc.Binders;
using DroneFlightLog.Mvc.Entities;
using Microsoft.AspNetCore.Mvc;

namespace DroneFlightLog.Mvc.Models
{
    public class FlightDetailsViewModel : Flight
    {
        [BindProperty(BinderType = typeof(FlightPropertyModelBinder))]
        public Dictionary<int,string> BoundPropertyValues { get; set; }

        public int PropertiesPerRow { get; set; }

        /// <summary>
        /// Make sure all available flight properties are represented in the view model
        /// </summary>
        /// <param name="availableProperties"></param>
        public void MergeProperties(List<FlightProperty> availableProperties)
        {
            // Identify the list of properties that don't already have values
            // associated with the flight
            IEnumerable<int> associatedPropertyIds = Properties.Select(p => p.Property.Id);
            IEnumerable<FlightProperty> missing = availableProperties.Where(ap => !associatedPropertyIds.Contains(ap.Id));

            // Add an empty value for available but not associated properties
            foreach (FlightProperty property in missing)
            {
                Properties.Add(new FlightPropertyValue
                {
                    Property = property
                });
            }

            // Sort the properties by name
            Properties = Properties.OrderBy(p => p.Property.Name).ToList();
        }

        /// <summary>
        /// Identify properties that have not already been saved and apply the
        /// updated values to them from the bound property values dictionary
        /// </summary>
        public void UpdatePropertiesFromBoundValues()
        {
            foreach (var kvp in BoundPropertyValues.Where(v => !string.IsNullOrEmpty(v.Value)))
            {
                FlightPropertyValue value = Properties.FirstOrDefault(p => (p.Id == 0) && (p.Property.Id == kvp.Key));
                if (value != null)
                {
                    switch (value.Property.DataType)
                    {
                        case FlightPropertyDataType.Date:
                            // TODO : Add date parsing
                            break;
                        case FlightPropertyDataType.Number:
                            value.NumberValue = decimal.Parse(kvp.Value);
                            break;
                        case FlightPropertyDataType.String:
                            value.StringValue = kvp.Value;
                            break;
                        default:
                            break;
                    }
                }
            }
        }
    }
}

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
            // associated with the flight and add entries to the properties list
            if (Properties != null)
            {
                IEnumerable<int> associatedPropertyIds = Properties.Select(p => p.Property.Id);
                IEnumerable<FlightProperty> missing = availableProperties.Where(ap => !associatedPropertyIds.Contains(ap.Id)).ToList();

                // Add an empty value for available but not associated properties
                if (missing.Any())
                {
                    Properties.AddRange(missing.Select(m => new FlightPropertyValue { Property = m }));
                }
            }
            else
            {
                Properties = availableProperties?.Select(m => new FlightPropertyValue { Property = m }).ToList();
            }

            // Sort the properties by name
            Properties = Properties?.OrderBy(p => p.Property.Name).ToList();
        }

        /// <summary>
        /// Update the model's flight property values from the bound property
        /// values dictionary
        /// </summary>
        public void UpdatePropertiesFromBoundValues()
        {
            // Iterate over the bound values
            foreach (var kvp in BoundPropertyValues)
            {
                // Find the property value that is associated with the bound value property
                FlightPropertyValue value = Properties.FirstOrDefault(p => p.Property.Id == kvp.Key);

                // Update the value if either it corresponds to a value that's already
                // been saved (updating) or the associated bound value isn't empty (creating)
                if ((value != null) && ((value.Id > 0) || !string.IsNullOrEmpty(kvp.Value)))
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

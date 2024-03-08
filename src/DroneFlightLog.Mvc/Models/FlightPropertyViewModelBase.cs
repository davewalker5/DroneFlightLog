using System;
using System.Collections.Generic;
using System.Linq;
using DroneFlightLog.Mvc.Entities;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace DroneFlightLog.Mvc.Models
{
    public abstract class FlightPropertyViewModelBase : FlightProperty
    {
        public List<SelectListItem> DataTypes { get; set; }

        /// <summary>
        /// Default constructor - build the data type list from the enumeration
        /// </summary>
        public FlightPropertyViewModelBase()
        {
            // Add the default selection, which is empty
            DataTypes = new List<SelectListItem>()
            {
                new SelectListItem{ Value = "", Text = "" }
            };

            // Add the data types from the enumeration
            IEnumerable<FlightPropertyDataType> types =
                    Enum.GetValues(typeof(FlightPropertyDataType))
                        .Cast<FlightPropertyDataType>();

            DataTypes.AddRange(types.Select(t =>
                                new SelectListItem
                                {
                                    Value = ((int)t).ToString(),
                                    Text = t.ToString()
                                }));
        }
    }
}

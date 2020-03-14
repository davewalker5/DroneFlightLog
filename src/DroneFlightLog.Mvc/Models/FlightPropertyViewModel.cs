using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using DroneFlightLog.Mvc.Entities;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace DroneFlightLog.Mvc.Models
{
    public class FlightPropertyViewModel : FlightProperty
    {
        public List<SelectListItem> DataTypes { get; private set; }
        public string Message { get; set; }

        [DisplayName("Data Type")]
        [Range(0, 2, ErrorMessage = "You must select a data type")]
        public int DataTypeValue { get; set; }

        /// <summary>
        /// Default constructor - build the data type list from the enumeration
        /// </summary>
        public FlightPropertyViewModel()
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

        /// <summary>
        /// Reset the view model
        /// </summary>
        public void Clear()
        {
            Id = 0;
            DataTypeValue = 0;
            Name = "";
            IsSingleInstance = false;
            Message = "";
        }
    }
}

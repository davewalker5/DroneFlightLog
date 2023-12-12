using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using DroneFlightLog.Mvc.Entities;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace DroneFlightLog.Mvc.Models
{
    public class FlightSearchByOperatorViewModel : FlightSearchBaseViewModel
    {
        [DisplayName("Operator")]
        [Range(1, int.MaxValue, ErrorMessage = "You must select an operator")]
        [Required]
        public int OperatorId { get; set; }

        public List<SelectListItem> Operators { get; private set; }

        /// <summary>
        /// Set the options for the operators drop-down list
        /// </summary>
        /// <param name="operators"></param>
        public void SetOperators(List<Operator> operators)
        {
            // Add the default selection, which is empty
            Operators = new List<SelectListItem>()
            {
                new SelectListItem{ Value = "", Text = "" }
            };

            // Add the operators retrieved from the service
            Operators.AddRange(operators.Select(o =>
                                new SelectListItem
                                {
                                    Value = o.Id.ToString(),
                                    Text = $"{o.FirstNames} {o.Surname} - {o.OperatorNumber}"
                                }));
        }
    }
}

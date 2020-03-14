using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using DroneFlightLog.Mvc.Entities;

namespace DroneFlightLog.Mvc.Models
{
    public class OperatorViewModel
    {
        public Operator Operator { get; set; }
        public Address Address { get; set; }

        [DisplayName("Date of Birth")]
        [Required(ErrorMessage = "You must provide a date of birth")]
        public string OperatorDateOfBirth { get; set; }

        public string Message { get; set; }

        public OperatorViewModel()
        {
            Clear();
        }

        public void Clear()
        {
            Operator = new Operator();
            Address = new Address();
            Message = "";
        }
    }
}

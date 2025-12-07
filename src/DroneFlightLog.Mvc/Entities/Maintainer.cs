using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace DroneFlightLog.Mvc.Entities
{
    public class Maintainer
    {
        public int Id { get; set; }

        [DisplayName("First Names")]
        [Required(ErrorMessage = "You must provide a first name")]
        public string FirstNames { get; set; }

        [DisplayName("Surname")]
        [Required(ErrorMessage = "You must provide a surname")]
        public string Surname { get; set; }

        /// <summary>
        /// Return the full name of the maintainer
        /// </summary>
        /// <returns></returns>
        public string MaintainerFullName
        {
            get
            {
                return $"{FirstNames} {Surname}";
            }
        }
    }
}

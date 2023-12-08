using System;
using System.ComponentModel.DataAnnotations;

namespace Space_Weather.Models
{
    public class Weather
    {
        public int Id { get; set; }

        [Required]
        [StringLength(100, ErrorMessage = "Forecast cannot be longer than 100 characters.")]
        public string Forecast { get; set; }

        public DateTime Date { get; set; }
        public int PlanetId { get; set; }
        public Planet Planet { get; set; }
    }
}

using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Airline.Web.Models
{
    public class Flight
    {
        public int Id { get; set; }
        
        [Required]
        public string FlightNumber { get; set; } = string.Empty;
        
        public DateTime DepartureTime { get; set; }
        public DateTime ArrivalTime { get; set; }
        
        // Внешние ключи
        public string DepartureAirportCode { get; set; } = string.Empty;
        public string ArrivalAirportCode { get; set; } = string.Empty;
        
        public int AvailableSeats { get; set; }
        public string Status { get; set; } = "Scheduled";

        // Навигационные свойства
        [ForeignKey(nameof(DepartureAirportCode))]
        public Airport? DepartureAirport { get; set; }

        [ForeignKey(nameof(ArrivalAirportCode))]
        public Airport? ArrivalAirport { get; set; }
    }
}
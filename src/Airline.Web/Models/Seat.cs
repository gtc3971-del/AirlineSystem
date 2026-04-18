namespace Airline.Web.Models
{
    public class Seat
    {
        public int Id { get; set; }
        public int FlightId { get; set; }
        public string SeatNumber { get; set; } = string.Empty; // "1A", "12C"
        public string Status { get; set; } = "Available"; // "Available", "Booked", "Occupied"
        
        public Flight? Flight { get; set; }
    }
}
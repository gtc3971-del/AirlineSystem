namespace Airline.Web.Models
{
    public class Booking
    {
        public int Id { get; set; }
        public string UserId { get; set; } = string.Empty; // Ссылка на AirlineUser.Id
        public DateTime BookingDate { get; set; } = DateTime.UtcNow;
        public string Status { get; set; } = "Pending"; // Pending, Confirmed, Cancelled
    }
}
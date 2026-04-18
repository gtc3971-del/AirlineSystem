namespace Airline.Web.Models
{
    public class Aircraft
    {
        public int Id { get; set; }
        public string TailNumber { get; set; } = string.Empty;
        public string Model { get; set; } = string.Empty;
        public int TotalSeats { get; set; }
        public string TechnicalStatus { get; set; } = "Active";
    }
}
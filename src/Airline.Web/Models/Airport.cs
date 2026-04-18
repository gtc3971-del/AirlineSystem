namespace Airline.Web.Models
{
    public class Airport
    {
        public int Id { get; set; }
        public string Code { get; set; } = string.Empty; // IATA код (SVO, LED и т.д.)
        public string Name { get; set; } = string.Empty;
        public string City { get; set; } = string.Empty;
        public string Country { get; set; } = string.Empty;
    }
}
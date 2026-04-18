using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema; // <--- Обязательно!

namespace Airline.Web.Models
{
    public class Ticket
    {
        public int Id { get; set; }
        
        // === СВЯЗЬ С РЕЙСОМ ===
        public int FlightId { get; set; }
        
        [ForeignKey(nameof(FlightId))] // <--- Явно указываем, что это внешний ключ
        public Flight? Flight { get; set; }
        
        // === СВЯЗЬ С ПАССАЖИРОМ ===
        public int PassengerId { get; set; }
        
        [ForeignKey(nameof(PassengerId))] // <--- Явно указываем, что это внешний ключ
        public Passenger? Passenger { get; set; }
        
        // === Остальные свойства ===
        public string SeatNumber { get; set; } = string.Empty;
        public decimal Price { get; set; }
        public string Status { get; set; } = "Booked";
        public DateTime PurchaseDate { get; set; } = DateTime.UtcNow;
    }
}
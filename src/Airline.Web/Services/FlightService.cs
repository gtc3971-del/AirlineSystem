using Airline.Web.Data;
using Airline.Web.Dtos;
using Airline.Web.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;

namespace Airline.Web.Services
{
    public class FlightService : IFlightService
    {
        private readonly AirlineDbContext _context;

        public FlightService(AirlineDbContext context)
        {
            _context = context;
        }

        public async Task<List<FlightSearchDto>> SearchFlightsAsync(string fromCode, string toCode, DateTime date)
        {
            return await _context.Flights
                .Where(f => f.DepartureAirportCode == fromCode &&
                            f.ArrivalAirportCode == toCode &&
                            f.DepartureTime.Date == date.Date &&
                            f.Status == "Scheduled")
                .Select(f => new FlightSearchDto
                {
                    Id = f.Id,
                    FlightNumber = f.FlightNumber,
                    DepartureTime = f.DepartureTime,
                    ArrivalTime = f.ArrivalTime,
                    DepartureAirport = f.DepartureAirportCode,
                    ArrivalAirport = f.ArrivalAirportCode,
                    AvailableSeats = f.AvailableSeats
                })
                .ToListAsync();
        }
public async Task<List<Seat>> GetAvailableSeatsAsync(int flightId)
{
    return await _context.Seats
        .Where(s => s.FlightId == flightId && s.Status == "Available")
        .ToListAsync();
}
        public async Task<Ticket> BookTicketAsync(int flightId, string passengerName, string seatNumber)
        {
            using IDbContextTransaction transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                var flight = await _context.Flights.FindAsync(flightId);
                if (flight == null || flight.AvailableSeats <= 0)
                    throw new InvalidOperationException("Рейс не найден или места закончились");

                // Проверка, не забронировано ли уже это место
                bool seatTaken = await _context.Tickets.AnyAsync(t => t.FlightId == flightId && t.SeatNumber == seatNumber);
                if (seatTaken)
                    throw new InvalidOperationException($"Место {seatNumber} уже забронировано");

                // Создаём пассажира (упрощённо)
                var passenger = new Passenger { FullName = passengerName, PassportNumber = "TEMP-" + Guid.NewGuid().ToString().Substring(0, 8) };
                _context.Passengers.Add(passenger);
                await _context.SaveChangesAsync();

                // Создаём билет
                var ticket = new Ticket
                {
                    FlightId = flightId,
                    PassengerId = passenger.Id,
                    SeatNumber = seatNumber.ToUpper(),
                    Price = 5000, // Заглушка, можно вычислять динамически
                    Status = "Booked",
                    PurchaseDate = DateTime.UtcNow
                };
                _context.Tickets.Add(ticket);

                // Уменьшаем количество мест
                flight.AvailableSeats--;
                _context.Flights.Update(flight);

                await _context.SaveChangesAsync();
                await transaction.CommitAsync();

                return ticket;
            }
            catch
            {
                await transaction.RollbackAsync();
                throw;
            }
        }
    }
}